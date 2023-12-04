using static ParsingUtility;
using System.Collections.Generic;

public class CommandTagProcessor : ITagProcessor<TMPCommandArgs>
{
    public object Database => database;

    public List<TMPCommandArgs> ProcessedTags
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
            return true;
        }

        return false;
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

        if (tagInfo.type == TagType.Open)
        {
            // check parameters
            var parameters = GetTagParametersDict(tagInfo.parameterString, 0);

            if (!database.GetCommand(tagInfo.name).ValidateParameters(parameters)) return false;

            TMPCommandArgs args = new TMPCommandArgs(textIndex, tagInfo.name, GetTagParametersDict(tagInfo.parameterString, 0));
            ProcessedTags.Add(args);
            return true;
        }

        return false;
    }

    public void Reset()
    {
        ProcessedTags.Clear();
    }
}