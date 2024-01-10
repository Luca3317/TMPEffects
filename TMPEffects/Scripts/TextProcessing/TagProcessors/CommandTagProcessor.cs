using static TMPEffects.TextProcessing.ParsingUtility;
using System.Collections.Generic;
using TMPEffects.Tags;
using TMPEffects.Databases;

namespace TMPEffects.TextProcessing.TagProcessors
{
    public class CommandTagProcessor : ITagProcessor<TMPCommandTag>
    {
        public object Database => database;

        public List<TMPCommandTag> ProcessedTags
        {
            get; private set;
        }

        TMPCommandDatabase database;

        public CommandTagProcessor(TMPCommandDatabase database)
        {
            this.database = database;
            ProcessedTags = new();
        }

        public bool PreProcess(TagInfo tagInfo)
        {
            if (database == null) return false;

            // check name
            if (!database.Contains(tagInfo.name)) return false;

            TMPCommand command;
            if ((command = database.GetEffect(tagInfo.name)) == null) return false;

            if (tagInfo.type == TagType.Open)
            {
                // check parameters
                var parameters = GetTagParametersDict(tagInfo.parameterString, 0);
                if (!command.ValidateParameters(parameters)) return false;
            }
            else if (command.CommandType == CommandType.Index) return false;

            return true;
        }

        public bool Process(TagInfo tagInfo, int textIndex, int length)
        {
            if (Process(tagInfo, textIndex))
            {
                if (database.GetEffect(tagInfo.name).CommandType != CommandType.Index)
                    ProcessedTags[ProcessedTags.Count - 1].Close(textIndex + length - 1);

                return true;
            }

            return false;
        }

        public bool Process(TagInfo tagInfo, int textIndex)
        {
            if (database == null) return false;

            // check name
            if (!database.Contains(tagInfo.name)) return false;

            TMPCommand command;
            if ((command = database.GetEffect(tagInfo.name)) == null) return false;

            TMPCommandTag tag;
            if (tagInfo.type == TagType.Open)
            {
                // check parameters
                var parameters = GetTagParametersDict(tagInfo.parameterString, 0);
                if (!command.ValidateParameters(parameters)) return false;

                tag = new TMPCommandTag(tagInfo.name, textIndex, parameters);
                ProcessedTags.Add(tag);
            }
            else if (command.CommandType != CommandType.Index)
            {
                for (int i = ProcessedTags.Count - 1; i >= 0; i--)
                {
                    tag = ProcessedTags[i];

                    if (tag.IsOpen && tag.IsEqual(tagInfo.name))
                    {
                        tag.Close(textIndex - 1);
                        break;
                    }
                }
            }
            else return false;

            return true;
        }

        public void Reset()
        {
            ProcessedTags.Clear();
        }
    }
}