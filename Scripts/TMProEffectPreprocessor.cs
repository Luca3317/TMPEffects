using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class TMProEffectPreprocessor : ITextPreprocessor
{
    public readonly ReadOnlyCollection<TMPEffectTag> Tags;
    public readonly ReadOnlyCollection<CharData> CharData;

    private readonly List<TMPEffectTag> tags;
    private readonly List<CharData> charData;

    private readonly TMPEffectDatabase database;

    private StringBuilder sb;

    public TMProEffectPreprocessor(TMPEffectDatabase database)
    {
        sb = new StringBuilder();

        tags = new List<TMPEffectTag>();
        Tags = new ReadOnlyCollection<TMPEffectTag>(tags);

        charData = new List<CharData>();
        CharData = new ReadOnlyCollection<CharData>(charData);

        this.database = database;
    }

    /// <summary>
    /// Store and remove all the custom TMProEffect tags from the given string
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string PreprocessText(string text)
    {
        Debug.Log("Preprocess");

        // TODO
        // Need to optimized this method
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        tags.Clear();

        int index = 0;
        int startIndex, endIndex;
        sb.Clear();

        int lastIndex = 0;
        while (ParsingUtility.GetNextTagIndeces(text, index, out startIndex, out endIndex))
        {
            if (lastIndex != startIndex)
            {
                sb.Append(text.Substring(lastIndex, startIndex - lastIndex));
            }

            string tagString = text.Substring(startIndex, endIndex - startIndex + 1);
            ProcessTag(tagString, startIndex, endIndex, out lastIndex, out index);
        }

        sb.Append(text.Substring(lastIndex, text.Length - lastIndex));

        // Returning an empty string does not update TMP_Text correctly

        foreach (var tag in tags)
        {
            if (tag.IsOpen) tag.Close(sb.Length - 1);
        }

        sw.Stop();
        Debug.Log("Preprocessing took " + sw.Elapsed.TotalMilliseconds);

        if (sb.Length == 0) return " ";
        return sb.ToString();
    }


    private void ProcessTag(string tagString, int startIndex, int endIndex, out int lastIndex, out int index)
    {
        string name = ParsingUtility.GetTagName(tagString);

        if (!database.Contains(name))
        {
            lastIndex = startIndex;
            index = startIndex + 1;
            return;
        }

        TMPEffectTag tag;
        ParsingUtility.TagType type = ParsingUtility.GetType(tagString);

        // If is opening tag
        if (type == ParsingUtility.TagType.Open)
        {
            var paramsDict = ParsingUtility.GetTagParametersDict(tagString);


            // EXPERIMENTAL
            // It would be cool to be able to define formulas to use to set values
            // 



            if (!database.GetEffect(name).ValidateParameters(paramsDict))
            {
                lastIndex = startIndex;
                index = startIndex + 1;
                return;
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

        lastIndex = endIndex + 1;
        index = endIndex + 1;
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Text;
//using System.Text.RegularExpressions;
//using TMPro;
//using UnityEngine;

//public class TMProEffectPreprocessor : ITextPreprocessor
//{
//    public readonly ReadOnlyCollection<TMPEffectTag> Tags;
//    public readonly ReadOnlyCollection<CharData> CharData;

//    private readonly List<TMPEffectTag> tags;
//    private readonly List<CharData> charData;

//    private readonly TMPEffectDatabase database;

//    private StringBuilder sb;

//    public TMProEffectPreprocessor(TMPEffectDatabase database)
//    {
//        sb = new StringBuilder();

//        tags = new List<TMPEffectTag>();
//        Tags = new ReadOnlyCollection<TMPEffectTag>(tags);

//        charData = new List<CharData>();
//        CharData = new ReadOnlyCollection<CharData>(charData);

//        this.database = database;
//    }

//    /// <summary>
//    /// Store and remove all the custom TMProEffect tags from the given string
//    /// </summary>
//    /// <param name="text"></param>
//    /// <returns></returns>
//    public string PreprocessText(string text)
//    {
//        Debug.Log("Preprocess");

//        // TODO
//        // Need to optimized this method
//        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
//        sw.Start();

//        tags.Clear();

//        int index = 0;
//        int startIndex, endIndex;
//        sb.Clear();

//        int lastIndex = 0;
//        while (ParsingUtility.GetNextTagIndeces(text, index, out startIndex, out endIndex))
//        {
//            if (lastIndex != startIndex)
//            {
//                sb.Append(text.Substring(lastIndex, startIndex - lastIndex));
//            }

//            string tagString = text.Substring(startIndex, endIndex - startIndex + 1);
//            ProcessTag(tagString, startIndex, endIndex, out lastIndex, out index);
//        }

//        sb.Append(text.Substring(lastIndex, text.Length - lastIndex));

//        // Returning an empty string does not update TMP_Text correctly

//        foreach (var tag in tags)
//        {
//            if (tag.IsOpen) tag.Close(sb.Length - 1);
//        }

//        sw.Stop();
//        Debug.Log("Preprocessing took " + sw.Elapsed.TotalMilliseconds);

//        if (sb.Length == 0) return " ";
//        return sb.ToString();
//    }


//    private void ProcessTag(string tagString, int startIndex, int endIndex, out int lastIndex, out int index)
//    {
//        string name = ParsingUtility.GetTagName(tagString);

//        if (!database.Contains(name))
//        {
//            lastIndex = startIndex;
//            index = startIndex + 1;
//            return;
//        }

//        TMPEffectTag tag;
//        ParsingUtility.TagType type = ParsingUtility.GetType(tagString);

//        // If is opening tag
//        if (type == ParsingUtility.TagType.Open)
//        {
//            var paramsDict = ParsingUtility.GetTagParametersDict(tagString);


//            // EXPERIMENTAL
//            // It would be cool to be able to define formulas to use to set values
//            // 



//            if (!database.GetEffect(name).ValidateParameters(paramsDict))
//            {
//                lastIndex = startIndex;
//                index = startIndex + 1;
//                return;
//            }

//            tag = new TMPEffectTag(name, sb.Length, paramsDict);
//            tags.Add(tag);
//        }
//        else
//        {
//            for (int i = tags.Count - 1; i >= 0; i--)
//            {
//                tag = tags[i];

//                if (tag.IsEqual(name))
//                {
//                    if (tag.IsOpen) tag.Close(sb.Length - 1);
//                    break;
//                }
//            }
//        }

//        lastIndex = endIndex + 1;
//        index = endIndex + 1;
//    }
//}