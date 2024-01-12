using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using TMPEffects.TextProcessing.TagProcessors;
using TMPEffects.Tags;

namespace TMPEffects.TextProcessing
{
    internal class TMPTextProcessor : ITextPreprocessor
    {
        public TMP_Text TextComponent { get; private set; }

        public Dictionary<char, ITagProcessor> TagProcessors => new(tagProcessors);
        Dictionary<char, ITagProcessor> tagProcessors;

        private StringBuilder sb;
        private Dictionary<TMPEffectTag, Indeces> newIndeces = new();
        private Stack<TMP_Style> styles = new();

        public delegate void TMPTextProcessorEventHandler(string text);
        public event TMPTextProcessorEventHandler BeginPreProcess;
        public event TMPTextProcessorEventHandler FinishPreProcess;
        public event TMPTextProcessorEventHandler BeginAdjustIndeces;
        public event TMPTextProcessorEventHandler FinishAdjustIndeces;

        public TMPTextProcessor(TMP_Text text)
        {
            sb = new StringBuilder();
            tagProcessors = new();
            this.TextComponent = text;
        }

        public void RegisterProcessor<T>(char prefix, ITagProcessor<T> preprocessor) where T : TMPEffectTag
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

        /// <summary>
        /// Preprocess the text.<br/>
        /// - Remove TMPEffects tags from text
        /// - Cache the tags incl. their indices
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string PreprocessText(string text)
        {
            BeginPreProcess?.Invoke(text);

            styles.Clear();
            foreach (var key in tagProcessors.Keys)
            {
                tagProcessors[key].Reset();
            }

            int indexOffset = 0;
            int searchIndex = 0;
            sb = new StringBuilder();
            ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();

            bool parse = true;

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
                else if (TextComponent.styleSheet != null && tagInfo.name == "style"/* && tagInfo.parameterString.Length > 8*/)
                {
                    if (tagInfo.type == ParsingUtility.TagType.Close)
                    {
                        text = text.Remove(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1);
                        if (styles.Count > 0)
                        {
                            text = text.Insert(tagInfo.startIndex, styles.Pop().styleClosingDefinition);
                        }

                        searchIndex = tagInfo.startIndex;
                        continue;
                    }
                    else
                    {
                        TMP_Style style;
                        int start = 6, end = tagInfo.parameterString.Length - 1;

                        if (tagInfo.parameterString[start] == '\"') start++;
                        if (tagInfo.parameterString[end] == '\"') end--;

                        style = TextComponent.styleSheet.GetStyle(tagInfo.parameterString.Substring(start, end - start + 1));
                        if (style != null)
                        {
                            text = text.Remove(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1);
                            text = text.Insert(tagInfo.startIndex, style.styleOpeningDefinition);
                            styles.Push(style);

                            searchIndex = tagInfo.startIndex;
                            continue;
                        }
                    }
                }

                // If a noparse tag is active, simply append the tag to the StringBuilder, adjust the searchIndex and continue to the next tag
                if (!parse)
                {
                    sb.Append(text.AsSpan(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));
                    searchIndex = tagInfo.endIndex + 1;
                    continue;
                }

                // Handle the tag; if it fails, meaning this is not a valid custom tag, append the tag to the StringBuilder
                if (!HandleTag(ref tagInfo, tagInfo.startIndex + indexOffset))
                {
                    sb.Append(text.AsSpan(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));
                }
                // If it succeeds, adjust the indexOffset accordingly
                else
                {
                    indexOffset -= (tagInfo.endIndex - tagInfo.startIndex + 1);
                }

                // Adjust the search index and continue to the next tag
                searchIndex = tagInfo.endIndex + 1;
            }

            // Append any text that came after the last tag
            sb.Append(text.AsSpan(searchIndex, text.Length - searchIndex));

            string parsed;
            if (sb.Length == 0) parsed = " ";
            else parsed = sb.ToString();

            FinishPreProcess?.Invoke(parsed);
            return parsed;
        }

        private class Indeces
        {
            public int start;
            public int end;

            public Indeces(int start, int end)
            {
                this.start = start;
                this.end = end;
            }
        }

        /// <summary>
        /// Adjust the indeces that were cached during the preprocess stage
        /// to text removed and inserted by TextMeshPro.
        /// </summary>
        /// <param name="info"></param>
        public void AdjustIndeces(TMP_TextInfo info)
        {
            BeginAdjustIndeces?.Invoke(info.textComponent.text);

            newIndeces.Clear();
            foreach (var processor in tagProcessors.Values)
                foreach (var tag in processor.ProcessedTags)
                    newIndeces.Add(tag, new Indeces(tag.startIndex, tag.endIndex));

            int lastIndex = -1;

            for (int i = 0; i < info.characterCount; i++)
            {
                var cInfo = info.characterInfo[i];

                if (cInfo.index - lastIndex != 1)
                {
                    // If the index did not change => inserted text
                    if (cInfo.index == lastIndex)
                    {
                        int insertedCharacters = 1;
                        while (i++ < info.characterCount && info.characterInfo[i].index == lastIndex)
                        {
                            insertedCharacters++;
                        }

                        foreach (var kvp in newIndeces)
                        {
                            if (kvp.Key.IsOpen)
                            {
                                if (kvp.Key.startIndex >= lastIndex)
                                {
                                    kvp.Value.start += insertedCharacters;
                                }
                            }
                            else
                            {
                                if (kvp.Key.endIndex < lastIndex) continue;

                                // If tag begins after inserted text
                                if (kvp.Key.startIndex >= lastIndex)
                                {
                                    kvp.Value.start += insertedCharacters;
                                }
                                kvp.Value.end += insertedCharacters;
                            }
                        }
                    }
                    // If the index incremented by more than one => text removed
                    else if (cInfo.index > lastIndex)
                    {
                        int diff = cInfo.index - lastIndex - 1;

                        foreach (var kvp in newIndeces)
                        {

                            if (kvp.Key.IsOpen)
                            {
                                if (kvp.Key.startIndex > lastIndex + 1)
                                {
                                    kvp.Value.start -= diff;
                                }
                            }
                            else
                            {
                                if (kvp.Key.endIndex <= lastIndex) continue;

                                // If tag begins after inserted text
                                if (kvp.Key.startIndex > lastIndex + 1)
                                {
                                    kvp.Value.start -= diff;
                                }
                                kvp.Value.end -= diff;
                            }
                        }
                    }
                    // If the index became lower again -- is there any case where that may happen?
                    else
                    {
                        Debug.LogWarning("Undefined case");
                    }
                }

                lastIndex = cInfo.index;
            }

            foreach (var kvp in newIndeces)
            {
                kvp.Key.SetStartIndex(kvp.Value.start);
                kvp.Key.SetEndIndex(kvp.Value.end);
            }

            FinishAdjustIndeces?.Invoke(info.textComponent.text);
        }

        private bool HandleTag(ref ParsingUtility.TagInfo tagInfo, int textIndex)
        {
            if (tagProcessors.ContainsKey(tagInfo.prefix))
            {
                return tagProcessors[tagInfo.prefix].Process(tagInfo, textIndex);
            }

            return false;
        }
    }
}