using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace TMPEffects.TextProcessing
{
    /// <summary>
    /// Utility methods for text processing and parsing.
    /// </summary>
    public static class ParsingUtility
    {
        /// <summary>
        /// Tag type enum.<br/>
        /// Either open, close, or both.
        /// </summary>
        [Flags]
        public enum TagType : int
        {
            Open = 1,
            Close = 2
        }

        /// <summary>
        /// Utility class that holds data about a parsed tag.
        /// </summary>
        public class TagInfo
        {
            /// <summary>
            /// Type of the tag (open, close).
            /// </summary>
            public TagType type;

            /// <summary>
            /// Prefix of the tag.
            /// </summary>
            public char prefix;
            /// <summary>
            /// Name of the tag.
            /// </summary>
            public string name;

            /// <summary>
            /// The parameters of the tag, unparsed.
            /// </summary>
            public string parameterString;

            /// <summary>
            /// Start index of the tag in the source text.
            /// </summary>
            public int startIndex;
            /// <summary>
            /// End index of the tag in the source text.
            /// </summary>
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
        }

        /// <summary>
        /// Get the next tag in <paramref name="text"/>, starting from <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="text">The full string.</param>
        /// <param name="startIndex">The index to start the search from.</param>
        /// <param name="tag">Reference to a <see cref="TagInfo"/>. Will be set to the parsed tag if one is found.</param>
        /// <param name="type">The type of tag to get. Leave at default for either type.</param>
        /// <returns>true if a next tag was found; false otherwise.</returns>
        public static bool GetNextTag(string text, int startIndex, ref TagInfo tag, TagType type = TagType.Open | TagType.Close)
        {
            int index = startIndex - 1;
            int len = text.Length;

            int tagStartIndex, tagEndIndex;

            if (index >= len - 3)
            {
                return false;
            }

            do
            {
                index = index + 1;
                tagStartIndex = text.IndexOf('<', index);

                // If there is no open bracket found or it is at the end of the text, there is no valid tag
                if (tagStartIndex == -1 || tagStartIndex == len - 1)
                {
                    return false;
                }

                tagEndIndex = text.IndexOf('>', tagStartIndex + 1);
                if (tagEndIndex == -1)
                {
                    return false;
                }

                //// If no further open bracket found, or
                //// if index is so large that there isnt enough space for <>, there is no valid tag
                //if (index >= len - 3 || tagEndIndex == -1) return false;

            } while (!TryParseTag(text, tagStartIndex, tagEndIndex, ref tag, type));

            return true;
        }

        /// <summary>
        /// Attempt to parse a tag from the substring of <paramref name="text"/> given by <paramref name="startIndex"/> and <paramref name="endIndex"/>.
        /// </summary>
        /// <param name="text">The full string.</param>
        /// <param name="startIndex">Start index of the substring.</param>
        /// <param name="endIndex">End index of the substring.</param>
        /// <param name="tag">Reference to a <see cref="TagInfo"/>. Will be set to the parsed tag if successful.</param>
        /// <param name="type">The type of tag to check for. Leave at default for either type.</param>
        /// <returns>true if a tag was successfully parsed from the substring; false otherwise.</returns>
        public static bool TryParseTag(string text, int startIndex, int endIndex, ref TagInfo tag, TagType type = TagType.Open | TagType.Close)
        {
            if (!IsWellFormed(text, startIndex, endIndex, type)) return false;

            char prefix = '\0';
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
            if (!TryParseTagName(text, index, ref name) || (actualType == TagType.Open && string.IsNullOrWhiteSpace(name))) return false;

            string parameterString;

            parameterString = text.Substring(index, endIndex - index);

            //tag = new TagInfo(startIndex, endIndex, actualType, prefix, name, parameterString);
            tag.startIndex = startIndex;
            tag.endIndex = endIndex;
            tag.type = actualType;
            tag.prefix = prefix;
            tag.name = name;
            tag.parameterString = parameterString;
            return true;
        }

        /// <summary>
        /// Checks if the given string contains a substring starting at <paramref name="startIndex"/>, going up to <paramref name="maxIndex"/>, that is a well formed tag.
        /// </summary>
        /// <param name="text">The string to check.</param>
        /// <param name="startIndex">The start index of the substring.</param>
        /// <param name="maxIndex">The end index of the substring. Leave at default to set to length of <paramref name="text"/>.</param>
        /// <returns>true if the given string contains a substring starting at <paramref name="startIndex"/>, going up to <paramref name="maxIndex"/>, that is a well formed tag; otherwise false.</returns>
        public static bool IsTag(string text, int startIndex, int maxIndex = -1, TagType type = TagType.Open | TagType.Close)
        {
            int length = text.Length;
            if (maxIndex == -1) maxIndex = text.Length - 1;
            if (startIndex >= maxIndex)
            {
                throw new System.IndexOutOfRangeException();
            }

            int index = startIndex;
            if (text[index] != '<')
            {
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
                return false;
            }
            if (closeIndex == -1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the given string is a well-formed tag (of the given type, if supplied).
        /// </summary>
        /// <param name="tag">The string to check.</param>
        /// <param name="type">The type of tag to check for. Leave default for either type.</param>
        /// <returns>true if the given string is a tag (of the given type, if supplied); otherwise false.</returns>
        public static bool IsTag(string tag, TagType type = TagType.Open | TagType.Close)
        {
            int endindex = tag.LastIndexOf('>');
            if (endindex == -1 || endindex != tag.Length - 1) return false;
            return IsTag(tag, 0, tag.Length, type);
        }

        /// <summary>
        /// Parses a string to a <see cref="Dictionary{TKey, TValue}"/>.<br/>
        /// Pass in either a full tag string, e.g. &lt;example=10 arg="value"&gt;, or the same but
        /// without the brackets (example=10 arg="value").<br/>
        /// Note that this only does some basic checks on the validity of the input string;
        /// malformed strings may lead to errors.<br/>
        /// Ideally use by passing in a <see cref="TagInfo.parameterString"/>.
        /// </summary>
        /// <param name="tag">The tag to parse the parameters of.</param>
        /// <returns>The parsed string as <see cref="Dictionary{TKey, TValue}"/>.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static Dictionary<string, string> GetTagParametersDict(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return new Dictionary<string, string>() { {"", ""} };
            tag = tag.Trim();
            if (tag[0] == '<')
            {
                if (!IsTag(tag/*, TagType.Open*/)) throw new System.ArgumentException(nameof(tag));
                else tag = tag.Substring(1, tag.Length - 2);
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();

            string key, value;
            int endValue;

            // Parse the tag's value
            ParseKeyValue(tag, out key, out value, out endValue);
            dict.Add("", value);
            tag = tag.Remove(0, Mathf.Min(endValue, tag.Length)).Trim();

            // Parse attribute keys and values
            while (tag.Length > 0)
            {
                ParseKeyValue(tag, out key, out value, out endValue);

                if (!dict.ContainsKey(key))
                    dict.Add(key, value);

                tag = tag.Remove(0, Mathf.Min(endValue, tag.Length)).Trim();

            }
            return dict;
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
            index++;
            endValue = index;
            value = "";

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

        static bool HasTagPrefix(string text, int index)
        {
            return !char.IsLetter(text[index]) && text[index] != '>';
        }

        static TagType GetTagType(string text, int start)
        {
            if (text[start + 1] == '/') return TagType.Close;
            return TagType.Open;
        }

        static bool IsWellFormed(string text, int start, int end, TagType type = TagType.Open | TagType.Close)
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
    }
}
