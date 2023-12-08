using System.Collections.Generic;

namespace TMPEffects.TextProcessing.TagProcessors
{
    public interface ITagProcessor
    {
        // Check if is valid tag
        public bool PreProcess(ParsingUtility.TagInfo tagInfo);
        // Check if is valid tag and create entry
        public bool Process(ParsingUtility.TagInfo tagInfo, int textIndex);
        public void Reset();
    }

    public interface ITagProcessor<T> : ITagProcessor
    {
        public List<T> ProcessedTags { get; }
    }
}