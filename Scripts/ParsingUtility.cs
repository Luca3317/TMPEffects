using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public static class ParsingUtility
{
    const string TAG_NAME_REGEX = @"^[^>\s=]+";
    static readonly Regex tagNameRegex = new Regex(TAG_NAME_REGEX);

    [Flags]
    public enum TagType : short
    {
        Open = 1,
        Close = 2
    }

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
            if (tagStartIndex == -1 || tagStartIndex == len - 1) return false;

            tagEndIndex = text.IndexOf('>', tagStartIndex + 1);

            // If no further open bracket found, or
            // if index is so large that there isnt enough space for <>, there is no valid tag
            if (index >= len - 3 || tagEndIndex == -1) return false;

        } while (!IsTag(text, tagStartIndex, tagEndIndex, type));

        //Debug.Log("returning indeces " + tagStartIndex + " and " + tagEndIndex);
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
