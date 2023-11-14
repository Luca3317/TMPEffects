using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System;

public class TMPEffectPreProcessor : ITextPreprocessor
{
    // TODO list for animations, commands and events
    public List<TMPEffectTag> tags = new List<TMPEffectTag>();
    public List<TMPEventArgs> events = new List<TMPEventArgs>();
    public List<TMPCommand> commands = new List<TMPCommand>();

    TMPEffectsDatabase effectsDatabase;

    public TMPEvent onEvent;

    public TMPEffectPreProcessor(TMPEffectsDatabase effectsDatabase)
    {
        this.effectsDatabase = effectsDatabase;
        onEvent = new TMPEvent();
    }

    public string PreprocessText(string text)
    {
        tags.Clear();

        int searchIndex = 0;
        StringBuilder sb = new StringBuilder();
        ParsingUtility.TagInfo tagInfo = default;

        while (ParsingUtility.GetNextTag(text, searchIndex, ref tagInfo))
        {
            if (searchIndex != tagInfo.startIndex)
                sb.Append(text.AsSpan(searchIndex, tagInfo.startIndex - searchIndex));

            if (!HandleTag(ref tagInfo, sb.Length))
                sb.Append(text.AsSpan(tagInfo.startIndex, tagInfo.endIndex - tagInfo.startIndex + 1));

            searchIndex = tagInfo.endIndex + 1;
        }

        sb.Append(text.AsSpan(searchIndex, text.Length - searchIndex));

        foreach (TMPEffectTag tag in tags)
        {
            if (tag.IsOpen)
            {
                tag.Close(sb.Length - 1);
                Debug.Log("Open tag: " + tag.name + " at " + tag.startIndex);
            }
        }

        if (sb.Length == 0) return " ";
        return sb.ToString();
    }

    // TODO Support various types of tags (Effect/animation, Command, Event)
    bool HandleTag(ref ParsingUtility.TagInfo tagInfo, int textIndex)
    {
        switch (tagInfo.prefix)
        {
            case ParsingUtility.TagInfo.NO_PREFIX:
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
            if (tagInfo.prefix != ParsingUtility.TagInfo.NO_PREFIX)
                return false;

            for (int i = tags.Count - 1; i >= 0; i--)
            {
                tag = tags[i];

                if (tag.IsOpen && tag.IsEqual(tagInfo.name))
                {
                    tag.Close(textIndex - 1);
                    Debug.Log("Closed tag; will effect from " + tag.startIndex + " for " + tag.length + " characters");
                    break;
                }
            }
        }

        return true;
    }

    bool ParseEvent(ref ParsingUtility.TagInfo tagInfo, int textIndex)
    {
        Debug.Log("Parsing event " +  textIndex);
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
