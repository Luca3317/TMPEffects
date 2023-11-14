using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEditor.ShaderKeywordFilter;
using UnityEditor.UIElements;
using UnityEngine;

public static class ParsingUtility
{

    // TODO Parsing utility should not care about the actual meaning of the prefixes => only include open close here
    [Flags]
    public enum TagType : int
    {
        Open = 1,
        Close = 2
    }

    private static readonly HashSet<char> validPrefixes = new HashSet<char>()
    {
        '!', '#'
    };

    public class TagInfo
    {
        public const char NO_PREFIX = '\0';

        public TagType type;

        public char prefix;
        public string name;

        public string parameterString;

        public int startIndex;
        public int endIndex;

        public TagInfo(int startIndex, int endIndex, TagType type, char prefix, string name, string parameterString)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;

            this.type = type;
            this.prefix = prefix;
            this.name = name;
            this.parameterString = parameterString;
        }

        //public override string ToString()
        //{
        //    string s = "<" + (prefix == NO_PREFIX ? "" : prefix.ToString()) + name;
        //    foreach (var kvp in  parameters)
        //    {
        //        s += " " + kvp.Key + "=" + kvp.Value;
        //    }
        //    return s + ">";
        //}
    }

    public static bool GetNextTag(string text, int startIndex, ref TagInfo tag, TagType type = TagType.Open | TagType.Close)
    {
        int index = startIndex - 1;
        int len = text.Length;

        int tagStartIndex, tagEndIndex;

        if (index >= len - 3) return false;

        do
        {
            index = index + 1;
            tagStartIndex = text.IndexOf('<', index);

            // If there is no open bracket found or it is at the end of the text, there is no valid tag
            if (tagStartIndex == -1 || tagStartIndex == len - 1) return false;

            tagEndIndex = text.IndexOf('>', tagStartIndex + 1);
            if (tagEndIndex == -1) return false;

            //// If no further open bracket found, or
            //// if index is so large that there isnt enough space for <>, there is no valid tag
            //if (index >= len - 3 || tagEndIndex == -1) return false;

        } while (!TryParseTag(text, tagStartIndex, tagEndIndex, ref tag, type));

        return true;
    }

    public static bool IsWellFormed(string text, int start, int end, TagType type = TagType.Open | TagType.Close)
    {
        if (text[start] != '<') return false;
        if (text[end] != '>') return false;

        if (end <= start + 1) return false;
        if ((type & GetTagType(text, start + 1)) == 0) return false;

        int nextOpenIndex = text.IndexOf('<', start + 1);
        if (nextOpenIndex != -1 && end > nextOpenIndex) return false;

        return true;
    }

    public static bool TryParseTag(string text, int startIndex, int endIndex, ref TagInfo tag, TagType type = TagType.Open | TagType.Close)
    {
        if (!IsWellFormed(text, startIndex, endIndex, type)) return false;

        char prefix = TagInfo.NO_PREFIX;
        string name = "";

        // Check the tags type; if type doesnt match return false
        TagType actualType = GetTagType(text, startIndex);
        if ((type & actualType) == 0) return false;

        // Need to cache startIndex
        int index = startIndex + 1;

        // If closing tag, advance index by 1 for the '/'
        if (actualType == TagType.Close) index++;

        // If has prefix, advance index by 1
        if (HasTagPrefix(text, index)) prefix = text[index++];

        // Index now at first character of name

        // Try parsing the name; if it fails, return false
        if (!TryParseTagName(text, index, ref name) || string.IsNullOrWhiteSpace(name)) return false;

        string parameterString = text.Substring(index, endIndex - index);

        tag = new TagInfo(startIndex, endIndex, actualType, prefix, name, parameterString);
        return true;
    }

    static bool HasTagPrefix(string text, int index)
    {
        if (validPrefixes.Contains(text[index])) return true;
        return false;
    }

    static TagType GetTagType(string text, int start)
    {
        if (text[start + 1] == '/') return TagType.Close;
        return TagType.Open;
    }

    static bool TryParseTagName(string text, int startIndex, ref string name)
    {
        int endIndex;
        int len = text.Length;

        char c = '\0';
        for (endIndex = startIndex; endIndex < len; endIndex++)
        {
            c = text[endIndex];

            if (char.IsWhiteSpace(c) || c == '=' || c == '>')
                break;
        }

        if (endIndex >= len) return false;
        //if (char.IsWhiteSpace(c) || c == '=' || c == '>')
        //    return false;

        name = text.Substring(startIndex, endIndex - startIndex);
        return true;
    }

    // TODO optimize a little
    public static Dictionary<string, string> GetTagParametersDict(string text, int startIndex)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();

        text = text.Substring(startIndex, text.Length - startIndex);

        string key, value;
        int endValue;

        // Parse the tag's value
        ParseKeyValue(text, out key, out value, out endValue);
        dict.Add("", value);
        text = text.Remove(0, Mathf.Min(endValue, text.Length)).Trim();

        // TODO only for debugging purposes, prevent endless loop
        int count = 0;

        // Parse attribute keys and values
        while (text.Length > 0)
        {
            ParseKeyValue(text, out key, out value, out endValue);
            //Debug.Log("Key and value: " + key + ", " + value);

            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }

            text = text.Remove(0, Mathf.Min(endValue, text.Length)).Trim();

            ////Debug.Log("Remainging tag: " + tag);
            count++;
            if (count > 5) break;
        }

        if (dict.Count == 0) return null;
        return dict;
    }

    static void ParseKeyValue(string text, out string key, out string value, out int endValue)
    {
        int index = 0;
        int len = text.Length;
        char c;

        bool searchValue = false;
        for (; index < len; index++)
        {
            c = text[index];
            if (c == '=')
            {
                searchValue = true;
                break;
            }
            if (char.IsWhiteSpace(c)) break;
            if (c == '>') break;
        }

        key = text.Substring(0, index);
        endValue = index;
        value = "";
        index++;

        if (!searchValue || index == 0 || index == len) return;


        bool quoted = false;
        if (text[index] == '"')
        {
            index++;
            quoted = true;
        }

        int startIndex = index;

        for (; index < len; index++)
        {
            c = text[index];
            if (c == '"' || c == '>' || (!quoted && c == ' '))
            {
                break;
            }
        }

        endValue = index + 1;
        value = text.Substring(startIndex, index - startIndex);
    }
}
