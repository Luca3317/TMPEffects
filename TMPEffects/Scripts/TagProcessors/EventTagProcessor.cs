using static ParsingUtility;
using System.Collections.Generic;

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