using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using TMPEffects.Databases;
using TMPEffects.Extensions;
using UnityEngine;

namespace TMPEffects.Parameters
{
    /// <summary>
    /// Utility class for parsing parameters.
    /// </summary>
    public static class ParameterParsing
    {
        private static string TrimIfNeeded(string text)
        {
            if (text.Length == 0) return text;
            if (!char.IsWhiteSpace(text[0]) && !char.IsWhiteSpace(text[^1])) return text;
            return text.Trim();
        }
        
        public static bool StringToInt(string str, out int result, ITMPKeywordDatabase keywords = null)
        {
            result = 0;

            str = TrimIfNeeded(str);
            if (str.Length == 0) return false;

            if (string.IsNullOrWhiteSpace(str)) return false;

            if (keywords != null && keywords.TryGetInt(str, out result))
                return true;

            if (int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                return true;

            return true;
        }

        public static bool StringToFloat(string str, out float result, ITMPKeywordDatabase keywords = null)
        {
            result = 0;

            str = TrimIfNeeded(str);
            if (str.Length == 0) return false;

            if (keywords != null && keywords.TryGetFloat(str, out result))
                return true;

            if (float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                return true;

            return false;
        }

        public static bool StringToBool(string str, out bool result, ITMPKeywordDatabase keywords = null)
        {
            result = false;

            str = TrimIfNeeded(str);
            if (str.Length == 0) return false;

            if (keywords != null && keywords.TryGetBool(str, out result))
                return true;

            if (bool.TryParse(str, out result))
                return true;

            return false;
        }


        public static bool StringToVector2(string str, out Vector2 result, ITMPKeywordDatabase keywords = null)
        {
            result = new Vector2(0, 0);

            str = TrimIfNeeded(str);
            if (str.Length == 0) return false;

            if (keywords != null && keywords.TryGetVector3(str, out Vector3 result3))
            {
                result = result3;
                return true;
            }

            Vector2? v;
            if ((v = TryParse()) != null)
            {
                result = v.Value;
                return true;
            }

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
                if (!StringToFloat(str.Substring(1, commaIndex - 1), out x, keywords)) return null;

                if (!StringToFloat(str.Substring(commaIndex + 1, str.Length - (commaIndex + 2)), out y, keywords))
                    return null;

                return new Vector2(x, y);
            }
        }

        public static bool StringToTypedVector3(string str, out TMPParameterTypes.TypedVector3 result,
            ITMPKeywordDatabase keywords = null)
        {
            if (StringToVector3(str, out Vector3 vec, keywords))
            {
                result = new TMPParameterTypes.TypedVector3(TMPParameterTypes.VectorType.Position, vec);
                return true;
            }

            if (StringToAnchor(str, out Vector2 vec2, keywords))
            {
                result = new TMPParameterTypes.TypedVector3(TMPParameterTypes.VectorType.Anchor, vec2);
                return true;
            }

            if (StringToVector3Offset(str, out vec, keywords))
            {
                result = new TMPParameterTypes.TypedVector3(TMPParameterTypes.VectorType.Offset, vec);
                return true;
            }

            result = default;
            return false;
        }

        public static bool StringToTypedVector2(string str, out TMPParameterTypes.TypedVector2 result,
            ITMPKeywordDatabase keywords = null)
        {
            if (StringToVector2(str, out Vector2 vec, keywords))
            {
                result = new TMPParameterTypes.TypedVector2(TMPParameterTypes.VectorType.Position, vec);
                return true;
            }

            if (StringToAnchor(str, out vec, keywords))
            {
                result = new TMPParameterTypes.TypedVector2(TMPParameterTypes.VectorType.Anchor, vec);
                return true;
            }

            if (StringToVector2Offset(str, out vec, keywords))
            {
                result = new TMPParameterTypes.TypedVector2(TMPParameterTypes.VectorType.Offset, vec);
                return true;
            }

            result = default;
            return false;
        }

        public static bool StringToVector3(string str, out Vector3 result, ITMPKeywordDatabase keywords = null)
        {
            result = new Vector3(0, 0, 0);

            str = TrimIfNeeded(str);
            if (str.Length == 0) return false;

            if (keywords != null && keywords.TryGetVector3(str, out result))
                return true;

            Vector3? v;
            if ((v = TryParse()) != null)
            {
                result = v.Value;
                return true;
            }

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
                    if (!StringToFloat(split[0], out x, keywords)) return null;
                    if (!StringToFloat(split[1], out y, keywords)) return null;
                }
                else
                {
                    if (!StringToFloat(split[0], out x, keywords)) return null;
                    if (!StringToFloat(split[1], out y, keywords)) return null;
                    if (!StringToFloat(split[2], out z, keywords)) return null;
                }

                return new Vector3(x, y, z);
            }
        }

