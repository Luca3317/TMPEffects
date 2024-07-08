using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using TMPEffects.Tags;
using System.Collections.ObjectModel;
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
            BeginPreProcess?.Invoke(text);

            // Clear style stack, which stores the currently active styles
            styles.Clear();

            // Reset all registered tag processors, clearing their tag collections
            foreach (var processor in processors)
            {
                processor.Reset();
            }

            // Indicates the order of the parsed tags at the respective index
            // i.e. <!wait><#someevent><!playsound> => 0,1,2 respcectively
            int currentOrderAtIndex = 0;

            int searchIndex = 0; // The index used to search the next tag; = previousTag.EndIndex + 1
            int indexOffset = 0; // The offset applied to a tag's text index to accomodate for previous tags
                                 // "<wave>Lorem <shake>ipsum" <wave> has offset 0, textindex 0 => 0
                                 // <shake> has offset 6 to accomodate for "<wave>", textindex 12 => 12 - 6 = 6

            sb = new StringBuilder();
            ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();

            // Whether to parse <=> whether a noparse tag is active
            bool parse = true;

            // Get the stylesheet used by the TMP_Text component
            TMP_StyleSheet sheet = TextComponent.styleSheet != null ? TextComponent.styleSheet : TMP_Settings.defaultStyleSheet;

            // If the text is empty, return " "; quick fix to an issue where empty text is not updated correctly
            if (string.IsNullOrEmpty(text)) return " ";

            // Iterate over all well-formed tags of the text
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
                if (tagInfo.name == "noparse" || tagInfo.name == "NOPARSE")
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
                else if (tagInfo.name == "sprite" || tagInfo.name == "SPRITE")
                {
                    var parameters = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);

                    // if has anim parameter (otherwise not relevant)
                    if (parameters.ContainsKey("anim"))
                    {
                        var split = parameters["anim"].Split(',');

                        // if has valid anim parameter (otherwise not relevant)
                        if (split.Length == 3)
                        {
                            // If tag is not handled, print warning and append indicator to text
                            if (!HandleTag(ref tagInfo, tagInfo.startIndex + indexOffset, currentOrderAtIndex))
                            {
                                //Debug.LogWarning("Native sprite animations (e.g. <sprite anim=\"0,8,10\">) are not supported. Add a TMPAnimator to get the exact same behavior.");
                                sb.Append(" <color=red>!NATIVE SPRITE ANIMATIONS NOT SUPPORTED; ADD TMPANIMATOR!</color> ");
                            }
                            // If tag is handled, append normal <sprite> tag to text
                            else
                            {
                                indexOffset -= (tagInfo.endIndex - tagInfo.startIndex + 1);
                                currentOrderAtIndex++;

                                StringBuilder sb2 = new StringBuilder();
                                sb2.Append($"<sprite");
                                foreach (var parameter in parameters)
                                {
                                    switch (parameter.Key)
                                    {
                                        case "index":
                                        case "INDEX":
                                        case "anim": break; 

                                        case "name":
                                        case "NAME":
                                        case "tint":
                                        case "TINT":
                                        case "color":
                                        case "COLOR":
                                            sb2.Append($" {parameter.Key}=\"{parameter.Value}\""); break;
                                        case "":
                                            if (!string.IsNullOrWhiteSpace(parameter.Value))
                                                sb2.Append($"{parameter.Key}=\"{parameter.Value}\""); break;
                                    }

                                    //if (string.IsNullOrWhiteSpace(parameter.Key)) continue;
                                }
                                sb2.Append(" index=" + split[0]);
                                sb2.Append("></sprite>");

                                text = text.Insert(tagInfo.endIndex + 1, sb2.ToString());
                            }

                            searchIndex = tagInfo.endIndex + 1;
                            continue;
                        }
                    }
                }

                // if the current tag is a style tag, handle it manually;
                // doing this instead of just allowing TextMeshPro to handle it as normal allows you to create style tags with TMPEffects tags
                else if (sheet != null && (tagInfo.name == "style" || tagInfo.name == "STYLE"))
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
            parsed = sb.ToString();

            FinishPreProcess?.Invoke(parsed);

            // Add a space at the end of the text;
            // Quick fix to issues with texts that are empty either after this
            // preprocess or after the native tag processing
            return parsed + " ";
        }

        /// <summary>
        /// Adjust the indices that were cached during the preprocess stage to accomodate
        /// for index changes due to native TextMeshPro tags.
        /// </summary>
        public void AdjustIndices()
        {
            var info = TextComponent.textInfo;

            BeginAdjustIndices?.Invoke(info.textComponent.text);

            // Create a mapping from TagProcessor to their tags with mutable indices
            Dictionary<TagProcessor, List<KeyValuePair<Indices, TMPEffectTag>>> dict = new();
            foreach (var processor in processors)
            {
                dict.Add(processor, new());
                foreach (var tag in processor.ProcessedTags)
                {
                    dict[processor].Add(new KeyValuePair<Indices, TMPEffectTag>(new Indices(tag.Key), tag.Value));
                }
            }

            // Iterate over all TagProcessors
            foreach (var kvp in dict)
            {
                // and their processed tags
                foreach (var thing in kvp.Value)
                {
                    // Set the indices of the tag to the indices indicated by the characterinfo
                    // TODO for long texts, maybe binary search or dynamic (one full iteration and storing indices)
                    // (tested for 1k character texts with 100 tags, slight performance loss still)
                    for (int i = 0; i < info.characterCount; i++)
                    {
                        var cinfo = info.characterInfo[i];

                        if (!thing.Key.startSet && thing.Key.start <= cinfo.index)
                        {
                            thing.Key.start = i;
                            thing.Key.startSet = true;
                        }

                        if (thing.Key.end != -1 && !thing.Key.endSet && thing.Key.end <= cinfo.index)
                        {
                            thing.Key.end = i;
                            thing.Key.endSet = true;
                        }

                        if (thing.Key.startSet && (thing.Key.end == -1 || thing.Key.endSet)) break;
                    }
                }
            }

            // Set the actual indices within the TagProcessors
            // TODO this currently uses an internal method on TagProcessor to set the indices of a tag
            // a little ugly, might be fine to keep
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

            public bool startSet;
            public bool endSet;

            public readonly TMPEffectTagIndices indices;

            public Indices(TMPEffectTagIndices indices)
            {
                this.start = indices.StartIndex;
                this.end = indices.EndIndex;

                startSet = false;
                endSet = false;

                this.indices = indices;
            }
        }
    }
}