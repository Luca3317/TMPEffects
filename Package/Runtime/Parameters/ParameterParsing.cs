using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using TMPEffects.Extensions;
using UnityEngine;

namespace TMPEffects.Parameters
{
    public static class ParameterParsing
    {
        public static TMPKeywordDatabase GlobalKeywordDatabase
        {
            get
            {
                if (globalKeywordDatabase == null)
                {
                    var settings = Resources.Load<TMPEffectsSettings>("TMPEffectsSettings_Project");
                    globalKeywordDatabase = settings.GlobalKeywordDatabase;
                }
                
                return globalKeywordDatabase;
            }
        }

        private static TMPKeywordDatabase globalKeywordDatabase = null;
        
        #region Parsing
        public static bool StringToInt(string str, out int result, IDictionary<string, int> keywords = null)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(str)) return false;

            if (keywords != null && keywords.TryGetValue(str, out result))
                return true;

            if (GlobalKeywordDatabase.IntKeywords.TryGetValue(str, out result))
                return true;
            
            // TODO need to decide on order
            // Either   Passed In keywords -> global key words -> parse
            // Or       Parse -> passed in -> global
            // And then fix for all of stringtoX methods
            if (int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                return true;

            return true;
        }

        public static bool StringToFloat(string str, out float result, IDictionary<string, float> keywords = null)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(str)) return false;
            
            if (keywords != null && keywords.TryGetValue(str, out result)) 
                return true;
            
            if (GlobalKeywordDatabase.FloatKeywords.TryGetValue(str, out result))
                return true;

            if (float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
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

        public static bool StringToTypedVector3(string str, out ParameterTypes.TypedVector3 result,
            IDictionary<string, string> keywords = null)
        {
            if (StringToVector3(str, out Vector3 vec))
            {
                result = new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Position, vec);
                return true;
            }
            
            if (StringToAnchor(str, out Vector2 vec2))
            {
                result = new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Anchor, vec2);
                return true;
            }
            
            if (StringToVector3Offset(str, out vec))
            {
                result = new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, vec);
                return true;
            }

            result = default;
            return false;
        }
        
        public static bool StringToTypedVector2(string str, out ParameterTypes.TypedVector2 result,
            IDictionary<string, string> keywords = null)
        {
            if (StringToVector2(str, out Vector2 vec))
            {
                result = new ParameterTypes.TypedVector2(ParameterTypes.VectorType.Position, vec);
                return true;
            }
            
            if (StringToAnchor(str, out vec))
            {
                result = new ParameterTypes.TypedVector2(ParameterTypes.VectorType.Anchor, vec);
                return true;
            }
            
            if (StringToVector2Offset(str, out vec))
            {
                result = new ParameterTypes.TypedVector2(ParameterTypes.VectorType.Offset, vec);
                return true;
            }

            result = default;
            return false;
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

        public static bool StringToVector2Offset(string str, out Vector2 result,
            IDictionary<string, Vector2> keywords = null)
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

        public static bool StringToVector3Offset(string str, out Vector3 result,
            IDictionary<string, Vector3> keywords = null)
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

        public static bool StringToAnchor(string str, out Vector2 result,
            IDictionary<string, Vector2> anchorKeywords = null, IDictionary<string, Vector2> vectorKeywords = null)
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
        public static bool StringToAnimCurve(string str, out AnimationCurve result,
            IDictionary<string, AnimationCurve> keywords = null)
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

        public static bool StringToWaveOffset(string str, out ParameterTypes.WaveOffsetType result,
            IDictionary<string, ParameterTypes.WaveOffsetType> keywords = null)
        {
            result = default;
            str = str.Trim();

            switch (str)
            {
                case "sidx":
                case "sindex":
                case "segmentindex":
                    result = ParameterTypes.WaveOffsetType.SegmentIndex;
                    return true;

                case "idx":
                case "index":
                    result = ParameterTypes.WaveOffsetType.Index;
                    return true;

                case "word":
                case "wordidx":
                case "wordindex":
                    result = ParameterTypes.WaveOffsetType.Word;
                    return true;

                case "line":
                case "lineno":
                case "linenumber":
                    result = ParameterTypes.WaveOffsetType.Line;
                    return true;

                case "base":
                case "baseline":
                    result = ParameterTypes.WaveOffsetType.Baseline;
                    return true;

                case "x":
                case "xpos":
                    result = ParameterTypes.WaveOffsetType.XPos;
                    return true;

                case "y":
                case "ypos":
                    result = ParameterTypes.WaveOffsetType.YPos;
                    return true;
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

        public static readonly ReadOnlyDictionary<string, Color> ColorKeyWords = new ReadOnlyDictionary<string, Color>(
            new Dictionary<string, Color>()
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

            Func<IEnumerable<Vector2>, AnimationCurve> constructor =
                AnimationCurveUtility.NameBezierConstructorMapping[str.Substring(0, currentStartIndex)];

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