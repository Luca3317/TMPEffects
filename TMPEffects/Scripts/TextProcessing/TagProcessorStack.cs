using System.Collections.Generic;
using TMPEffects.TextProcessing.TagProcessors;
using static TMPEffects.TextProcessing.ParsingUtility;

namespace TMPEffects.TextProcessing
{
    public class TagProcessorStack<T> : ITagProcessor<T>
    {
        public List<T> ProcessedTags
        {
            get
            {
                if (processedTags == null)
                {
                    processedTags = new List<T>();
                    foreach (var tagProcessor in tagProcessors)
                    {
                        processedTags.AddRange(tagProcessor.ProcessedTags);
                    }
                }
                return processedTags;
            }
        }

        private List<T> processedTags = null;
        private List<ITagProcessor<T>> tagProcessors = null;

        public TagProcessorStack()
        {
            processedTags = null;
            tagProcessors = new List<ITagProcessor<T>>();
        }

        public void AddProcessor(ITagProcessor<T> processor)
        {
            if (tagProcessors.Contains(processor)) return;
            tagProcessors.Add(processor);
        }

        public void RemoveProcessor(ITagProcessor<T> processor)
        {
            tagProcessors.Remove(processor);
        }

        public bool PreProcess(ParsingUtility.TagInfo tagInfo)
        {
            for (int i = 0; i < tagProcessors.Count; i++)
            {
                if (tagProcessors[i].PreProcess(tagInfo)) return true;
            }

            return false;
        }

        public bool Process(TagInfo tagInfo, int textIndex, int length)
        {
            for (int i = 0; i < tagProcessors.Count; i++)
            {
                if (tagProcessors[i].Process(tagInfo, textIndex, length)) return true;
            }

            return false;
        }

        public bool Process(ParsingUtility.TagInfo tagInfo, int textIndex)
        {
            for (int i = 0; i < tagProcessors.Count; i++)
            {
                if (tagProcessors[i].Process(tagInfo, textIndex)) return true;
            }

            return false;
        }

        public void Reset()
        {
            for (int i = 0; i < tagProcessors.Count; i++)
            {
                tagProcessors[i].Reset();
            }

            processedTags = null;
        }
    }
}
