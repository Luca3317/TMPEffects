using static TMPEffects.TextProcessing.ParsingUtility;
using System.Collections.Generic;
using TMPEffects.Tags;

namespace TMPEffects.TextProcessing.TagProcessors
{
    public class EventTagProcessor : ITagProcessor<TMPEventTag>
    {
        public object Database => null;

        public List<TMPEventTag> ProcessedTags
        {
            get; private set;
        }

        public EventTagProcessor()
        {
            ProcessedTags = new();
        }

        public bool PreProcess(TagInfo info)
        {
            return true;
        }

        public bool Process(TagInfo tagInfo, int textIndex, int length)
        {
            return Process(tagInfo, textIndex);
        }

        public bool Process(TagInfo tagInfo, int textIndex)
        {
            TMPEventTag args = new TMPEventTag(textIndex, tagInfo.name, GetTagParametersDict(tagInfo.parameterString, 0));
            ProcessedTags.Add(args);
            return true;
        }

        public void Reset()
        {
            ProcessedTags.Clear();
        }
    }
}