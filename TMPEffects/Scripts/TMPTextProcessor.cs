using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class TMPTextProcessor : ITextPreprocessor
{
    Dictionary<char, ITagProcessor> tagProcessors;

    private StringBuilder sb;

    public delegate void TMPEffectPreProcesserEventHandler(string text);
    public event TMPEffectPreProcesserEventHandler BeginPreProcess;
    public event TMPEffectPreProcesserEventHandler FinishPreProcess;

    public TMPTextProcessor()
    {
        sb = new StringBuilder();
        tagProcessors = new();
    }

    public void RegisterPreprocessor(char prefix, ITagProcessor preprocessor)
    {
        if (tagProcessors.ContainsKey(prefix)) return;
        tagProcessors.Add(prefix, preprocessor);
    }

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

    public string PreprocessText(string text)
    {
        sw.Reset();
        sw.Start();

        BeginPreProcess?.Invoke(text);

        foreach (var key in tagProcessors.Keys)
        {
            tagProcessors[key].Reset();
        }

        int searchIndex = 0;
        sb = new StringBuilder();
        ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();

        int parse = 0;

        // Iterate over the text until there is no next tag
        while (ParsingUtility.GetNextTag(text, searchIndex, ref tagInfo))
        {
            // If the searchIndex is not equal to the startIndex of the tag, meaning there was text between the previous tag and the current one,
            // add the text inbetween the tags to the StringBuilder
            if (searchIndex != tagInfo.startIndex)
                sb.Append(text.AsSpan(searchIndex, tagInfo.startIndex - searchIndex));

            // If the current tag is a noparse tag, toggle whether to parse the succeeding text
            if (tagInfo.name == "noparse")
            {
                int prev = parse;
                if (tagInfo.type == ParsingUtility.TagType.Open) parse++;
                else parse = parse = Mathf.Max(0, parse - 1);

                //if (prev != parse)
                //{
                //    searchIndex = tagInfo.endIndex + 1;
                //    continue;
                //}

                //searchIndex = tagInfo.endIndex + 1;
                //continue;
            }

            // If a noparse tag is active, simply append the tag to the StringBuilder, adjust the searchIndex and continue to the next tag
            if (parse != 0)
            {
                sb.Append(text.AsSpan(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));
                searchIndex = tagInfo.endIndex + 1;
                continue;
            }

            // Handle the tag; if it fails, meaning this is not a valid custom tag, append the tag to the StringBuilder
            if (!HandleTagPreprocess(ref tagInfo, sb.Length))
                sb.Append(text.AsSpan(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));

            // Adjust the search index and continue to the next tag
            searchIndex = tagInfo.endIndex + 1;
        }

        // Append any text that came after the last tag
        sb.Append(text.AsSpan(searchIndex, text.Length - searchIndex));

        // TMP_Text seems to not correctly update for empty text, so return a whitespace character if the
        // preprocessed text is empty.
        if (sb.Length == 0)
        {
            FinishPreProcess?.Invoke(" ");
            sw.Stop();
            return " ";
        }
        FinishPreProcess?.Invoke(sb.ToString());
        sw.Stop();
        return sb.ToString();
    }


    public void Process(string rawText, string parsedText)
    {
        sw.Start();
        BeginPreProcess?.Invoke(parsedText);

        foreach (var key in tagProcessors.Keys)
        {
            tagProcessors[key].Reset();
        }

        ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();

        // The index offset between the raw and the parsedText;
        // Invariant: rawText.len >= parsedTextLen => indexOffset <= 0;
        int indexOffset = 0;
        int searchIndex = 0;

        int parsedLen = parsedText.Length;
        bool parsedOver = false;
        int parse = 0;

        Debug.Log("Processing");
        Debug.Log("Raw: " + rawText);
        Debug.Log("Parsed: " + parsedText);

        ReadOnlySpan<char> parsedSpan = parsedText;
        ReadOnlySpan<char> rawSpan = rawText;

        // Iterate over the text until there is no next tag
        while (ParsingUtility.GetNextTag(rawText, searchIndex, ref tagInfo))
        {
            Debug.Log("TAG: " + tagInfo.name + "  StartIndex: " + tagInfo.startIndex);
            Debug.Log("Raw: " + rawText.Substring(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));

            int parsedStart = tagInfo.startIndex + indexOffset;
            int parsedEnd = Mathf.Min(parsedLen - parsedStart - 1, parsedStart + (tagInfo.endIndex - tagInfo.startIndex + 1));
            if (parsedEnd > parsedStart)
                Debug.Log("Parsed: " + parsedText.Substring(parsedStart, parsedEnd - parsedStart) + "   Indexoffset = " + indexOffset);


            // Check if the start of the current tag, with the indexOffset applied, exceeds the parsed tag and set a flag accordingly
            if (!parsedOver)
                parsedOver = tagInfo.startIndex + indexOffset >= parsedLen;

            // If the current tag is a noparse tag, toggle whether to parse the succeeding text
            if (tagInfo.name == "noparse")
            {
                Debug.Log("Got tag with noparse name at " + tagInfo.startIndex + ": " + rawText.Substring(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));
                int prev = parse;
                if (tagInfo.type == ParsingUtility.TagType.Open) parse++;
                else parse = Mathf.Max(0, parse - 1);

                indexOffset -= tagInfo.endIndex - tagInfo.startIndex + 1;
                searchIndex = tagInfo.endIndex + 1;
                continue;
            }

            if (parse < 0) Debug.LogWarning("PARSE UNDER 0");

            // If a noparse tag is active, simply adjust the searchIndex and continue to the next tag
            if (parse != 0)
            {
                Debug.Log("Continue cause !parse");
                searchIndex = tagInfo.endIndex + 1;
                continue;
            }

            // If the current tag's startIndex exceeds the parsed text, meaning the current (and all following) tags are only contained
            // within the raw text, handle the current tag and adjust the search index
            if (parsedOver)
            {
                Debug.Log("Continue cause parsedOver");
                HandleTag(ref tagInfo, tagInfo.startIndex + indexOffset);
                searchIndex = tagInfo.endIndex + 1;
                continue;
            }

            int tagLen = tagInfo.endIndex - tagInfo.startIndex + 1;

            // If the parsed text does not have an open bracket at its corresponding position, only the raw text contains the current tag
            // Handle the tag and adjust the indexOffset
            if (parsedText[tagInfo.startIndex + indexOffset] != '<')
            {
                Debug.Log("Continue cause no '<' detected; will adjjust offset by " + tagLen);

                // If handle tag returns false, ergo this is a native tag
                if (!HandleTag(ref tagInfo, tagInfo.startIndex + indexOffset))
                {
                    //// If is close noparse tag
                    //if (rawText[tagInfo.startIndex + 1] == '/' &&
                    //    System.MemoryExtensions.Equals(rawSpan.Slice(tagInfo.startIndex + 2, 6), "noparse", StringComparison.Ordinal))
                    //{
                    //    parse = true;
                    //}
                    //// If is open noparse tag
                    //else if (System.MemoryExtensions.Equals(rawSpan.Slice(tagInfo.startIndex + 1, 6), "noparse", StringComparison.Ordinal))
                    //{
                    //    parse = false;
                    //}
                }
                Debug.Log("Indexoffset was " + indexOffset);
                indexOffset -= tagLen;
                Debug.Log("Indexoffset is " + indexOffset);
            }

            // Otherwise, do a more expensive check to see if it is the same tag
            // (as opposed to a tag directly following the current one or normal text containing a '<').
            // If it is, do nothing.
            else if (!System.MemoryExtensions.Equals(
                parsedSpan.Slice(tagInfo.startIndex + indexOffset, Mathf.Min(tagLen, parsedLen - (tagInfo.startIndex + indexOffset))),
                rawSpan.Slice(tagInfo.startIndex, tagLen), StringComparison.Ordinal)
                )
            {
                Debug.Log("Both contained the tag: " + parsedText.Substring(tagInfo.startIndex + indexOffset, Mathf.Min(tagLen, parsedLen - (tagInfo.startIndex + indexOffset))));
                if (HandleTag(ref tagInfo, tagInfo.startIndex + indexOffset))
                {
                }

                indexOffset -= tagLen;
            }


            searchIndex = tagInfo.endIndex + 1;
        }


        FinishPreProcess?.Invoke(parsedText);

        sw.Stop();

        Debug.Log("The entire processing work (preprocess + postprocess) took " + sw.Elapsed.TotalMilliseconds);
    }

    bool HandleTag(ref ParsingUtility.TagInfo tagInfo, int textIndex)
    {
        if (tagProcessors.ContainsKey(tagInfo.prefix))
        {
            return tagProcessors[tagInfo.prefix].Preprocess(tagInfo, textIndex);
        }

        return false;
    }

    bool HandleTagPreprocess(ref ParsingUtility.TagInfo tagInfo, int textIndex)
    {
        if (tagProcessors.ContainsKey(tagInfo.prefix))
        {
            return tagProcessors[tagInfo.prefix].Preprocess(tagInfo, textIndex);
        }

        return false;
    }
}


