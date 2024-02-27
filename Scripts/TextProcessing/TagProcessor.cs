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
        public readonly ReadOnlyCollection<KeyValuePair<EffectTagIndices, EffectTag>> ProcessedTags;

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
        private List<KeyValuePair<EffectTagIndices, EffectTag>> processedTags;

        internal void AdjustIndices(KeyValuePair<EffectTagIndices, EffectTag> oldPair, KeyValuePair<EffectTagIndices, EffectTag> newPair)
        {
            int index = ProcessedTags.IndexOf(oldPair);
            if (index < 0) return;

            processedTags[index] = newPair;
        }

        private bool Process_Open(ParsingUtility.TagInfo tagInfo, int textIndex, int orderAtIndex)
        {
            EffectTag tag;
            if (!validator.ValidateTag(tagInfo, out tag)) return false;

            EffectTagIndices indices = new EffectTagIndices(textIndex, -1, orderAtIndex);
            KeyValuePair<EffectTagIndices, EffectTag> kvp = new KeyValuePair<EffectTagIndices, EffectTag>(indices, tag);
            processedTags.Add(kvp);

            return true;
        }

        private bool Process_Close(ParsingUtility.TagInfo tagInfo, int textIndex)
        {
            if (!validator.ValidateTag(tagInfo)) return false;

            KeyValuePair<EffectTagIndices, EffectTag> kvp;
            for (int i = ProcessedTags.Count - 1; i >= 0; i--)
            {
                kvp = ProcessedTags[i];
                if (kvp.Key.IsOpen && kvp.Value.Name == tagInfo.name)
                {
                    EffectTagIndices newIndices = new EffectTagIndices(kvp.Key.StartIndex, textIndex, kvp.Key.OrderAtIndex);
                    processedTags[i] = new KeyValuePair<EffectTagIndices, EffectTag>(newIndices, kvp.Value);
                    return true;
                }
            }

            return true;
        }
    }
}
