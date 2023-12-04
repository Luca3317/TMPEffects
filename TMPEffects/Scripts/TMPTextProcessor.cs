using Codice.Client.BaseCommands;
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
            throw new System.ArgumentNullException(nameof(preprocessor));
        }

        if (tagProcessors.ContainsKey(prefix))
        {
            return;
        }
        tagProcessors.Add(prefix, preprocessor);
    }

    public void UnregisterProcessor(char prefix)
    {
        if (!tagProcessors.ContainsKey(prefix))
        {
            return;
        }
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
            return " ";
        }
        FinishPreProcess?.Invoke(sb.ToString());
        sw.Stop();
        return sb.ToString();
    }



    /*
     * TODO the index in TMP_CharacterInfo actually refers to its index in the raw preprocessed text; can i use that to simplify this?
     * 
     */

    public void ProcessTags(string rawText, string parsedText)
    {
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

        ReadOnlySpan<char> parsedSpan = parsedText;
        ReadOnlySpan<char> rawSpan = rawText;

        // Iterate over the text until there is no next tag
        while (ParsingUtility.GetNextTag(rawText, searchIndex, ref tagInfo))
        {
            int tagLen = tagInfo.endIndex - tagInfo.startIndex + 1;

            // Check if the start of the current tag, with the indexOffset applied, exceeds the parsed tag and set a flag accordingly
            if (!parsedOver)
                parsedOver = tagInfo.startIndex + indexOffset >= parsedLen;

            // If the current tag is a noparse tag, toggle whether to parse the succeeding text
            if (tagInfo.name == "noparse")
            {
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

            // If a noparse tag is active, simply adjust the searchIndex and continue to the next tag
            if (!parse)
            {
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
