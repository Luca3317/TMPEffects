using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPEffects.Tags;

namespace TMPEffects.TextProcessing
{
    /// <summary>
    /// Handles processing of, and stores successfully processed tags.
    /// </summary>
    public sealed class TagProcessor
    {
        /// <summary>
        /// All processed tags.
        /// </summary>
        public readonly ReadOnlyCollection<KeyValuePair<TMPEffectTagIndices, TMPEffectTag>> ProcessedTags;

        /// <summary>
        /// Tags with this keyword as name will close all open tags of this tag processor.
        /// </summary>
        public const string ALL_KEYWORD = "all";
        /// <summary>
        /// Tags with this keyword as name will close teh most recent open tag of this tag processor.
        /// </summary>
        public const string MOST_RECENT_KEYWORD = "";

        public TagProcessor(ITMPTagValidator validator)
        {
            processedTags = new();
            ProcessedTags = new(processedTags);
            this.validator = validator;
        }

        /// <summary>
        /// Process the given tag.
        /// </summary>
        /// <param name="tagInfo">Information about the tag.</param>
        /// <param name="textIndex">The index of the tag within its source text.</param>
        /// <param name="orderAtIndex">The order at the index of the tag withing its source text.</param>
        /// <returns>true if tag is successfully processed, false otherwise.</returns>
        public bool Process(ParsingUtility.TagInfo tagInfo, int textIndex, int orderAtIndex)
        {
            if (tagInfo.type == ParsingUtility.TagType.Open) return Process_Open(tagInfo, textIndex, orderAtIndex);
            else return Process_Close(tagInfo, textIndex);
        }

        /// <summary>
        /// Reset this TagProcessor.
        /// </summary>
        public void Reset()
        {
            processedTags.Clear();
        }

        private ITMPTagValidator validator;
        private List<KeyValuePair<TMPEffectTagIndices, TMPEffectTag>> processedTags;

        internal void AdjustIndices(KeyValuePair<TMPEffectTagIndices, TMPEffectTag> oldPair, KeyValuePair<TMPEffectTagIndices, TMPEffectTag> newPair)
        {
            int index = ProcessedTags.IndexOf(oldPair);
            if (index < 0) return;

            processedTags[index] = newPair;
        }

        private bool Process_Open(ParsingUtility.TagInfo tagInfo, int textIndex, int orderAtIndex)
        {
            TMPEffectTag tag;
            int endIndex;
            if (!validator.ValidateOpenTag(tagInfo, out tag, out endIndex)) return false;

            // TODO I dont like this; potentially rework (split) ITMPTagValidator and its integration into TagProcessor
            // Fix the end index
            endIndex = endIndex == -1 ? -1 : endIndex - tagInfo.startIndex + textIndex;

            TMPEffectTagIndices indices = new TMPEffectTagIndices(textIndex, endIndex, orderAtIndex);
            KeyValuePair<TMPEffectTagIndices, TMPEffectTag> kvp = new KeyValuePair<TMPEffectTagIndices, TMPEffectTag>(indices, tag);
            processedTags.Add(kvp);

            return true;
        }

        private bool Process_Close(ParsingUtility.TagInfo tagInfo, int textIndex)
        {
            if (tagInfo.name == MOST_RECENT_KEYWORD)
            {
                return CloseMostRecent(textIndex);
            }
            else if (tagInfo.name == ALL_KEYWORD)
            {
                CloseAll(textIndex);
                return true;
            }

            if (!validator.ValidateTag(tagInfo)) return false;

            KeyValuePair<TMPEffectTagIndices, TMPEffectTag> kvp;
            for (int i = ProcessedTags.Count - 1; i >= 0; i--)
            {
                kvp = ProcessedTags[i];
                if (kvp.Key.IsOpen && kvp.Value.Name == tagInfo.name)
                {
                    TMPEffectTagIndices newIndices = new TMPEffectTagIndices(kvp.Key.StartIndex, textIndex, kvp.Key.OrderAtIndex);
                    processedTags[i] = new KeyValuePair<TMPEffectTagIndices, TMPEffectTag>(newIndices, kvp.Value);
                    return true;
                }
            }

            return false;
        }

        private bool CloseMostRecent(int textIndex)
        {
            KeyValuePair<TMPEffectTagIndices, TMPEffectTag> kvp;
            for (int i = ProcessedTags.Count - 1; i >= 0; i--)
            {
                kvp = ProcessedTags[i];
                if (kvp.Key.IsOpen)
                {
                    TMPEffectTagIndices newIndices = new TMPEffectTagIndices(kvp.Key.StartIndex, textIndex, kvp.Key.OrderAtIndex);
                    processedTags[i] = new KeyValuePair<TMPEffectTagIndices, TMPEffectTag>(newIndices, kvp.Value);
                    return true;
                }
            }

            return false;
        }

        private void CloseAll(int textIndex)
        {
            KeyValuePair<TMPEffectTagIndices, TMPEffectTag> kvp;
            for (int i = ProcessedTags.Count - 1; i >= 0; i--)
            {
                kvp = ProcessedTags[i];
                if (kvp.Key.IsOpen)
                {
                    TMPEffectTagIndices newIndices = new TMPEffectTagIndices(kvp.Key.StartIndex, textIndex, kvp.Key.OrderAtIndex);
                    processedTags[i] = new KeyValuePair<TMPEffectTagIndices, TMPEffectTag>(newIndices, kvp.Value);
                }
            }
        }
    }
}
