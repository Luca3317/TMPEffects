using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System;
using static ParsingUtility;

public class OLDTMPEffectPreProcessor : ITextPreprocessor
{
    // TODO list for animations, commands and events
    public List<TMPEffectTag> tags = new List<TMPEffectTag>();
    public List<TMPEventArgs> events = new List<TMPEventArgs>();
    public List<TMPCommand> commands = new List<TMPCommand>();

    TMPEffectsDatabase effectsDatabase;

    private StringBuilder sb;

    public OLDTMPEffectPreProcessor(TMPEffectsDatabase effectsDatabase)
    {
        this.effectsDatabase = effectsDatabase;
        sb = new StringBuilder();
    }

    public string PreprocessText(string text)
    {
        // TODO
        // Need to optimized this method

        tags.Clear();
        events.Clear();

        int index = 0;
        int startIndex, endIndex;
        sb.Clear();

        int textIndex = 0;
        while (ParsingUtility.GetNextTagIndeces(text, index, out startIndex, out endIndex))
        {
            if (textIndex != startIndex)
            {
                sb.Append(text.Substring(textIndex, startIndex - textIndex));
            }

            string tagString = text.Substring(startIndex, endIndex - startIndex + 1);
            ProcessTag(tagString, startIndex, endIndex, out textIndex, out index);
        }

        sb.Append(text.Substring(textIndex, text.Length - textIndex));

        // Returning an empty string does not update TMP_Text correctly

        foreach (var tag in tags)
        {
            if (tag.IsOpen) tag.Close(sb.Length - 1);
        }

        if (sb.Length == 0) return " ";
        return sb.ToString();

        //ParsingUtility.Getnexttagdtopwatch.Reset();
        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
        //tags.Clear();

        //int searchIndex = 0;
        //StringBuilder sb = new StringBuilder();
        //ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();

        //while (ParsingUtility.GetNextTag(text, searchIndex, ref tagInfo))
        //{
        //    if (searchIndex != tagInfo.startIndex)
        //        sb.Append(text.AsSpan(searchIndex, tagInfo.startIndex - searchIndex));

        //    if (!HandleTag(ref tagInfo, sb.Length))
        //        sb.Append(text.AsSpan(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));

        //    searchIndex = tagInfo.endIndex + 1;
        //}

        //sb.Append(text.AsSpan(searchIndex, text.Length - searchIndex));

        //foreach (TMPEffectTag tag in tags)
        //{
        //    if (tag.IsOpen)
        //    {
        //        tag.Close(sb.Length - 1);
        //    }
        //}

        //sw.Stop();
        //Debug.Log("Preprocess took " + sw.Elapsed.TotalMilliseconds + " of which getntexttag took " + ParsingUtility.Getnexttagdtopwatch.Elapsed.TotalMilliseconds);

        //if (sb.Length == 0) return " ";
        //return sb.ToString();

    }

    private void ProcessTag(string tagString, int startIndex, int endIndex, out int textIndex, out int index)
    {
        bool processed = false;

        switch (ParsingUtility.GetTagPrefix(tagString))
        {
            case ParsingUtility.NO_PREFIX: processed = ProcessAnimation(tagString); break;
            case '#': processed = ProcessEvent(tagString); break;
            case '"': break;
        }

        if (processed)
        {
            textIndex = endIndex + 1;
            index = endIndex + 1;
        }
        else
        {
            textIndex = startIndex;
            index = startIndex + 1;
        }
    }


    bool ProcessAnimation(string tagString)
    {
        string name = ParsingUtility.GetTagName(tagString);

        if (!effectsDatabase.Contains(name))
        {
            return false;
        }

        TMPEffectTag tag;
        ParsingUtility.TagType type = ParsingUtility.GetType(tagString);

        // If is opening tag
        if (type == ParsingUtility.TagType.Open)
        {
            var paramsDict = ParsingUtility.GetTagParametersDict(tagString);

            // EXPERIMENTAL
            // It would be cool to be able to define formulas to use to set values

            if (!effectsDatabase.GetEffect(name).ValidateParameters(paramsDict))
            {
                return false;
            }

            tag = new TMPEffectTag(name, sb.Length, paramsDict);
            tags.Add(tag);
        }
        else
        {
            for (int i = tags.Count - 1; i >= 0; i--)
            {
                tag = tags[i];

                if (tag.IsEqual(name))
                {
                    if (tag.IsOpen) tag.Close(sb.Length - 1);
                    break;
                }
            }
        }

        return true;
    }

    bool ProcessEvent(string tagString)
    {
        TMPEventArgs args = new TMPEventArgs(sb.Length, ParsingUtility.GetTagName(tagString), ParsingUtility.GetTagParametersDict(tagString));
        events.Add(args);
        return true;
    }





