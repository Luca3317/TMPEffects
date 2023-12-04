using static ParsingUtility;
using System.Collections.Generic;

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
        // TODO How to handle this case?
        // Technically you probably shouldnt be able to create a
        // processor with an invalid database
        // DO SAME FOR ANIMTAGPROCESSOR
        if (database == null) return false;

        // check name
        if (!database.Contains(tagInfo.name)) return false;

        if (tagInfo.type == TagType.Open)
        {
            // check parameters
            var parameters = GetTagParametersDict(tagInfo.parameterString, 0);
            if (!database.GetCommand(tagInfo.name).ValidateParameters(parameters)) return false;
        }
        else if (database.GetCommand(tagInfo.name).CommandType == CommandType.Index) return false;

        return true;
    }

    public bool Process(TagInfo tagInfo, int textIndex)
    {
        // TODO How to handle this case?
        // Technically you probably shouldnt be able to create a
        // processor with an invalid database
        // DO SAME FOR ANIMTAGPROCESSOR
        if (database == null) return false;

        // check name
        if (!database.Contains(tagInfo.name)) return false;

        TMPCommandTag tag;
        if (tagInfo.type == TagType.Open)
        {
            // check parameters
            var parameters = GetTagParametersDict(tagInfo.parameterString, 0);
            if (!database.GetCommand(tagInfo.name).ValidateParameters(parameters)) return false;

            tag = new TMPCommandTag(tagInfo.name, textIndex, parameters);
            ProcessedTags.Add(tag);

            //TMPCommandArgs args = new TMPCommandArgs(textIndex, tagInfo.name, GetTagParametersDict(tagInfo.parameterString, 0));
            //ProcessedTags.Add(args);
            //return true;
        }
        else if (database.GetCommand(tagInfo.name).CommandType != CommandType.Index)
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
        else return false; // TODO Not remove invalid clsoing command tags to communicate they dont need to be closed

        return true;
    }

    public void Reset()
    {
        ProcessedTags.Clear();
    }
}