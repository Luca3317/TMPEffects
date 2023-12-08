using static TMPEffects.TextProcessing.ParsingUtility;
using System.Collections.Generic;
using TMPEffects.Tags;

namespace TMPEffects.TextProcessing.TagProcessors
{
    public class EventTagProcessor : ITagProcessor<TMPEventArgs>
    {
        public object Database => null;

        public List<TMPEventArgs> ProcessedTags
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

        public bool Process(TagInfo tagInfo, int textIndex)
        {
            TMPEventArgs args = new TMPEventArgs(textIndex, tagInfo.name, GetTagParametersDict(tagInfo.parameterString, 0));
            ProcessedTags.Add(args);
            return true;
        }

        public void Reset()
        {
            ProcessedTags.Clear();
        }
    }
}