using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPEffects.Extensions;
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
            if (closeIndex <= index + 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the given string is a well formed tag (of the given type, if supplied).
        /// </summary>
        /// <param name="tag">The string to check.</param>
        /// <param name="type">The type of tag to check for. Leave default for either type.</param>
        /// <returns>true if the the given string is a tag (of the given type, if supplied); otherwise false.</returns>
        public static bool IsTag(string tag, TagType type = TagType.Open | TagType.Close)
        {
            if (tag.IndexOf('>') != tag.Length - 1) return false;
            return IsTag(tag, 0, tag.Length, type);
        }

        /// <summary>
        /// Parses a string to a <see cref="Dictionary{string, string}"/>.<br/>
        /// Pass in either a full tag string, e.g. <example=10 arg="value">, or the same but
        /// without the brackets (example=10 arg="value").<br/>
        /// Note that this only does some basic checks on the validity of the input string;
        /// malformed strings may lead to errors.<br/>
        /// Ideally use by passing in a <see cref="TagInfo.parameterString"/>.
        /// </summary>
        /// <param name="tag">The tag to parse the parameters of.</param>
        /// <returns>The parsed string as <see cref="Dictionary{string, string}"/>.</returns>
        /// <exception cref="System.ArgumentException"></exception>
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
            return !char.IsLetter(text[index]);
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

        #region Parsing
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

            int commaIndex = str.IndexOf(',');

            if (commaIndex < 2) return false;

            float x, y;
            if (!StringToFloat(str.Substring(1, commaIndex - 1), out x)) return false;

            if (!StringToFloat(str.Substring(commaIndex + 1, str.Length - (commaIndex + 2)), out y)) return false;

            result.x = x;
            result.y = y;
            return true;
        }

        private static bool SpanToVector2(ReadOnlySpan<char> span, out Vector2 result)
        {
            result = new Vector2(0, 0);
            span = span.Trim();
            if (span.Length <= 3) return false;

            if (span[0] != '(') return false;

            if (span[span.Length - 1] != ')') return false;

            int commaIndex = span.IndexOf(',');

            if (commaIndex < 2) return false;

            float x, y;
            if (!SpanToFloat(span.Slice(1, commaIndex - 1), out x)) return false;

            if (!SpanToFloat(span.Slice(commaIndex + 1, span.Length - (commaIndex + 2)), out y)) return false;

            result.x = x;
            result.y = y;
            return true;
        }

        private static bool SpanToFloat(ReadOnlySpan<char> span, out float result)
        {
            if (!float.TryParse(span, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                return false;

            return true;
        }

        public static bool StringToVector3(string str, out Vector3 result)
        {
            result = new Vector3(0, 0, 0);
            str = str.Trim();
            if (str.Length <= 5) return false;

            if (str[0] != '(') return false;

            if (str[str.Length - 1] != ')') return false;

            int commaIndex = str.IndexOf(',');
            int commaIndex2 = str.IndexOf(',', commaIndex + 1);
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

        /*
         * Valid anim curve format
         * 
         *  cubic(x0,y0,x1,y1....); same with quadratic/linear; alt names e.g. cubic-bezier
         *  
         *  cubic((x0,y0),(x1,y1)); same as above but with vectors
         *  
         *  
         *  pure vector sequence: (x0,y0),(x1,y1)...; infer type based on amount of points 
         *  
         *  //recognizable by: no '(' ')' or ','
         *  keywords: easein, easeout, etc
         */
        public static bool StringToAnimationCurve(string str, out AnimationCurve result)
        {
            result = null;
            str = str.Trim();

            if (str.Length == 0) return false;

            // If vector
            if (str[0] == '(')
            {
                return VectorSequenceToAnimationCurve(str, ref result);
            }

            // If method
            if (str.Contains('('))
            {
                return MethodToAnimationCurve(str, ref result);
            }

            // else, keyword
            if (AnimationCurveUtility.NameConstructorMapping.TryGetValue(str, out Func<AnimationCurve> ctor))
            {
                result = ctor();
                return true;
            }

            return false;
        }

        private static bool VectorSequenceToAnimationCurve(string str, ref AnimationCurve result)
        {
            List<Vector2> vectors = new List<Vector2>();
            int currentStartIndex = str.IndexOf('(', 0);
            int currentEndIndex = str.IndexOf(')', currentStartIndex);

            if (currentStartIndex == -1 || currentEndIndex == -1) return false;

            var span = str.AsSpan();

            while (currentEndIndex < str.Length && currentEndIndex != -1)
            {
                var slice = span.Slice(currentStartIndex, currentEndIndex + 1 - currentStartIndex);

                if (!SpanToVector2(slice, out Vector2 vectorResult))
                {
                    return false;
                }

                vectors.Add(vectorResult);
                currentStartIndex = str.IndexOf('(', currentEndIndex);
                currentEndIndex = currentStartIndex != -1 ? str.IndexOf(')', currentStartIndex) : -1;
            }

            result = AnimationCurveUtility.Bezier(vectors);
            return true;
        }

        private static bool MethodToAnimationCurve(string str, ref AnimationCurve result)
        {
            if (str.Length < 4) return false;

            List<Vector2> vectors = new List<Vector2>();
            int currentStartIndex = str.IndexOf("((", 0);
            int currentEndIndex = str.IndexOf(')', currentStartIndex);

            if (currentStartIndex == -1 || currentEndIndex == -1) return false;

            Func<IEnumerable<Vector2>, AnimationCurve> constructor = AnimationCurveUtility.NameBezierConstructorMapping[str.Substring(0, currentStartIndex)];

            Debug.Log("MEthodname: " + str.Substring(0, currentStartIndex));

            currentStartIndex++;
            var span = str.AsSpan();
            if (span[span.Length - 1] != ')' || span[span.Length - 2] != ')')
                return false;

            while (currentEndIndex < str.Length && currentEndIndex != -1)
            {
                var slice = span.Slice(currentStartIndex, currentEndIndex + 1 - currentStartIndex);

                UnityEngine.Debug.Log("Trying " + slice.ToString());
                if (!SpanToVector2(slice, out Vector2 vectorResult))
                {
                    return false;
                }

                vectors.Add(vectorResult);
                currentStartIndex = str.IndexOf('(', currentEndIndex);
                currentEndIndex = currentStartIndex != -1 ? str.IndexOf(')', currentStartIndex) : -1;
            }

            result = constructor(vectors);
            return true;
        }
        #endregion
    }
}
