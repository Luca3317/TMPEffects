using System.Collections.Generic;

public interface ITagProcessor
{
    // TODO Unnecessarily expensive to check both times
    // Refacto: Preprocess checks and holds empty entry
    // Process populates the entries with the correct indices

    public object Database { get; }
    
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