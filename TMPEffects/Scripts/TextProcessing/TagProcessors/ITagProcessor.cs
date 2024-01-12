using System.Collections.Generic;
using TMPEffects.Tags;

namespace TMPEffects.TextProcessing.TagProcessors
{
    public interface ITagProcessor
    {
        public IEnumerable<TMPEffectTag> ProcessedTags { get; }

        // Check if is valid tag
        public bool PreProcess(ParsingUtility.TagInfo tagInfo);
        // Check if is valid tag and create entry
        public bool Process(ParsingUtility.TagInfo tagInfo, int textIndex);
        public bool Process(ParsingUtility.TagInfo tagInfo, int textIndex, int length);
        public void Reset();
    }

    public interface ITagProcessor<T> : ITagProcessor where T : TMPEffectTag
    {
        IEnumerable<TMPEffectTag> ITagProcessor.ProcessedTags 
        {
            get
            {
                foreach (var tag in ProcessedTags)
                {
                    yield return tag;
                }
            }
        }
        public new List<T> ProcessedTags { get; }
    }
}