/*
 * Backup - works but unstable (especially anything noparse related)
 * 
 * 
 * using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class TMPTextProcessor : ITextPreprocessor
{
    Dictionary<char, ITagProcessor> tagProcessors;

    private StringBuilder sb;

    public delegate void TMPEffectPreProcesserEventHandler(string text);
    public event TMPEffectPreProcesserEventHandler BeginPreProcess;
    public event TMPEffectPreProcesserEventHandler FinishPreProcess;

    public TMPTextProcessor()
    {
        sb = new StringBuilder();
        tagProcessors = new();
    }

    public void RegisterPreprocessor(char prefix, ITagProcessor preprocessor)
    {
        if (tagProcessors.ContainsKey(prefix)) return;
        tagProcessors.Add(prefix, preprocessor);
    }

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

    public string PreprocessText(string text)
    {
        sw.Reset();
        sw.Start();

        BeginPreProcess?.Invoke(text);

        foreach (var key in tagProcessors.Keys)
        {
            tagProcessors[key].Reset();
        }

        int searchIndex = 0;
        sb = new StringBuilder();
        ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();

        int parse = 0;

        // Iterate over the text until there is no next tag
        while (ParsingUtility.GetNextTag(text, searchIndex, ref tagInfo))
        {
            // If the searchIndex is not equal to the startIndex of the tag, meaning there was text between the previous tag and the current one,
            // add the text inbetween the tags to the StringBuilder
            if (searchIndex != tagInfo.startIndex)
                sb.Append(text.AsSpan(searchIndex, tagInfo.startIndex - searchIndex));

            // If the current tag is a noparse tag, toggle whether to parse the succeeding text
            if (tagInfo.name == "noparse")
            {
                int prev = parse;
                if (tagInfo.type == ParsingUtility.TagType.Open) parse++;
                else parse = parse = Mathf.Max(0, parse - 1);

                //if (prev != parse)
                //{
                //    searchIndex = tagInfo.endIndex + 1;
                //    continue;
                //}

                //searchIndex = tagInfo.endIndex + 1;
                //continue;
            }

            // If a noparse tag is active, simply append the tag to the StringBuilder, adjust the searchIndex and continue to the next tag
            if (parse != 0)
            {
                sb.Append(text.AsSpan(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));
                searchIndex = tagInfo.endIndex + 1;
                continue;
            }

            // Handle the tag; if it fails, meaning this is not a valid custom tag, append the tag to the StringBuilder
            if (!HandleTagPreprocess(ref tagInfo, sb.Length))
                sb.Append(text.AsSpan(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));

            // Adjust the search index and continue to the next tag
            searchIndex = tagInfo.endIndex + 1;
        }

        // Append any text that came after the last tag
        sb.Append(text.AsSpan(searchIndex, text.Length - searchIndex));

        // TMP_Text seems to not correctly update for empty text, so return a whitespace character if the
        // preprocessed text is empty.
        if (sb.Length == 0)
        {
            FinishPreProcess?.Invoke(" ");
            sw.Stop();
            return " ";
        }
        FinishPreProcess?.Invoke(sb.ToString());
        sw.Stop();
        return sb.ToString();
    }


    public void Process(string rawText, string parsedText)
    {
        sw.Start();
        BeginPreProcess?.Invoke(parsedText);

        foreach (var key in tagProcessors.Keys)
        {
            tagProcessors[key].Reset();
        }

        ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();

        // The index offset between the raw and the parsedText;
        // Invariant: rawText.len >= parsedTextLen => indexOffset <= 0;
        int indexOffset = 0;
        int searchIndex = 0;

        int parsedLen = parsedText.Length;
        bool parsedOver = false;
        int parse = 0;

        Debug.Log("Processing");
        Debug.Log("Raw: " + rawText);
        Debug.Log("Parsed: " + parsedText);

        ReadOnlySpan<char> parsedSpan = parsedText;
        ReadOnlySpan<char> rawSpan = rawText;

        // Iterate over the text until there is no next tag
        while (ParsingUtility.GetNextTag(rawText, searchIndex, ref tagInfo))
        {
            Debug.Log("TAG: " + tagInfo.name + "  StartIndex: " + tagInfo.startIndex);
            Debug.Log("Raw: " + rawText.Substring(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));

            int parsedStart = tagInfo.startIndex + indexOffset;
            int parsedEnd = Mathf.Min(parsedLen - parsedStart - 1, parsedStart + (tagInfo.endIndex - tagInfo.startIndex + 1));
            if (parsedEnd > parsedStart)
                Debug.Log("Parsed: " + parsedText.Substring(parsedStart, parsedEnd - parsedStart) + "   Indexoffset = " + indexOffset);


            // Check if the start of the current tag, with the indexOffset applied, exceeds the parsed tag and set a flag accordingly
            if (!parsedOver)
                parsedOver = tagInfo.startIndex + indexOffset >= parsedLen;

            // If the current tag is a noparse tag, toggle whether to parse the succeeding text
            if (tagInfo.name == "noparse")
            {
                Debug.Log("Got tag with noparse name at " + tagInfo.startIndex + ": " + rawText.Substring(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));
                int prev = parse;
                if (tagInfo.type == ParsingUtility.TagType.Open) parse++;
                else parse = Mathf.Max(0, parse - 1);

                indexOffset -= tagInfo.endIndex - tagInfo.startIndex + 1;
                searchIndex = tagInfo.endIndex + 1;
                continue;
            }

            if (parse < 0) Debug.LogWarning("PARSE UNDER 0");

            // If a noparse tag is active, simply adjust the searchIndex and continue to the next tag
            if (parse != 0)
            {
                Debug.Log("Continue cause !parse");
                searchIndex = tagInfo.endIndex + 1;
                continue;
            }

            // If the current tag's startIndex exceeds the parsed text, meaning the current (and all following) tags are only contained
            // within the raw text, handle the current tag and adjust the search index
            if (parsedOver)
            {
                Debug.Log("Continue cause parsedOver");
                HandleTag(ref tagInfo, tagInfo.startIndex + indexOffset);
                searchIndex = tagInfo.endIndex + 1;
                continue;
            }

            int tagLen = tagInfo.endIndex - tagInfo.startIndex + 1;

            // If the parsed text does not have an open bracket at its corresponding position, only the raw text contains the current tag
            // Handle the tag and adjust the indexOffset
            if (parsedText[tagInfo.startIndex + indexOffset] != '<')
            {
                Debug.Log("Continue cause no '<' detected; will adjjust offset by " + tagLen);

                // If handle tag returns false, ergo this is a native tag
                if (!HandleTag(ref tagInfo, tagInfo.startIndex + indexOffset))
                {
                    //// If is close noparse tag
                    //if (rawText[tagInfo.startIndex + 1] == '/' &&
                    //    System.MemoryExtensions.Equals(rawSpan.Slice(tagInfo.startIndex + 2, 6), "noparse", StringComparison.Ordinal))
                    //{
                    //    parse = true;
                    //}
                    //// If is open noparse tag
                    //else if (System.MemoryExtensions.Equals(rawSpan.Slice(tagInfo.startIndex + 1, 6), "noparse", StringComparison.Ordinal))
                    //{
                    //    parse = false;
                    //}
                }
                Debug.Log("Indexoffset was " + indexOffset);
                indexOffset -= tagLen;
                Debug.Log("Indexoffset is " + indexOffset);
            }

            // Otherwise, do a more expensive check to see if it is the same tag
            // (as opposed to a tag directly following the current one or normal text containing a '<').
            // If it is, do nothing.
            else if (!System.MemoryExtensions.Equals(
                parsedSpan.Slice(tagInfo.startIndex + indexOffset, Mathf.Min(tagLen, parsedLen - (tagInfo.startIndex + indexOffset))),
                rawSpan.Slice(tagInfo.startIndex, tagLen), StringComparison.Ordinal)
                )
            {
                Debug.Log("Both contained the tag: " + parsedText.Substring(tagInfo.startIndex + indexOffset, Mathf.Min(tagLen, parsedLen - (tagInfo.startIndex + indexOffset))));
                if (HandleTag(ref tagInfo, tagInfo.startIndex + indexOffset))
                {
                }

                indexOffset -= tagLen;
            }


            searchIndex = tagInfo.endIndex + 1;
        }


        FinishPreProcess?.Invoke(parsedText);

        sw.Stop();

        Debug.Log("The entire processing work (preprocess + postprocess) took " + sw.Elapsed.TotalMilliseconds);
    }

    bool HandleTag(ref ParsingUtility.TagInfo tagInfo, int textIndex)
    {
        if (tagProcessors.ContainsKey(tagInfo.prefix))
        {
            return tagProcessors[tagInfo.prefix].Preprocess(tagInfo, textIndex);
        }

        return false;
    }

    bool HandleTagPreprocess(ref ParsingUtility.TagInfo tagInfo, int textIndex)
    {
        if (tagProcessors.ContainsKey(tagInfo.prefix))
        {
            return tagProcessors[tagInfo.prefix].Preprocess(tagInfo, textIndex);
        }

        return false;
    }
}

 * 
 */