    // TODO Support various types of tags (Effect/animation, Command, Event)
    bool HandleTag(ref ParsingUtility.TagInfo tagInfo, int textIndex)
    {
        switch (tagInfo.prefix)
        {
            case ParsingUtility.NO_PREFIX:
                return ParseAnimation(ref tagInfo, textIndex);

            case '#':
                return ParseEvent(ref tagInfo, textIndex);

            case '!':
                return ParseCommand(ref tagInfo, textIndex);

            default: Debug.LogError("WTF"); return false;
        }
    }

    bool ParseAnimation(ref ParsingUtility.TagInfo tagInfo, int textIndex)
    {
        // check name
        if (!effectsDatabase.Contains(tagInfo.name)) return false;

        TMPEffectTag tag;
        if (tagInfo.type == ParsingUtility.TagType.Open)
        {
            // check parameters
            var parameters = ParsingUtility.GetTagParametersDict(tagInfo.parameterString, 0);
            if (!effectsDatabase.GetEffect(tagInfo.name).ValidateParameters(parameters)) return false;

            tag = new TMPEffectTag(tagInfo.name, textIndex, parameters);
            tags.Add(tag);
        }
        else
        {
            // Prefixes on closing tags are invalid (TODO for now)
            if (tagInfo.prefix != ParsingUtility.NO_PREFIX)
                return false;

            for (int i = tags.Count - 1; i >= 0; i--)
            {
                tag = tags[i];

                if (tag.IsOpen && tag.IsEqual(tagInfo.name))
                {
                    tag.Close(textIndex - 1);
                    break;
                }
            }
        }

        return true;
    }

    bool ParseEvent(ref ParsingUtility.TagInfo tagInfo, int textIndex)
    {
        TMPEventArgs args = new TMPEventArgs(textIndex, tagInfo.name, ParsingUtility.GetTagParametersDict(tagInfo.parameterString, 0));
        events.Add(args);
        return true;
    }

    bool ParseCommand(ref ParsingUtility.TagInfo tagInfo, int textIndex)
    {


        //TMPEventArgs args = new TMPEventArgs(textIndex, tagInfo.name, ParsingUtility.GetTagParametersDict(tagInfo.parameterString, 0));
        //events.Add(args);
        return true;
    }
}




public interface ITagProcessor
{
    // TODO Unnecessarily expensive to check both times
    // Refacto: Preprocess checks and holds empty entry
    // Process populates the entries with the correct indices

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

public class AnimationTagProcessor : ITagProcessor<TMPEffectTag>
{
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
        Debug.Log("Preprocessing tag: " + tagInfo.name + "; Database == null " + (database == null));
        // TODO How to handle this case?
        // Technically you probably shouldnt be able to create a
        // processor with an invalid database
        // DO SAME FOR COMMANDTAGPROCESSOR
        if (database == null) return false;
        Debug.Log("Preprocessing tag: " + tagInfo.name + "; within actual!");

        // check name
        if (!database.Contains(tagInfo.name)) return false;
        Debug.Log("Preprocessing tag: " + tagInfo.name + "; within actual!");

        if (tagInfo.type == ParsingUtility.TagType.Open)
        {
            // check parameters
            var parameters = ParsingUtility.GetTagParametersDict(tagInfo.parameterString, 0);
            if (!database.GetEffect(tagInfo.name).ValidateParameters(parameters)) return false;
        }

