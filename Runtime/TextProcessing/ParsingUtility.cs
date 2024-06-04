using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using TMPEffects.Extensions;
using TMPEffects.TMPAnimations;
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
        /// Checks if the given string is a well formed tag (of the given type, if supplied).
        /// </summary>
        /// <param name="tag">The string to check.</param>
        /// <param name="type">The type of tag to check for. Leave default for either type.</param>
        /// <returns>true if the the given string is a tag (of the given type, if supplied); otherwise false.</returns>
        public static bool IsTag(string tag, TagType type = TagType.Open | TagType.Close)
        {
            int endindex = tag.IndexOf('>');
            if (endindex == -1 || endindex != tag.Length - 1) return false;
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

        #region Parsing
        public static bool StringToInt(string str, out int result, IDictionary<string, int> keywords = null)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(str)) return false;

            if (int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                return true;

            if (keywords != null && keywords.TryGetValue(str, out result))
                return true;

            return true;
        }

        public static bool StringToFloat(string str, out float result, IDictionary<string, float> keywords = null)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(str)) return false;

            if (float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                return true;

            if (keywords != null && keywords.TryGetValue(str, out result))
                return true;

            return false;
        }

        public static bool StringToBool(string str, out bool result, IDictionary<string, bool> keywords = null)
        {
            result = false;
            if (string.IsNullOrWhiteSpace(str)) return false;

            if (bool.TryParse(str, out result))
                return true;

            if (keywords != null && keywords.TryGetValue(str, out result))
                return true;

            return false;
        }




        public static bool StringToVector2(string str, out Vector2 result, IDictionary<string, Vector2> keywords = null)
        {
            result = new Vector2(0, 0);

            Vector2? v;
            if ((v = TryParse()) != null)
            {
                result = v.Value;
                return true;
            }

            if (keywords != null && keywords.TryGetValue(str, out result))
                return true;

            return false;

            Vector2? TryParse()
            {
                str = str.Trim();
                if (str.Length <= 3) return null;

                if (str[0] != '(') return null;

                if (str[str.Length - 1] != ')') return null;

                int commaIndex = str.IndexOf(',');

                if (commaIndex < 2) return null;

                float x, y;
                if (!StringToFloat(str.Substring(1, commaIndex - 1), out x)) return null;

                if (!StringToFloat(str.Substring(commaIndex + 1, str.Length - (commaIndex + 2)), out y)) return null;

                return new Vector2(x, y);
            }
        }

        public static bool StringToVector3(string str, out Vector3 result, IDictionary<string, Vector3> keywords = null)
        {
            result = new Vector3(0, 0, 0);

            Vector3? v;
            if ((v = TryParse()) != null)
            {
                result = v.Value;
                return true;
            }

            if (keywords != null && keywords.TryGetValue(str, out result))
                return true;

            return false;

            Vector3? TryParse()
            {
                str = str.Trim();
                if (str.Length <= 3) return null;

                if (str[0] != '(') return null;

                if (str[str.Length - 1] != ')') return null;


                var split = str.Substring(1, str.Length - 2).Split(',');

                if (split.Length < 2 || split.Length > 3) return null;

                //int commaIndex = str.IndexOf(',');
                //int commaIndex2 = str.IndexOf(',', commaIndex + 1);
                //if (commaIndex < 2) return null;
                //if (commaIndex2 < 4) return null;

                float x, y, z = 0f;
                if (split.Length == 2)
                {
                    if (!StringToFloat(split[0], out x)) return null;
                    if (!StringToFloat(split[1], out y)) return null;
                }
                else
                {
                    if (!StringToFloat(split[0], out x)) return null;
                    if (!StringToFloat(split[1], out y)) return null;
                    if (!StringToFloat(split[2], out z)) return null;
                }

                return new Vector3(x, y, z);
            }
        }

        public static bool StringToVector2Offset(string str, out Vector2 result, IDictionary<string, Vector2> keywords = null)
        {
            str = str.Trim();
            if (str.Length < 3 || str[0] != 'o' || str[1] != ':')
            {
                result = Vector2.zero;
                return false;
            }

            str = str.Substring(2, str.Length - 2);
            if (keywords != null && keywords.TryGetValue(str, out result))
                return true;

            if (StringToVector2(str, out result, keywords))
            {
                return true;
            }

            return false;
        }

        public static bool StringToVector3Offset(string str, out Vector3 result, IDictionary<string, Vector3> keywords = null)
        {
            str = str.Trim();
            if (str.Length < 3 || str[0] != 'o' || str[1] != ':')
            {
                result = Vector3.zero;
                return false;
            }

            str = str.Substring(2, str.Length - 2);
            if (keywords != null && keywords.TryGetValue(str, out result))
                return true;

            if (StringToVector3(str, out result, keywords))
            {
                return true;
            }

            return false;
        }

        public static bool StringToAnchor(string str, out Vector2 result, IDictionary<string, Vector2> anchorKeywords = null, IDictionary<string, Vector2> vectorKeywords = null)
        {
            str = str.Trim();
            if (str.Length < 3 || str[0] != 'a' || str[1] != ':')
            {
                result = Vector3.zero;
                return false;
            }

            if (anchorKeywords != null && anchorKeywords.TryGetValue(str, out result))
                return true;

            str = str.Substring(2, str.Length - 2);
            if (StringToVector2(str, out result, vectorKeywords))
            {
                return true;
            }

            return false;
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
        public static bool StringToAnimationCurve(string str, out AnimationCurve result, IDictionary<string, AnimationCurve> keywords = null)
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

            // else, keyword; first passed in ones, then built in ones
            if (keywords != null && keywords.TryGetValue(str, out result))
                return true;

            if (AnimationCurveUtility.NameConstructorMapping.TryGetValue(str, out Func<AnimationCurve> ctor))
            {
                result = ctor();
                return true;
            }

            return false;
        }

        public static bool StringToWaveOffsetType(string str, out AnimationUtility.WaveOffsetType result, IDictionary<string, AnimationUtility.WaveOffsetType> keywords = null)
        {
            result = default;
            str = str.Trim();

            switch (str)
            {
                case "sidx":
                case "sindex":
                case "segmentindex": result = AnimationUtility.WaveOffsetType.SegmentIndex; return true;

                case "idx":
                case "index": result = AnimationUtility.WaveOffsetType.Index; return true;

                case "word":
                case "wordidx":
                case "wordindex": result = AnimationUtility.WaveOffsetType.Word; return true;

                case "line":
                case "lineno":
                case "linenumber": result = AnimationUtility.WaveOffsetType.Line; return true;

                case "base":
                case "baseline": result = AnimationUtility.WaveOffsetType.Baseline; return true;

                case "x":
                case "xpos": result = AnimationUtility.WaveOffsetType.XPos; return true;

                case "y":
                case "ypos": result = AnimationUtility.WaveOffsetType.YPos; return true;
            }

            if (keywords != null && keywords.ContainsKey(str))
            {
                result = keywords[str];
                return true;
            }

            return false;
        }

        public static bool StringToColor(string str, out Color result, IDictionary<string, Color> keywords = null)
        {
            str = str.Trim();

            if (ColorKeyWords.TryGetValue(str, out Color color))
            {
                result = color;
                return true;
            }

            if (StringToHexColor(str, out result)) return true;
            if (StringToHSVColor(str, out result)) return true;
            if (StringToRGBColor(str, out result)) return true;
            if (StringToRGBAColor(str, out result)) return true;

            if (keywords != null && keywords.TryGetValue(str, out color))
            {
                result = color;
                return true;
            }

            return false;
        }


        internal static bool StringToHexColor(string str, out Color result)
        {
            result = default;

            if (str.Length != 7 && str.Length != 9) return false;

            if (str[0] != '#') return false;

            try
            {
                string red = str.Substring(1, 2);
                int redInt = Convert.ToInt32(red, 16);
                string green = str.Substring(3, 2);
                int greenInt = Convert.ToInt32(green, 16);
                string blue = str.Substring(5, 2);
                int blueInt = Convert.ToInt32(blue, 16);

                if (str.Length == 9)
                {
                    string alpha = str.Substring(7, 2);
                    int alphaInt = Convert.ToInt32(alpha, 16);
                    result = new Color(redInt / 255f, greenInt / 255f, blueInt / 255f, alphaInt / 255f);
                }

                result = new Color(redInt / 255f, greenInt / 255f, blueInt / 255f);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static bool StringToHSVColor(string str, out Color result)
        {
            result = default;

            if (str.Length < 10) return false;

            if (str.Substring(0, 3) == "hsv")
            {
                if (str[3] != '(') return false;
                if (str[str.Length - 1] != ')') return false;

                var values = str.Substring(4, str.Length - 5).Split(',');
                if (values.Length != 3 && values.Length != 4) return false;

                float[] floats = new float[3];
                for (int i = 0; i < 3; i++)
                {
                    if (!StringToFloat(values[i], out float res, null))
                    {
                        return false;
                    }

                    floats[i] = res;
                }

                if (values.Length == 4)
                {
                    if (!StringToBool(values[3], out bool res, null))
                    {
                        return false;
                    }

                    result = Color.HSVToRGB(floats[0], floats[1], floats[2], res);
                    return true;
                }
                else
                {
                    result = Color.HSVToRGB(floats[0], floats[1], floats[2]);
                    return true;
                }
            }

            return false;
        }

        internal static bool StringToRGBColor(string str, out Color result)
        {
            result = default;

            if (str.Length < 10) return false;

            if (str.Substring(0, 3) == "rgb")
            {
                if (str[3] != '(') return false;
                if (str[str.Length - 1] != ')') return false;

                var values = str.Substring(4, str.Length - 5).Split(',');
                if (values.Length != 3) return false;

                float[] floats = new float[3];
                for (int i = 0; i < 3; i++)
                {
                    if (!StringToFloat(values[i], out float res, null))
                    {
                        return false;
                    }

                    floats[i] = res;
                }

                result = new Color(floats[0], floats[1], floats[2]);
                return true;
            }

            return false;
        }

        internal static bool StringToRGBAColor(string str, out Color result, IDictionary<string, Color> keywords = null)
        {
            result = default;

            if (str.Length < 11) return false;

            if (str.Substring(0, 4) == "rgba")
            {
                if (str[4] != '(') return false;
                if (str[str.Length - 1] != ')') return false;

                var values = str.Substring(5, str.Length - 6).Split(',');
                if (values.Length != 4) return false;

                float[] floats = new float[4];
                for (int i = 0; i < 4; i++)
                {
                    if (!StringToFloat(values[i], out float res, null))
                    {
                        return false;
                    }

                    floats[i] = res;
                }

                result = new Color(floats[0], floats[1], floats[2], floats[3]);
                return true;
            }

            return false;
        }

        public static readonly ReadOnlyDictionary<string, Color> ColorKeyWords = new ReadOnlyDictionary<string, Color>(new Dictionary<string, Color>()
        {
            { "black", Color.black },
            { "blue", Color.blue },
            { "clear", Color.clear },
            { "green", Color.green },
            { "cyan", Color.cyan },
            { "gray", Color.gray },
            { "grey", Color.grey },
            { "magenta", Color.magenta },
            { "red", Color.red },
            { "white", Color.white },
            { "yellow", Color.yellow },

            { "maroon", new Color32(128, 0, 0, 255) },
            { "olive", new Color32(128, 128, 0, 255) },
            { "lime", new Color32(0, 255, 0, 255) },
            { "aqua", new Color32(0, 255, 255, 255) },
            { "teal", new Color32(0, 128, 128, 255) },
            { "navy", new Color32(0, 0, 128, 255) },
            { "fuchsia", new Color32(255, 0, 255, 255) },
            { "purple", new Color32(128, 0, 128, 255) },
            { "silver", new Color32(192, 192, 192, 255) },
            { "orange", new Color32(255, 165, 0, 255) },
            { "pink", new Color32(255, 192, 203, 255) },

            { "gold", new Color32(255, 215, 0, 255) },
            { "indigo", new Color32(75, 0, 130, 255) },
            { "violet", new Color32(238, 130, 238, 255) },
            { "brown", new Color32(165, 42, 42, 255) },
            { "beige", new Color32(245, 245, 220, 255) },
            { "ivory", new Color32(255, 255, 240, 255) },
            { "khaki", new Color32(240, 230, 140, 255) },
            { "lavender", new Color32(230, 230, 250, 255) },
            { "salmon", new Color32(250, 128, 114, 255) },
            { "turquoise", new Color32(64, 224, 208, 255) },

            { "coral", new Color32(255, 127, 80, 255) },
            { "peach", new Color32(255, 218, 185, 255) },
            { "mint", new Color32(189, 252, 201, 255) },
            { "skyblue", new Color32(135, 206, 235, 255) },
            { "plum", new Color32(221, 160, 221, 255) },
            { "chocolate", new Color32(210, 105, 30, 255) },
            { "tomato", new Color32(255, 99, 71, 255) },
            { "honeydew", new Color32(240, 255, 240, 255) },
            { "orchid", new Color32(218, 112, 214, 255) },
            { "papayawhip", new Color32(255, 239, 213, 255) },

            { "darkblue", new Color32(0, 0, 139, 255) },
            { "lightblue", new Color32(173, 216, 230, 255) },
            { "darkred", new Color32(139, 0, 0, 255) },
            { "lightred", new Color32(255, 102, 102, 255) },
            { "darkgreen", new Color32(0, 100, 0, 255) },
            { "lightgreen", new Color32(144, 238, 144, 255) },
            { "darkyellow", new Color32(204, 204, 0, 255) },
            { "lightyellow", new Color32(255, 255, 224, 255) },
            { "darkorange", new Color32(255, 140, 0, 255) },
            { "lightorange", new Color32(255, 165, 0, 255) },
            { "darkviolet", new Color32(148, 0, 211, 255) },
            { "lightviolet", new Color32(238, 130, 238, 255) },
            { "darkbrown", new Color32(101, 67, 33, 255) },
            { "lightbrown", new Color32(181, 101, 29, 255) },
            { "darkpurple", new Color32(128, 0, 128, 255) },
            { "lightpurple", new Color32(147, 112, 219, 255) },
            { "darkmagenta", new Color32(139, 0, 139, 255) },
            { "lightmagenta", new Color32(255, 0, 255, 255) },
        });

        internal static bool VectorSequenceToAnimationCurve(string str, ref AnimationCurve result)
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

        internal static bool MethodToAnimationCurve(string str, ref AnimationCurve result)
        {
            if (str.Length < 4) return false;

            List<Vector2> vectors = new List<Vector2>();
            int currentStartIndex = str.IndexOf("((", 0);
            int currentEndIndex = str.IndexOf(')', currentStartIndex);

            if (currentStartIndex == -1 || currentEndIndex == -1) return false;

            Func<IEnumerable<Vector2>, AnimationCurve> constructor = AnimationCurveUtility.NameBezierConstructorMapping[str.Substring(0, currentStartIndex)];

            currentStartIndex++;
            var span = str.AsSpan();
            if (span[span.Length - 1] != ')' || span[span.Length - 2] != ')')
                return false;

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

            result = constructor(vectors);
            return true;
        }

        internal static bool SpanToVector2(ReadOnlySpan<char> span, out Vector2 result)
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

        internal static bool SpanToFloat(ReadOnlySpan<char> span, out float result)
        {
            if (!float.TryParse(span, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                return false;

            return true;
        }
        #endregion
    }
}
