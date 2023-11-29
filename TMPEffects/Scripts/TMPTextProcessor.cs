using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TMPTextProcessor : ITextPreprocessor
{
    Dictionary<char, ITagProcessor> tagProcessors;

    private StringBuilder sb;

    public delegate void TMPTextProcessorEventHandler(string text);
    public event TMPTextProcessorEventHandler BeginPreProcess;
    public event TMPTextProcessorEventHandler FinishPreProcess;
    public event TMPTextProcessorEventHandler BeginProcessTags;
    public event TMPTextProcessorEventHandler FinishProcessTags;

    public TMPTextProcessor()
    {
        sb = new StringBuilder();
        tagProcessors = new();
    }

    public void RegisterProcessor(char prefix, ITagProcessor preprocessor)
    {
        if (preprocessor == null)
        {
            Debug.LogError("Tried to register null processor");
            throw new System.ArgumentNullException(nameof(preprocessor));
        }

        if (tagProcessors.ContainsKey(prefix))
        {
            Debug.LogWarning("Tried to register processor with duplicate prefix:\"" + prefix + "\"");
            return;
        }
        Debug.LogWarning("Registered processor with prefix: \"" + prefix + "\"");
        tagProcessors.Add(prefix, preprocessor);
    }

    public void UnregisterProcessor(char prefix)
    {
        if (!tagProcessors.ContainsKey(prefix))
        {
            Debug.LogWarning("Tried to unregister processor with prefix:\"" + prefix + "\", which was not registered");
            return;
        }
        Debug.LogWarning("Unregistered processor with prefix: \"" + prefix + "\"");
        tagProcessors.Remove(prefix);
    }

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

    /*
     * Update noparse handling:
     *  check if tag.name == noparse
     *  if so, skip but insert <noparse>; this ensures that the tag is detected by both native parser and processor
     */

    public string PreprocessText(string text)
    {
        Debug.Log("PREProcess text with " + tagProcessors.Keys.Count + " tag processors");
        foreach (var pr in tagProcessors)
        {
            Debug.Log("Processor: " + pr.Value.GetType().ToString());
        }

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

        bool parse = true;

        // Iterate over the text until there is no next tag
        while (ParsingUtility.GetNextTag(text, searchIndex, ref tagInfo))
        {
            Debug.Log("Found tag : " + tagInfo.name);

            // If the searchIndex is not equal to the startIndex of the tag, meaning there was text between the previous tag and the current one,
            // add the text inbetween the tags to the StringBuilder
            if (searchIndex != tagInfo.startIndex)
                sb.Append(text.AsSpan(searchIndex, tagInfo.startIndex - searchIndex));

            // If the current tag is a noparse tag, toggle whether to parse the succeeding text
            if (tagInfo.name == "noparse")
            {
                if (tagInfo.type == ParsingUtility.TagType.Open)
                {
                    sb.Append("<noparse>");
                    parse = false;
                }
                else
                {
                    sb.Append("</noparse>");
                    parse = true;
                }

                searchIndex = tagInfo.endIndex + 1;
                continue;
            }

            // If a noparse tag is active, simply append the tag to the StringBuilder, adjust the searchIndex and continue to the next tag
            if (!parse)
            {
                sb.Append(text.AsSpan(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));
                searchIndex = tagInfo.endIndex + 1;
                continue;
            }

            // Handle the tag; if it fails, meaning this is not a valid custom tag, append the tag to the StringBuilder
            if (!HandleTagPreprocess(ref tagInfo))
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
            Debug.Log("Returning ");
            return " ";
        }
        FinishPreProcess?.Invoke(sb.ToString());
        sw.Stop();
        Debug.Log("Returning " + sb.ToString());
        return sb.ToString();
    }



    /*
     * TODO the index in TMP_CharacterInfo actually refers to its index in the raw preprocessed text; can i use that to simplify this?
     * 
     */

    public void ProcessTags(string rawText, string parsedText)
    {
        Debug.Log("Process text with " + tagProcessors.Keys.Count + " tag processors");
        Debug.Log("rawtext len: " + rawText.Length + " PArsedtext len: " + parsedText.Length);
        Debug.Log("Raw: " + rawText);
        Debug.Log("Parsed: " + parsedText);

        sw.Start();
        BeginProcessTags?.Invoke(parsedText);

        foreach (var key in tagProcessors.Keys)
        {
            tagProcessors[key].Reset();
        }

        ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();

        // The index offset between the raw and the parsedText;
        // Invariant: rawText.len >= parsedTextLen => indexOffset <= 0;
        int indexOffset = 0;
        int searchIndex = 0;
        int prevSearchIndex = 0;

        int parsedLen = parsedText.Length;
        bool parsedOver = false;
        bool parse = true;

        //Debug.Log("Processing");
        //Debug.Log("Raw: " + rawText);
        //Debug.Log("Parsed: " + parsedText);

        ReadOnlySpan<char> parsedSpan = parsedText;
        ReadOnlySpan<char> rawSpan = rawText;


        // Iterate over the text until there is no next tag
        while (ParsingUtility.GetNextTag(rawText, searchIndex, ref tagInfo))
        {
            int tagLen = tagInfo.endIndex - tagInfo.startIndex + 1;

            //Debug.Log("TAG: " + tagInfo.name + "  StartIndex: " + tagInfo.startIndex);
            //Debug.Log("Raw: " + rawText.Substring(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));

            //int parsedStart = tagInfo.startIndex + indexOffset;
            //int parsedEnd = Mathf.Min(parsedLen - parsedStart - 1, parsedStart + (tagInfo.endIndex - tagInfo.startIndex + 1));
            //if (parsedEnd > parsedStart)
            //Debug.Log("Parsed: " + parsedText.Substring(parsedStart, parsedEnd - parsedStart) + "   Indexoffset = " + indexOffset);


            // Check if the start of the current tag, with the indexOffset applied, exceeds the parsed tag and set a flag accordingly
            if (!parsedOver)
                parsedOver = tagInfo.startIndex + indexOffset >= parsedLen;

            // If the current tag is a noparse tag, toggle whether to parse the succeeding text
            if (tagInfo.name == "noparse")
            {
                //Debug.Log("Got tag with noparse name at " + tagInfo.startIndex + ": " + rawText.Substring(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));
                if (tagInfo.type == ParsingUtility.TagType.Open)
                {
                    parse = false;
                }
                else
                {
                    parse = true;
                }

                indexOffset -= tagInfo.endIndex - tagInfo.startIndex + 1;

                prevSearchIndex = searchIndex;
                searchIndex = tagInfo.endIndex + 1;
                continue;
            }

            //if (parse < 0) Debug.LogWarning("PARSE UNDER 0");

            // If a noparse tag is active, simply adjust the searchIndex and continue to the next tag
            if (!parse)
            {
                //Debug.Log("Continue cause !parse");

                prevSearchIndex = searchIndex;
                searchIndex = tagInfo.endIndex + 1;
                continue;
            }

            // If the current tag's startIndex exceeds the parsed text, meaning the current (and all following) tags are only contained
            // within the raw text, handle the current tag and adjust the search index
            if (parsedOver)
            {
                // In some cases, ie wrap mode truncated, the parsed text will be cut off;
                // This checks if this is the case by affirming whether there was text between
                // the previous tag and this one; if so, the textindex is clamped to the parsedLen
                if (tagInfo.startIndex > prevSearchIndex + 1)
                {
                    Debug.LogWarning("There was cut off text detected; all following tags will use parsed.length as closer");
                    HandleTag(ref tagInfo, parsedLen);// tagInfo.startIndex + indexOffset);
                }
#if UNITY_EDITOR
                else if (tagInfo.startIndex < prevSearchIndex)
                {
                    Debug.LogError("The prev search Index was larger than the current tags startIndex; that should not be possible");
                    return;
                }
#endif
                else
                {
                    HandleTag(ref tagInfo, tagInfo.startIndex + indexOffset);
                }

                prevSearchIndex = searchIndex;
                searchIndex = tagInfo.endIndex + 1;
                continue;
            }

            // If the parsed text does not have an open bracket at its corresponding position, only the raw text contains the current tag
            // Handle the tag and adjust the indexOffset
            if (parsedText[tagInfo.startIndex + indexOffset] != '<')
            {
                //Debug.Log("Continue cause no '<' detected; will adjjust offset by " + tagLen);

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
                indexOffset -= tagLen;
            }

            // Otherwise, do a more expensive check to see if it is the same tag
            // (as opposed to a tag directly following the current one or normal text containing a '<').
            // If it is, do nothing.
            else if (!System.MemoryExtensions.Equals(
                parsedSpan.Slice(tagInfo.startIndex + indexOffset, Mathf.Min(tagLen, parsedLen - (tagInfo.startIndex + indexOffset))),
                rawSpan.Slice(tagInfo.startIndex, tagLen), StringComparison.Ordinal)
                )
            {
                //Debug.Log("Both contained the tag: " + parsedText.Substring(tagInfo.startIndex + indexOffset, Mathf.Min(tagLen, parsedLen - (tagInfo.startIndex + indexOffset))));
                if (HandleTag(ref tagInfo, tagInfo.startIndex + indexOffset))
                {
                }

                indexOffset -= tagLen;
            }

            prevSearchIndex = searchIndex;
            searchIndex = tagInfo.endIndex + 1;
        }

        FinishProcessTags?.Invoke(parsedText);
        sw.Stop();
    }

    bool HandleTag(ref ParsingUtility.TagInfo tagInfo, int textIndex)
    {
        if (tagProcessors.ContainsKey(tagInfo.prefix))
        {
            return tagProcessors[tagInfo.prefix].Process(tagInfo, textIndex);
        }

        return false;
    }

    bool HandleTagPreprocess(ref ParsingUtility.TagInfo tagInfo)
    {
        if (tagProcessors.ContainsKey(tagInfo.prefix))
        {
            return tagProcessors[tagInfo.prefix].PreProcess(tagInfo);
        }

        return false;
    }
}
