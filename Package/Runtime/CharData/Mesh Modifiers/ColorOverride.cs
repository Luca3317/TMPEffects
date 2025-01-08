using System;
using UnityEngine;

namespace TMPEffects.Modifiers
{
    [System.Serializable]
    public struct ColorOverride
    {
        public OverrideMode Override;
        public Color32 Color;
    
        public bool OverrideAlpha => Override.HasFlag(OverrideMode.Alpha);
        public bool OverrideColor => Override.HasFlag(OverrideMode.Color);

        public ColorOverride(ColorOverride original)
        {
            Color = original.Color;
            Override = original.Override;
        }

        public ColorOverride(Color32 color, OverrideMode overrideMode)
        {
            Override = overrideMode;
            Color = color;
        }

        public Color32 GetValue(Color32 fallback)
        {
            Color32 color;
            if (OverrideColor)
                color = Color;
            else
                color = fallback;

            if (OverrideAlpha)
                color.a = Color.a;
            else
                color.a = fallback.a;
        
            return color;
        }
    
        [Flags]
        public enum OverrideMode : byte
        {
            None = 0,
            Color = 1,
            Alpha = 1 << 1
        }

        public bool Equals(ColorOverride obj)
        {
            return Override == obj.Override &&
                   Color.r == obj.Color.r &&
                   Color.g == obj.Color.g &&
                   Color.b == obj.Color.b &&
                   Color.a == obj.Color.a;
        }

        public static ColorOverride LerpUnclamped(ColorOverride start, Color32 end, float t)
        {
            return LerpUnclamped(end, start, 1 - t);
        }

        public static ColorOverride LerpUnclamped(Color32 start, ColorOverride end, float t)
        {
            if (t >= 1)
            {
                return new ColorOverride(end);
            }

            if (t <= 0)
            {
                return new ColorOverride()
                {
                    Color = start,
                    Override = end.Override
                };
            }

            ColorOverride result = new ColorOverride();
            result.Color = Color32.LerpUnclamped(start, end.Color, t);
            result.Override = end.Override;
            return result;
        }

        public static ColorOverride LerpUnclamped(ColorOverride start, ColorOverride end, float t)
        {
            ColorOverride result = new ColorOverride();

            byte r = 0, g = 0, b = 0, a = 0;

            Color32 color;
            if (t >= 1)
            {
                if (end.OverrideAlpha)
                    result.Override |= OverrideMode.Alpha;
                a = end.Color.a;

                if (end.OverrideColor)
                    result.Override |= OverrideMode.Color;
                r = end.Color.r;
                g = end.Color.g;
                b = end.Color.b;
                color = new Color32(r, g, b, a);
            }
            else if (t <= 0)
            {
                if (start.OverrideAlpha)
                    result.Override |= OverrideMode.Alpha;
                a = start.Color.a;

                if (start.OverrideColor)
                    result.Override |= OverrideMode.Color;
                r = start.Color.r;
                g = start.Color.g;
                b = start.Color.b;
                color = new Color32(r, g, b, a);
            }
            else
            {
                color = Color32.Lerp(start.Color, end.Color, t);

                if (start.OverrideAlpha || end.OverrideAlpha)
                    result.Override |= OverrideMode.Alpha;
                if (start.OverrideColor || end.OverrideColor)
                    result.Override |= OverrideMode.Color;
            }

            result.Color = color;
            return result;
        }

        public static ColorOverride operator +(ColorOverride lhs, ColorOverride rhs)
        {
            ColorOverride result = new ColorOverride();

            byte r = 0, g = 0, b = 0, a = 0;
            if (rhs.OverrideAlpha)
            {
                result.Override |= OverrideMode.Alpha;
                a = rhs.Color.a;
            }
            else if (lhs.OverrideAlpha)
            {
                result.Override |= OverrideMode.Alpha;
                a = lhs.Color.a;
            }

            if (rhs.OverrideColor)
            {
                result.Override |= OverrideMode.Color;
                r = rhs.Color.r;
                g = rhs.Color.g;
                b = rhs.Color.b;
            }
            else if (lhs.OverrideColor)
            {
                result.Override |= OverrideMode.Color;
                r = lhs.Color.r;
                g = lhs.Color.g;
                b = lhs.Color.b;
            }

            result.Color = new Color32(r, g, b, a);
            return result;
        }

        public override string ToString()
        {
            return "{ Color: " + Color + " Override: " + Override + " }";
        }
    }
}