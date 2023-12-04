using static ParsingUtility;
using System.Collections.Generic;

public class AnimationTagProcessor : ITagProcessor<TMPEffectTag>
{
    public object Database => database;

    public List<TMPEffectTag> ProcessedTags
    {
        get; private set;
    }

    TMPEffectsDatabase database;

    public AnimationTagProcessor(TMPEffectsDatabase database)
    {
        this.database = database;
        ProcessedTags = new();
    }

    public bool PreProcess(TagInfo tagInfo)
    {
        // TODO How to handle this case?
        // Technically you probably shouldnt be able to create a
        // processor with an invalid database
        // DO SAME FOR COMMANDTAGPROCESSOR
        if (database == null) return false;

        // check name
        if (!database.Contains(tagInfo.name)) return false;

        if (tagInfo.type == TagType.Open)
        {
            // check parameters
            var parameters = GetTagParametersDict(tagInfo.parameterString, 0);
            if (!database.GetEffect(tagInfo.name).ValidateParameters(parameters)) return false;
        }
        return true;
    }

    public bool Process(TagInfo tagInfo, int textIndex)
    {
        // TODO How to handle this case?
        // Technically you probably shouldnt be able to create a
        // processor with an invalid database
        // DO SAME FOR COMMANDTAGPROCESSOR
        if (database == null) return false;

        // check name
        if (!database.Contains(tagInfo.name)) return false;

        TMPEffectTag tag;
        if (tagInfo.type == TagType.Open)
        {
            // check parameters
            var parameters = GetTagParametersDict(tagInfo.parameterString, 0);
            if (!database.GetEffect(tagInfo.name).ValidateParameters(parameters)) return false;

            tag = new TMPEffectTag(tagInfo.name, textIndex, parameters);
            ProcessedTags.Add(tag);
        }
        else
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

        return true;
    }

    public void Reset()
    {
        ProcessedTags.Clear();
    }
}
