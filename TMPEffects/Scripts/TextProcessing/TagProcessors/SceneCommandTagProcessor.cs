using static TMPEffects.TextProcessing.ParsingUtility;
using System.Collections.Generic;
using TMPEffects.Tags;
using TMPEffects.Commands;
using UnityEngine.UIElements;

namespace TMPEffects.TextProcessing.TagProcessors
{
    public class SceneCommandTagProcessor : ITagProcessor<TMPCommandTag>
    {
        public object Database => tags;

        public List<TMPCommandTag> ProcessedTags
        {
            get; private set;
        }

        Dictionary<string, SceneCommand> tags;

        public SceneCommandTagProcessor(Dictionary<string, SceneCommand> tags)
        {
            this.tags = tags == null ? new Dictionary<string, SceneCommand>() : tags;
            ProcessedTags = new();
        }

        public bool PreProcess(TagInfo tagInfo)
        {
            if (!tags.ContainsKey(tagInfo.name))
            {
                return false;
            }

            if (tagInfo.type == TagType.Open || tags[tagInfo.name].CommandType != CommandType.Index)
            {
                return true;
            }

            return false;
        }

        //public bool ProcessAndClose(TagInfo tagInfo, int textIndex, int length)
        //{
        //    if (Process(tagInfo, textIndex))
        //    {
        //        ProcessedTags[ProcessedTags.Count - 1].Close(textIndex + length - 1);
        //        return true;
        //    }

        //    return false;
        //}

        public bool Process(TagInfo tagInfo, int textIndex, int order)
        {
            if (!tags.ContainsKey(tagInfo.name)) return false;

            if (tagInfo.type == TagType.Open || tags[tagInfo.name].CommandType != CommandType.Index)
            {
                var parameters = GetTagParametersDict(tagInfo.parameterString, 0);
                TMPCommandTag tag = new TMPCommandTag(tagInfo.name, textIndex, order, parameters);
                ProcessedTags.Add(tag);
                return true;
            }

            return false;
        }

        public void Reset()
        {
            ProcessedTags.Clear();
        }
    }
}