        public static bool StringToVector2Offset(string str, out Vector2 result, ITMPKeywordDatabase keywords = null)
        {
            result = Vector2.zero;

            str = TrimIfNeeded(str);
            if (str.Length == 0) return false;

            // check for "o:" prefix
            if (str.Length < 3 || str[0] != 'o' || str[1] != ':')
            {
                return false;
            }

            str = str.Substring(2, str.Length - 2);
            if (StringToVector2(str, out result, keywords))
            {
                return true;
            }

            return false;
        }

        public static bool StringToVector3Offset(string str, out Vector3 result, ITMPKeywordDatabase keywords = null)
        {
            result = Vector3.zero;

            str = TrimIfNeeded(str);
            if (str.Length == 0) return false;

            if (str.Length < 3 || str[0] != 'o' || str[1] != ':')
                return false;

            str = str.Substring(2, str.Length - 2);
            if (StringToVector3(str, out result, keywords))
                return true;

            return false;
        }

        public static bool StringToAnchor(string str, out Vector2 result, ITMPKeywordDatabase keywords = null)
        {
            result = default;

            str = TrimIfNeeded(str);
            if (str.Length == 0) return false;

            // check for "a:" prefix
            if (str.Length < 3 || str[0] != 'a' || str[1] != ':')
            {
                result = Vector3.zero;
                return false;
            }

            if (keywords != null && keywords.TryGetAnchor(str, out result))
                return true;

            str = str.Substring(2, str.Length - 2);
            
            if (StringToVector2(str, out result, keywords))
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
        public static bool StringToAnimCurve(string str, out AnimationCurve result, ITMPKeywordDatabase keywords = null)
        {
            result = null;

            str = TrimIfNeeded(str);
            if (str.Length == 0) return false;

            if (keywords != null && keywords.TryGetAnimCurve(str, out result))
            {
                result = new AnimationCurve(result.keys);
                return true;
            }

            // If vector
            if (str[0] == '(')
            {
                return VectorSequenceToAnimationCurve(str, ref result, keywords);
            }

            // If method
            if (str.Contains('('))
            {
                return MethodToAnimationCurve(str, ref result, keywords);
            }

            return false;
        }

        public static bool StringToUnityObject(string str, out UnityEngine.Object result,
            ITMPKeywordDatabase keywords = null)
        {
            result = null;

            str = TrimIfNeeded(str);
            if (str.Length == 0) return false;

            return keywords != null && keywords.TryGetUnityObject(str, out result);
        }


        // public static bool StringToOffsetProvider(string str, out ITMPOffsetProvider result,
        //     ITMPKeywordDatabase keywords = null)
        // {
        //     result = default;
        //     
        //     str = TrimIfNeeded(str);
        //     if (str.Length == 0) return false;
        //
        //     if (keywords != null && keywords.TryGetOffsetType(str, out result))
        //         return true;
        //     
        //     return GlobalKeywordDatabase.TryGetOffsetType(str, out result);
        // }

        public static bool StringToColor(string str, out Color result, ITMPKeywordDatabase keywords = null)
        {
            result = default;

            str = TrimIfNeeded(str);
            if (str.Length == 0) return false;

            if (keywords != null && keywords.TryGetColor(str, out result))
                return true;

            if (StringToHexColor(str, out result, keywords)) return true;
            if (StringToHSVColor(str, out result, keywords)) return true;
            if (StringToRGBColor(str, out result, keywords)) return true;
            if (StringToRGBAColor(str, out result, keywords)) return true;

            return false;
        }

        internal static bool StringToHexInt(string str, out int result, ITMPKeywordDatabase keywords = null)
        {
            try
            {
                result = Convert.ToInt32(str, 16);
                return true;
            }
            catch
            {
                result = default;
                return keywords != null && keywords.TryGetInt(str, out result);
            }
        }

        internal static bool StringToHexColor(string str, out Color result, ITMPKeywordDatabase keywords = null)
        {
            result = default;

            if (str.Length != 7 && str.Length != 9) return false;

            if (str[0] != '#') return false;

            string red = str.Substring(1, 2);
            if (!StringToHexInt(red, out int redInt, keywords))
                return false;

            string green = str.Substring(3, 2);
            if (!StringToHexInt(green, out int greenInt, keywords))
                return false;

            string blue = str.Substring(5, 2);
            if (!StringToHexInt(blue, out int blueInt, keywords))
                return false;

            if (str.Length == 9)
            {
                string alpha = str.Substring(7, 2);
                if (!StringToHexInt(alpha, out int alphaInt, keywords))
                    return false;

                result = new Color(redInt / 255f, greenInt / 255f, blueInt / 255f, alphaInt / 255f);
            }
            else
                result = new Color(redInt / 255f, greenInt / 255f, blueInt / 255f);

            return true;
        }

        internal static bool StringToHSVColor(string str, out Color result, ITMPKeywordDatabase keywords = null)
        {
            result = default;

            if (str.Length < 10) return false;

            if (str.Substring(0, 3) != "hsv")
                return false;

            if (str[3] != '(') return false;
            if (str[str.Length - 1] != ')') return false;

            var values = str.Substring(4, str.Length - 5).Split(',');
            if (values.Length != 3 && values.Length != 4) return false;

            float[] floats = new float[3];
            for (int i = 0; i < 3; i++)
            {
                if (!StringToFloat(values[i], out float res, keywords))
                {
                    return false;
                }

                floats[i] = res;
            }

            if (values.Length == 4)
            {
                if (!StringToBool(values[3], out bool res, keywords))
                {
                    return false;
                }

                result = Color.HSVToRGB(floats[0], floats[1], floats[2], res);
            }
            else
            {
                result = Color.HSVToRGB(floats[0], floats[1], floats[2]);
            }

            return true;
        }

        internal static bool StringToRGBColor(string str, out Color result, ITMPKeywordDatabase keywords = null)
        {
            result = default;

            if (str.Length < 10) return false;

            if (str.Substring(0, 3) != "rgb")
                return false;

            if (str[3] != '(') return false;
            if (str[str.Length - 1] != ')') return false;

            var values = str.Substring(4, str.Length - 5).Split(',');
            if (values.Length != 3) return false;

            float[] floats = new float[3];
            for (int i = 0; i < 3; i++)
            {
                if (!StringToFloat(values[i], out float res, keywords))
                {
                    return false;
                }

                floats[i] = res;
            }

            result = new Color(floats[0], floats[1], floats[2]);
            return true;
        }

        internal static bool StringToRGBAColor(string str, out Color result, ITMPKeywordDatabase keywords = null)
        {
            result = default;

            if (str.Length < 11) return false;

            if (str.Substring(0, 4) != "rgba")
                return false;

            if (str[4] != '(') return false;
            if (str[str.Length - 1] != ')') return false;

            var values = str.Substring(5, str.Length - 6).Split(',');
            if (values.Length != 4) return false;

            float[] floats = new float[4];
            for (int i = 0; i < 4; i++)
            {
                if (!StringToFloat(values[i], out float res, keywords))
                {
                    return false;
                }

                floats[i] = res;
            }

            result = new Color(floats[0], floats[1], floats[2], floats[3]);
            return true;
        }

        internal static bool VectorSequenceToAnimationCurve(string str, ref AnimationCurve result,
            ITMPKeywordDatabase keywords = null)
        {
            List<Vector2> vectors = new List<Vector2>();
            int currentStartIndex = str.IndexOf('(', 0);
            int currentEndIndex = str.IndexOf(')', currentStartIndex);

            if (currentStartIndex == -1 || currentEndIndex == -1) return false;

            while (currentEndIndex < str.Length && currentEndIndex != -1)
            {
                var s = str.Substring(currentStartIndex, currentEndIndex + 1 - currentStartIndex);

                if (!StringToVector2(s, out Vector2 vectorResult, keywords))
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

        internal static bool MethodToAnimationCurve(string str, ref AnimationCurve result,
            ITMPKeywordDatabase keywords = null)
        {
            if (str.Length < 4) return false;

            List<Vector2> vectors = new List<Vector2>();
            int currentStartIndex = str.IndexOf("((", 0);
            int currentEndIndex = str.IndexOf(')', currentStartIndex);

            if (currentStartIndex == -1 || currentEndIndex == -1) return false;

            Func<IEnumerable<Vector2>, AnimationCurve> constructor =
                AnimationCurveUtility.NameBezierConstructorMapping[str.Substring(0, currentStartIndex)];

            currentStartIndex++;
            if (str[str.Length - 1] != ')' || str[str.Length - 2] != ')')
                return false;

            while (currentEndIndex < str.Length && currentEndIndex != -1)
            {
                var s = str.Substring(currentStartIndex, currentEndIndex + 1 - currentStartIndex);

                if (!StringToVector2(s, out Vector2 vectorResult, keywords))
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
    }
}