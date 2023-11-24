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

        public TagInfo() { }

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

    public static System.Diagnostics.Stopwatch Getnexttagdtopwatch = new System.Diagnostics.Stopwatch();
    public static System.Diagnostics.Stopwatch oldw = new System.Diagnostics.Stopwatch();

    public static bool GetNextTag(string text, int startIndex, ref TagInfo tag, TagType type = TagType.Open | TagType.Close)
    {
        Getnexttagdtopwatch.Start();
        int index = startIndex - 1;
        int len = text.Length;

        int tagStartIndex, tagEndIndex;

        if (index >= len - 3) return false;

        do
        {
            index = index + 1;
            tagStartIndex = text.IndexOf('<', index);

            // If there is no open bracket found or it is at the end of the text, there is no valid tag
            if (tagStartIndex == -1 || tagStartIndex == len - 1)
            {
                Getnexttagdtopwatch.Stop();
                return false;
            }

            tagEndIndex = text.IndexOf('>', tagStartIndex + 1);
            if (tagEndIndex == -1)
            {
                Getnexttagdtopwatch.Stop();
                return false;
            }

            //// If no further open bracket found, or
            //// if index is so large that there isnt enough space for <>, there is no valid tag
            //if (index >= len - 3 || tagEndIndex == -1) return false;

        } while (!TryParseTag(text, tagStartIndex, tagEndIndex, ref tag, type));

        Getnexttagdtopwatch.Stop();
        return true;
    }

    public static bool IsWellFormed(string text, int start, int end, TagType type = TagType.Open | TagType.Close)
    {
        if (start < 0 || end <= 0) return false;

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

        char prefix = NO_PREFIX;
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

        //tag = new TagInfo(startIndex, endIndex, actualType, prefix, name, parameterString);
        tag.startIndex = startIndex;
        tag.endIndex = endIndex;
        tag.type = actualType;
        tag.prefix = prefix;
        tag.name = name;
        tag.parameterString = parameterString;
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
        if (!string.IsNullOrEmpty(value))
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




    const string TAG_NAME_REGEX = @"^[^>\s=]+";
    static readonly Regex tagNameRegex = new Regex(TAG_NAME_REGEX);


    const string VALID_NAME_CHARS = "!#$&*-0123456789:;?@ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz~";
    const string END_NAME_CHARS = ">= ";
    static readonly List<char> validNameChars = VALID_NAME_CHARS.ToCharArray().ToList();
    static readonly List<char> endNameChars = END_NAME_CHARS.ToCharArray().ToList();

    /// <summary>
    /// Checks if <paramref name="text"/> contains a substring starting at <paramref name="startIndex"/>, going up to <paramref name="maxIndex"/>, that is a well formed tag.
    /// TODO For now this checks that there is a '<' at the beginning, followed by a '>' with no other '<' inbetween.
    /// Maybe should check for hasname and has attribute or some shit too?
    /// </summary>
    /// <param name="text"></param>
    /// <param name="startIndex"></param>
    /// <param name="maxIndex"></param>
    /// <returns></returns>
    public static bool IsTag(string text, int startIndex, int maxIndex = -1, TagType type = TagType.Open | TagType.Close)
    {
        int length = text.Length;
        if (maxIndex == -1) maxIndex = text.Length - 1;
        if (startIndex >= maxIndex)
        {
            Debug.LogError("Passed in invalid indeces; startIndex is larger than maxIndex (startIndex: " + startIndex + "; maxIndex: " + maxIndex + ")");
            throw new System.ArgumentException();
        }

        int index = startIndex;
        if (text[index] != '<')
        {
            Debug.Log("First item was not opening bracket");
            return false;
        }

        int nextOpenIndex = text.IndexOf('<', index + 1);
        int closeIndex = text.IndexOf('>', index + 1);

        if (type.HasFlag(TagType.Close) && !type.HasFlag(TagType.Open))
        {
            if (text[index + 1] != '/') return false;
        }
        else if (!type.HasFlag(TagType.Close) && type.HasFlag(TagType.Open))
        {
            if (text[index + 1] == '/') return false;
        }

        if (nextOpenIndex != -1 && closeIndex > nextOpenIndex)
        {
            //Debug.Log("Another opne bracket before close bracket; " + nextOpenIndex);
            return false;
        }
        if (closeIndex == -1)
        {
            //Debug.Log("No close bracket");
            return false;
        }
        if (closeIndex <= index + 1)
        {
            //Debug.Log("Close bracket too close");
            return false;
        }

        return true;
    }

    public static bool IsTag(string tag, TagType type = TagType.Open | TagType.Close)
    {
        if (tag.IndexOf('>') != tag.Length - 1) return false;
        return IsTag(tag, 0, tag.Length, type);
    }


    public static bool IsValidTag(string tag, TagType type = TagType.Open | TagType.Close)
    {
        return IsTag(tag); // TODO && NameValid && ParamsValid
    }


    public static bool GetNextTag(string text, int startIndex, out string tag, TagType type = TagType.Open | TagType.Close)
    {
        tag = "";
        int start, end;
        if (!GetNextTagIndeces(text, startIndex, out start, out end, type)) return false;

        //Debug.Log("got " + start + ", " + end);
        tag = text.Substring(start, end - start + 1);
        //Debug.Log("TAG " + tag);
        return true;
    }

    public static bool GetNextTag(string text, string tagName, int startIndex, out string tag, TagType type = TagType.Open | TagType.Close)
    {
        tag = "";
        int start, end;
        if (!GetNextTagIndeces(text, tagName, startIndex, out start, out end, type)) return false;

        //Debug.Log("got " + start + ", " + end);
        tag = text.Substring(start, end - start + 1);
        //Debug.Log("TAG " + tag);
        return true;
    }

    public static bool GetNextTagIndeces(string text, int startIndex, out int tagStartIndex, out int tagEndIndex, TagType type = TagType.Open | TagType.Close)
    {
        oldw.Start();
        tagStartIndex = -1;
        tagEndIndex = -1;
        int index = startIndex - 1;
        int len = text.Length;

        if (index >= len - 3) return false;

        do
        {
            index = index + 1;
            tagStartIndex = text.IndexOf('<', index);

            // If there is no open bracket found or it is at the end of the text, there is no valid tag
            if (tagStartIndex == -1 || tagStartIndex == len - 1)
            {
                oldw.Stop();
                return false;
            }

            tagEndIndex = text.IndexOf('>', tagStartIndex + 1);

            // If no further open bracket found, or
            // if index is so large that there isnt enough space for <>, there is no valid tag
            if (index >= len - 3 || tagEndIndex == -1)
            {
                oldw.Stop();
                return false;
            }

        } while (!IsTag(text, tagStartIndex, tagEndIndex, type));

        //Debug.Log("returning indeces " + tagStartIndex + " and " + tagEndIndex);
        oldw.Stop();
        return true;
    }

    public static bool GetNextTagIndeces(string text, string tagName, int startIndex, out int tagStartIndex, out int tagEndIndex, TagType type = TagType.Open | TagType.Close)
    {
        //int index = startIndex - 1;
        string search = "<" + tagName;
        int index = startIndex;
        do
        {
            tagStartIndex = text.IndexOf(search, index);
            tagEndIndex = text.IndexOf('>', tagStartIndex + 1);
            index = index + 1;

            if (tagStartIndex == -1 || tagEndIndex == -1) return false;

        } while (!IsTag(text, tagStartIndex, tagEndIndex, type));

        //Debug.Log("returning indeces " + tagStartIndex + " and " + tagEndIndex);
        return true;
    }

    const string NEW_NAME_REGEX = @"(?<=^<)[^>=\s]+";
    static readonly Regex newNameRegex = new Regex(NEW_NAME_REGEX);

    public static char GetTagPrefix(string tag)
    {
        int index = 1;
        if (GetType(tag) == TagType.Close)
        {
            index++;
        }

        if (validPrefixes.Contains(tag[index])) return tag[index];
        return NO_PREFIX;
    }

    public const char NO_PREFIX = '\0';

    /// <summary>
    /// Get a tag's name.
    /// Presumes you pass in a valid tag, and that startIndex is the position
    /// of the opening bracket.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="startIndex"></param>
    /// <param name="maxIndex"></param>
    /// <returns></returns>
    public static string GetTagName(string tag)
    {
        if (!IsTag(tag)) throw new System.ArgumentException(tag);

        int len = tag.Length;
        int start = 1;
        if (GetType(tag) == TagType.Close)
        {
            start++;
        }

        if (validPrefixes.Contains(tag[start]))
        {
            start++;
        }

        char c;
        int i;
        for (i = start; i < len; i++)
        {
            c = tag[i];

            if (char.IsWhiteSpace(c) || c == '=' || c == '>')
                break;
        }

        //Match m = newNameRegex.Match(tag);
        //return m.Value;

        return tag.Substring(start, i - start);
    }

    public static string GetTagParameters(string tag)
    {
        if (!IsTag(tag, TagType.Open)) throw new System.ArgumentException(nameof(tag));
        string replaced = newNameRegex.Replace(tag, "");
        return replaced.Substring(1, replaced.Length - 2).Trim();
    }


    /*
     * TODO
     * Need some way to let you get the tag value and attribute keyvalue pairs
     * For now simple string dictionary; this method will likely have to be used a lot
     * (either: every frame for each tag or every time text is changed, for each tag)
     * => more efficient solution?
     * 
     * Reusing the same dictionary
     * Maybe richtexttagattribute array?
     */
    public static Dictionary<string, string> GetTagParametersDict(string tag)
    {
        if (!IsTag(tag, TagType.Open)) throw new System.ArgumentException(nameof(tag));
        Dictionary<string, string> dict = new Dictionary<string, string>();

        tag = tag.Substring(1, tag.Length - 2);

        string key, value;
        int endValue;

        // Parse the tag's value
        ParseKeyValue(tag, out key, out value, out endValue);
        dict.Add("", value);
        tag = tag.Remove(0, Mathf.Min(endValue, tag.Length)).Trim();

        // TODO only for debugging purposes, prevent endless loop
        int count = 0;
        // Parse attribute keys and values
        while (tag.Length > 0)
        {
            ParseKeyValue(tag, out key, out value, out endValue);
            //Debug.Log("Key and value: " + key + ", " + value);

            if (!dict.ContainsKey(key))
                dict.Add(key, value);

            tag = tag.Remove(0, Mathf.Min(endValue, tag.Length)).Trim();

            //Debug.Log("Remainging tag: " + tag);
            count++;
            if (count > 5) break;
        }

        return dict;
    }

    internal static TagType GetType(string tag)
    {
        if (tag[1] == '/') return TagType.Close;
        return TagType.Open;
    }

    /*
     * TODO 
     * For now, closing brackets in quotes are still treated like closing brackets; this prevents you from using them in any string attribute values
     *      => TMPro seems to do the same
     * 
     * TODO 
     * Later on, make some stuff internal and remove then superfluous istag checks
     * 
     * TODO ClosingTag versions?
     * Better solution: give each method an optional parameter of TagType; include depending on that
     * 
     * bool IsTag(string tag) <= check if passed in string is a well formed tag
     * bool IsTag(string text, int startIndex, int maxIndex = -1) <= check if passed in string contains well formed tag
     * TODO Maybe not this one; basically the same as GetNextTag?
     * bool IsTag(string text, int startIndex, out int endIndex, int maxIndex = -1) <= check if passed in string contains tag, and return the relevant indeces
     * 
     * bool IsValidTag(string tag) <= checks if the passed in tag is genuinely valid; that is, is it well formed as a tag in general (IsTag) and is its tag registered and its parameters valid
     * 
     * TODO should these return bool and rest with outs? Id say yes, otherwise need tuple for indeces version
     * bool GetNextTag(string text, int startIndex, out string tag) <= returns the next tag contained in a string
     * bool GetNextTagIndeces(string text, int startIndex, out int tagStartIndex, out int tagEndIndex) <= returns the next tags indeces contained in the string
     * 
     * bool GetNextClosingTag(string text, int startIndex, out string closingTag) <= returns next closing tag contained in string
     * bool GetNextClosingTagIndeces(string text, int startIndex, out int tagStartIndex, out int tagEndIndex) <= returns the next closing tag contained in a string
     * bool GetClosingTagIndeces(string text, int startIndex, string tagName, out int tagStartIndex, out int tagEndIndex) <= returns the indeces of the next closing tag with the given name
     * 
     * TODO maybe have some tag data structure? 
     * string GetTagName(string tag) <= get a tags name; throw if tag is not well formed
     * string GetTagParameters(string tag) <= get a tags parameters (value and attributes); throw if tag is not well formed
     * string GetTagValue(string tag) <= get a tags value; throw if not well formed TODO actually maybe dont throw; make responsibility of caller that tag is well formed
     * string GetTagAttributes(string tag) <= get a tags attributes; throw if not well formed
     * Dictionary<string, string> GetTagAttributesDict(string tag) <= get a tags attributes as key/value pairs; throw if not well formed
     * 
     */
}
