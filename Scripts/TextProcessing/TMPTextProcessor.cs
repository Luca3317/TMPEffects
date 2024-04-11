using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using TMPEffects.Tags;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace TMPEffects.TextProcessing
{
    /// <summary>
    /// Preprocesses the text, removing valid tags.<br/>
    /// As part of a post process, also adjusts the indices of the parsed tags to
    /// accomodate for native TextMeshPro tags.
    /// </summary>
    public class TMPTextProcessor : ITextPreprocessor, ITagProcessorManager
    {
        /// <summary>
        /// The associated <see cref="TMP_Text"/> component.
        /// </summary>
        public TMP_Text TextComponent { get; private set; }

        ///<inheritdoc/>
        public ReadOnlyDictionary<char, ReadOnlyCollection<TagProcessor>> TagProcessors => ((ITagProcessorManager)processors).TagProcessors;

        public delegate void TMPTextProcessorEventHandler(string text);
        /// <summary>
        /// Raised just before the PreProcess routine begins.
        /// </summary>
        public event TMPTextProcessorEventHandler BeginPreProcess;
        /// <summary>
        /// Raised once the PreProcess routine finished.
        /// </summary>
        public event TMPTextProcessorEventHandler FinishPreProcess;
        /// <summary>
        /// Raised just before the AdjustIndices routine begins.
        /// </summary>
        public event TMPTextProcessorEventHandler BeginAdjustIndices;
        /// <summary>
        /// Raised once the AdjustIndices routine finished.
        /// </summary>
        public event TMPTextProcessorEventHandler FinishAdjustIndices;

        public TMPTextProcessor(TMP_Text text)
        {
            sb = new StringBuilder();
            processors = new TagProcessorManager();

            TextComponent = text;
        }

        ///<inheritdoc/>
        public void AddProcessor(char prefix, TagProcessor processor, int priority = 0) => processors.AddProcessor(prefix, processor, priority);
        ///<inheritdoc/>
        public bool RemoveProcessor(char prefix, TagProcessor processor) => processors.RemoveProcessor(prefix, processor);
        ///<inheritdoc/>
        public IEnumerator<TagProcessor> GetEnumerator() => processors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => processors.GetEnumerator();

        /// <summary>
        /// Preprocess the text.<br/>
        /// - Remove TMPEffects tags from text<br/>
        /// - Cache the tags incl. their indices
        /// </summary>
        /// <param name="text">The text to preprocess.</param>
        /// <returns>The preprocessed text.</returns>
        public string PreprocessText(string text)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            BeginPreProcess?.Invoke(text);

            styles.Clear();
            foreach (var processor in processors)
            {
                processor.Reset();
            }

            // Indicates the order of the parsed tags at the respective index
            // i.e. <!wait><#someevent><!playsound> => 0,1,2 respcectively
            int currentOrderAtIndex = 0;

            int indexOffset = 0;
            int searchIndex = 0;
            sb = new StringBuilder();
            ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();

            bool parse = true;

            TMP_StyleSheet sheet = TextComponent.styleSheet != null ? TextComponent.styleSheet : TMP_Settings.defaultStyleSheet;

            while (ParsingUtility.GetNextTag(text, searchIndex, ref tagInfo))
            {
                // If the searchIndex is not equal to the startIndex of the tag, meaning there was text between the previous tag and the current one,
                // add the text inbetween the tags to the StringBuilder
                if (searchIndex != tagInfo.startIndex)
                {
                    currentOrderAtIndex = 0;
                    sb.Append(text.AsSpan(searchIndex, tagInfo.startIndex - searchIndex));
                }

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

                // if the current tag is an animated sprite text, handle it manually
                else if (tagInfo.name == "sprite")
                {
                    var parameters = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);
                    if (parameters.ContainsKey("anim"))
                    {
                        Debug.Log("ANIMATED SPRITE TAG");
                        var split = parameters["anim"].Split(',');
                        if (split.Length == 3)
                        {
                            Debug.Log("Opening tag at " + (tagInfo.startIndex + indexOffset));
                            if (!HandleTag(ref tagInfo, tagInfo.startIndex + indexOffset, currentOrderAtIndex))
                            {
                                Debug.LogWarning("Native sprite animations (e.g. <sprite anim=\"0,8,10\">) are not supported. Add a TMPAnimator to get the exact same behavior.");
                                sb.Append(" <color=red>!SEE CONSOLE!</color> ");
                            }
                            else
                            {
                                indexOffset -= (tagInfo.endIndex - tagInfo.startIndex + 1);
                                currentOrderAtIndex++;

                                StringBuilder sb2 = new StringBuilder();
                                sb2.Append($"<sprite={split[0]}");
                                foreach (var parameter in parameters)
                                {
                                    if (string.IsNullOrWhiteSpace(parameter.Key)) continue;
                                    if (parameter.Key == "anim") continue;
                                    sb2.Append($" {parameter.Key}=\"{parameter.Value}\"");
                                }
                                sb2.Append("></sprite>");

                                text = text.Insert(tagInfo.endIndex + 1, sb2.ToString());
                            }

                            searchIndex = tagInfo.endIndex + 1;
                            continue;
                        }
                    }
                }

                // if the current tag is a style tag, handle it manually
                else if (sheet != null && tagInfo.name == "style")
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
                    else if (tagInfo.parameterString.Length > 6)
                    {
                        TMP_Style style;
                        int start = 6, end = tagInfo.parameterString.Length - 1;
                        if (start >= end)
                            if (tagInfo.parameterString[start] == '\"') start++;
                        if (tagInfo.parameterString[end] == '\"') end--;

                        style = sheet.GetStyle(tagInfo.parameterString.Substring(start, end - start + 1));
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
                    currentOrderAtIndex = 0;
                    sb.Append(text.AsSpan(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));
                    searchIndex = tagInfo.endIndex + 1;
                    continue;
                }

                // Handle the tag; if it fails, meaning this is not a valid custom tag, append the tag to the StringBuilder
                if (!HandleTag(ref tagInfo, tagInfo.startIndex + indexOffset, currentOrderAtIndex))
                {
                    sb.Append(text.AsSpan(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));

                    // Dont reset order, as this might be a valid native tag, meaning the previous
                    // and the next tag may still share an index; if not thats fine, order will just start
                    // at n > 0 but still maintain its order
                    //currentOrderAtIndex = 0;
                }
                // If it succeeds, adjust the indexOffset accordingly
                else
                {
                    indexOffset -= (tagInfo.endIndex - tagInfo.startIndex + 1);
                    currentOrderAtIndex++;
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

            sw.Stop();

            Debug.Log("Parsed: " + parsed);
            return parsed;
        }

        /// <summary>
        /// Adjust the indices that were cached during the preprocess stage to accomodate
        /// for index changes due to native TextMeshPro tags.
        /// </summary>
        public void AdjustIndices()
        {
            var info = TextComponent.textInfo;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            BeginAdjustIndices?.Invoke(info.textComponent.text);

            Dictionary<TagProcessor, List<KeyValuePair<Indices, TMPEffectTag>>> dict = new();
            foreach (var processor in processors)
            {
                dict.Add(processor, new());
                foreach (var tag in processor.ProcessedTags)
                {
                    dict[processor].Add(new KeyValuePair<Indices, TMPEffectTag>(new Indices(tag.Key), tag.Value));
                }
            }

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

                        foreach (var list in dict.Values)
                        {
                            foreach (var kvp in list)
                            {
                                if (kvp.Key.originalEnd == -1)
                                {
                                    if (kvp.Key.originalStart >= lastIndex)
                                    {
                                        kvp.Key.start += insertedCharacters;
                                    }
                                }
                                else
                                {
                                    if (kvp.Key.originalEnd < lastIndex) continue;

                                    // If tag begins after inserted text
                                    if (kvp.Key.originalStart >= lastIndex)
                                    {
                                        kvp.Key.start += insertedCharacters;
                                    }
                                    kvp.Key.end += insertedCharacters;
                                }
                            }
                        }
                    }
                    // If the index incremented by more than one => text removed
                    else if (cInfo.index > lastIndex)
                    {
                        int diff = cInfo.index - lastIndex - 1;

                        foreach (var list in dict.Values)
                        {
                            foreach (var kvp in list)
                            {
                                if (kvp.Key.originalEnd == -1)
                                {
                                    if (kvp.Key.originalStart > lastIndex + 1)
                                    {
                                        kvp.Key.start -= diff;
                                    }
                                }
                                else
                                {
                                    if (kvp.Key.originalEnd <= lastIndex) continue;

                                    // If tag begins after inserted text
                                    if (kvp.Key.originalStart > lastIndex + 1)
                                    {
                                        kvp.Key.start -= diff;
                                    }
                                    kvp.Key.end -= diff;
                                }
                            }
                        }
                    }
                    // If the index became lower again -- is there any case where that may happen?
                    else
                    {
                        Debug.LogWarning("Undefined case; character index became lower again");
                    }
                }

                lastIndex = cInfo.index;
            }

            // Added as quick fix for something like this:
            // "A<color=white><wave>"; TODO cleaner fix?
            foreach (var kvp in dict)
            {
                foreach (var thing in kvp.Value)
                {
                    if (thing.Key.start > info.characterCount)
                    {
                        thing.Key.start = info.characterCount;
                    }
                }
            }

            foreach (var kvp in dict)
            {
                foreach (var thing in kvp.Value)
                {
                    kvp.Key.AdjustIndices(
                        new KeyValuePair<TMPEffectTagIndices, TMPEffectTag>(thing.Key.indices, thing.Value),
                        new KeyValuePair<TMPEffectTagIndices, TMPEffectTag>(new TMPEffectTagIndices(thing.Key.start, thing.Key.end, thing.Key.indices.OrderAtIndex), thing.Value));
                }
            }

            FinishAdjustIndices?.Invoke(info.textComponent.text);

            sw.Stop();
        }

        private TagProcessorManager processors;

        private StringBuilder sb;
        private Stack<TMP_Style> styles = new();

        private bool HandleTag(ref ParsingUtility.TagInfo tagInfo, int textIndex, int order)
        {
            ReadOnlyCollection<TagProcessor> coll;
            if (!processors.TagProcessors.TryGetValue(tagInfo.prefix, out coll))
                return false;

            if (coll.Count == 1)
                return coll[0].Process(tagInfo, textIndex, order);

            for (int i = 0; i < coll.Count; i++)
            {
                if (coll[i].Process(tagInfo, textIndex, order))
                    return true;
            }

            return false;
        }

        private class Indices
        {
            public int start;
            public int end;

            public readonly int originalStart;
            public readonly int originalEnd;

            public readonly TMPEffectTagIndices indices;

            public Indices(TMPEffectTagIndices indices)
            {
                this.start = indices.StartIndex;
                this.end = indices.EndIndex;

                this.originalStart = indices.StartIndex;
                this.originalEnd = indices.EndIndex;

                this.indices = indices;
            }
        }
    }
}