        Debug.Log("Preprocessing tag: " + tagInfo.name + "; within actual!");
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
        if (tagInfo.type == ParsingUtility.TagType.Open)
        {
            // check parameters
            var parameters = ParsingUtility.GetTagParametersDict(tagInfo.parameterString, 0);
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

public class CommandTagProcessor : ITagProcessor<TMPCommandArgs>
{
    public List<TMPCommandArgs> ProcessedTags
    {
        get; private set;
    }

    TMPCommandDatabase database;

    public CommandTagProcessor(TMPCommandDatabase database)
    {
        Debug.Log("Created command tag processor with " + (database == null));
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
        Debug.Log("Preprocessing command");
        if (!database.Contains(tagInfo.name)) return false;

        if (tagInfo.type == ParsingUtility.TagType.Open)
        {
            // check parameters
            var parameters = ParsingUtility.GetTagParametersDict(tagInfo.parameterString, 0);
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

        if (tagInfo.type == ParsingUtility.TagType.Open)
        {
            // check parameters
            var parameters = ParsingUtility.GetTagParametersDict(tagInfo.parameterString, 0);

            if (!database.GetCommand(tagInfo.name).ValidateParameters(parameters)) return false;

            TMPCommandArgs args = new TMPCommandArgs(textIndex, tagInfo.name, ParsingUtility.GetTagParametersDict(tagInfo.parameterString, 0));
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

public class EventTagProcessor : ITagProcessor<TMPEventArgs>
{
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
        TMPEventArgs args = new TMPEventArgs(textIndex, tagInfo.name, ParsingUtility.GetTagParametersDict(tagInfo.parameterString, 0));
        ProcessedTags.Add(args);
        return true;
    }

    public void Reset()
    {
        ProcessedTags.Clear();
    }
}

public class TMPEffectPreProcessor : ITextPreprocessor
{
    Dictionary<char, ITagProcessor> tagPreprocessors;

    private StringBuilder sb;

    public delegate void TMPEffectPreProcesserEventHandler(string text);
    public event TMPEffectPreProcesserEventHandler BeginPreProcess;
    public event TMPEffectPreProcesserEventHandler FinishPreProcess;

    public TMPEffectPreProcessor()
    {
        sb = new StringBuilder();
        tagPreprocessors = new();
    }

    public void RegisterPreprocessor(char prefix, ITagProcessor preprocessor)
    {
        if (tagPreprocessors.ContainsKey(prefix)) return;
        tagPreprocessors.Add(prefix, preprocessor);
    }

    /*
     * TODO
     * Needs to respect TMPS native tags
     * Solution 1: iterate over the string like ive been doing and remove the tags
     */
    public string PreprocessText(string text)
    {
        BeginPreProcess?.Invoke(text);

        foreach (var key in tagPreprocessors.Keys)
        {
            tagPreprocessors[key].Reset();
        }

        int searchIndex = 0;
        sb = new StringBuilder();
        ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();

        while (ParsingUtility.GetNextTag(text, searchIndex, ref tagInfo))
        {
            if (searchIndex != tagInfo.startIndex)
                sb.Append(text.AsSpan(searchIndex, tagInfo.startIndex - searchIndex));

            if (!HandleTag(ref tagInfo, sb.Length))
                sb.Append(text.AsSpan(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));

            searchIndex = tagInfo.endIndex + 1;
        }

        sb.Append(text.AsSpan(searchIndex, text.Length - searchIndex));

        if (sb.Length == 0)
        {
            FinishPreProcess?.Invoke(" ");
            return " ";
        }
        FinishPreProcess?.Invoke(sb.ToString());
        return sb.ToString();
    }

    bool HandleTag(ref ParsingUtility.TagInfo tagInfo, int textIndex)
    {
        if (tagPreprocessors.ContainsKey(tagInfo.prefix))
        {
            return tagPreprocessors[tagInfo.prefix].Process(tagInfo, textIndex);
        }

        return false;
    }
}

//public class TMPEffectPreProcessorBCAKUP : ITextPreprocessor
//{
//    Dictionary<char, ITagPreprocessor> tagPreprocessors;

//    private StringBuilder sb;
//    private TMP_Text text;

//    public delegate void TMPEffectPreProcesserEventHandler(string text);
//    public event TMPEffectPreProcesserEventHandler BeginPreProcess;
//    public event TMPEffectPreProcesserEventHandler FinishPreProcess;

//    public TMPEffectPreProcessor(TMP_Text text)
//    {
//        this.text = text;
//        sb = new StringBuilder();
//        tagPreprocessors = new();
//    }

//    public void RegisterPreprocessor(char prefix, ITagPreprocessor preprocessor)
//    {
//        if (tagPreprocessors.ContainsKey(prefix)) return;
//        tagPreprocessors.Add(prefix, preprocessor);
//    }

//    public string PreprocessText(string text)
//    {
//        BeginPreProcess?.Invoke(text);

//        foreach (var key in tagPreprocessors.Keys)
//        {
//            tagPreprocessors[key].Reset();
//        }

//        int searchIndex = 0;
//        StringBuilder sb = new StringBuilder();
//        ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();

//        while (ParsingUtility.GetNextTag(text, searchIndex, ref tagInfo))
//        {
//            if (searchIndex != tagInfo.startIndex)
//                sb.Append(text.AsSpan(searchIndex, tagInfo.startIndex - searchIndex));

//            if (!HandleTag(ref tagInfo, sb.Length))
//                sb.Append(text.AsSpan(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));

//            searchIndex = tagInfo.endIndex + 1;
//        }

//        sb.Append(text.AsSpan(searchIndex, text.Length - searchIndex));

//        if (sb.Length == 0)
//        {
//            FinishPreProcess?.Invoke(" ");
//            return " ";
//        }
//        FinishPreProcess?.Invoke(sb.ToString());
//        return sb.ToString();
//    }

//    bool HandleTag(ref ParsingUtility.TagInfo tagInfo, int textIndex)
//    {
//        if (tagPreprocessors.ContainsKey(tagInfo.prefix))
//        {
//            return tagPreprocessors[tagInfo.prefix].Preprocess(tagInfo, textIndex);
//        }

//        return false;
//    }
//}
