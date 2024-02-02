using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TMPEffects.TextProcessing
{
    public static class ParsingUtility
    {
        [Flags]
        public enum TagType : int
        {
            Open = 1,
            Close = 2
        }

        public const char NO_PREFIX = '\0';
        public const char ANIMATION_PREFIX = NO_PREFIX;
        public const char SHOW_ANIMATION_PREFIX = '+';
        public const char HIDE_ANIMATION_PREFIX = '-';
        public const char COMMAND_PREFIX = '!';
        public const char EVENT_PREFIX = '#';

        private static readonly HashSet<char> validPrefixes = new HashSet<char>()
        {
            '!', '#', '+', '-'
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
            //int count = 0;

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

                //count++;
                //if (count > 5) break;
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

        /// <summary>
        /// Checks if <paramref name="text"/> contains a substring starting at <paramref name="startIndex"/>, going up to <paramref name="maxIndex"/>, that is a well formed tag.
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
            if (closeIndex <= index + 1)
            {
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
            return IsTag(tag);
        }


        public static bool GetNextTag(string text, int startIndex, out string tag, TagType type = TagType.Open | TagType.Close)
        {
            tag = "";
            int start, end;
            if (!GetNextTagIndeces(text, startIndex, out start, out end, type)) return false;

            tag = text.Substring(start, end - start + 1);
            return true;
        }

        public static bool GetNextTag(string text, string tagName, int startIndex, out string tag, TagType type = TagType.Open | TagType.Close)
        {
            tag = "";
            int start, end;
            if (!GetNextTagIndeces(text, tagName, startIndex, out start, out end, type)) return false;

            tag = text.Substring(start, end - start + 1);
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
                if (tagStartIndex == -1 || tagStartIndex == len - 1)
                {
                    return false;
                }

                tagEndIndex = text.IndexOf('>', tagStartIndex + 1);

                // If no further open bracket found, or
                // if index is so large that there isnt enough space for <>, there is no valid tag
                if (index >= len - 3 || tagEndIndex == -1)
                {
                    return false;
                }

            } while (!IsTag(text, tagStartIndex, tagEndIndex, type));

            //Debug.Log("returning indeces " + tagStartIndex + " and " + tagEndIndex);
            return true;
        }

        public static bool GetNextTagIndeces(string text, string tagName, int startIndex, out int tagStartIndex, out int tagEndIndex, TagType type = TagType.Open | TagType.Close)
        {
            string search = "<" + tagName;
            int index = startIndex;
            do
            {
                tagStartIndex = text.IndexOf(search, index);
                tagEndIndex = text.IndexOf('>', tagStartIndex + 1);
                index = index + 1;

                if (tagStartIndex == -1 || tagEndIndex == -1) return false;

            } while (!IsTag(text, tagStartIndex, tagEndIndex, type));

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

        public static Dictionary<string, string> GetTagParametersDict(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) throw new System.ArgumentException(nameof(tag));
            tag = tag.Trim();
            if (tag[0] == '<')
            {
                if (!IsTag(tag, TagType.Open)) throw new System.ArgumentException(nameof(tag));
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
                //Debug.Log("Key and value: " + key + ", " + value); 

                if (!dict.ContainsKey(key))
                    dict.Add(key, value);

                tag = tag.Remove(0, Mathf.Min(endValue, tag.Length)).Trim();

            }
            return dict;
        }

        internal static TagType GetType(string tag)
        {
            if (tag[1] == '/') return TagType.Close;
            return TagType.Open;
        }



        //public static bool TryParseAnimationTag(int textIndex, TagInfo tagInfo, TMPAnimationDatabase database, ref TMPAnimationTag tag)
        //{
        //    if (database == null) return false;

        //    // check name
        //    if (!database.Contains(tagInfo.name)) return false;

        //    ITMPAnimation animation;
        //    if ((animation = database.GetEffect(tagInfo.name)) == null) return false;

        //    if (tagInfo.type == TagType.Open)
        //    {
        //        // check parameters
        //        var parameters = GetTagParametersDict(tagInfo.parameterString, 0);
        //        if (!animation.ValidateParameters(parameters)) return false;

        //        tag = new TMPAnimationTag(tagInfo.name, textIndex, parameters);
        //        //ProcessedTags.Add(tag);
        //    }
        //    else
        //    {
        //        for (int i = ProcessedTags.Count - 1; i >= 0; i--)
        //        {
        //            tag = ProcessedTags[i];

        //            if (tag.IsOpen && tag.IsEqual(tagInfo.name))
        //            {
        //                tag.Close(textIndex - 1);
        //                break;
        //            }
        //        }
        //    }

        //    return true;
        //}




        public static bool StringToInt(string str, out int result)
        {
            if (!int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                return false;

            return true;
        }

        public static bool StringToFloat(string str, out float result)
        {
            if (!float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                return false;

            return true;
        }

        public static bool StringToBool(string str, out bool result)
        {
            if (!bool.TryParse(str, out result)) return false;
            return true;
        }

        public static bool StringToVector2(string str, out Vector2 result)
        {
            result = new Vector2(0, 0);
            str = str.Trim();
            if (str.Length <= 3) return false;

            if (str[0] != '(') return false;

            if (str[str.Length - 1] != ')') return false;

            int commaIndex = str.IndexOf(';');

            if (commaIndex < 2) return false;

            float x, y;
            if (!StringToFloat(str.Substring(1, commaIndex - 1), out x)) return false;

            if (!StringToFloat(str.Substring(commaIndex + 1, str.Length - (commaIndex + 2)), out y)) return false;

            result.x = x;
            result.y = y;
            return true;
        }

        public static bool StringToVector3(string str, out Vector3 result)
        {
            result = new Vector3(0, 0, 0);
            str = str.Trim();
            if (str.Length <= 5) return false;

            if (str[0] != '(') return false;

            if (str[str.Length - 1] != ')') return false;

            int commaIndex = str.IndexOf(';');
            int commaIndex2 = str.IndexOf(';', commaIndex + 1);
            if (commaIndex < 2) return false;
            if (commaIndex2 < 4) return false;

            float x, y, z;
            if (!StringToFloat(str.Substring(1, commaIndex - 1), out x)) return false;
            if (!StringToFloat(str.Substring(commaIndex + 1, commaIndex2 - (commaIndex + 1)), out y)) return false;
            if (!StringToFloat(str.Substring(commaIndex2 + 1, str.Length - (commaIndex2 + 2)), out z)) return false;

            result.x = x;
            result.y = y;
            result.z = z;
            return true;
        }
    }
}
