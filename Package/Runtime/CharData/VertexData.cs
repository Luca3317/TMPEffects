using TMPEffects.Modifiers;
using TMPro;
using UnityEngine;

namespace TMPEffects.CharacterData
{
    /// <summary>
    /// Holds data about a TextMeshPro character mesh.
    /// </summary>
    public class VertexData
    {
        /// <summary>
        /// The modifiers of this mesh.
        /// </summary>
        public TMPMeshModifiers Modifiers => modifiers;
        private TMPMeshModifiers modifiers;

        /// <summary>
        /// The bottom left vertex. Index = 0
        /// </summary>
        private TMP_Vertex vertex_BL;

        /// <summary>
        /// The top left vertex. Index = 1
        /// </summary>
        private TMP_Vertex vertex_TL;

        /// <summary>
        /// The top right vertex. Index = 2
        /// </summary>
        private TMP_Vertex vertex_TR;

        /// <summary>
        /// The bottom right vertex. Index = 3
        /// </summary>
        private TMP_Vertex vertex_BR;


        /// <summary>
        /// The initial, immutable vertex data of the character.
        /// </summary>
        public readonly ReadOnlyVertexData initial;

        /// <summary>
        /// Whether the positions have been manipulated.
        /// </summary>
        public bool positionsDirty { get; private set; }

        /// <summary>
        /// Whether the vertex colors have been manipulated.
        /// </summary>
        public bool colorsDirty { get; private set; }

        /// <summary>
        /// Whether the vertex alphas have been manipulated.
        /// </summary>
        public bool alphasDirty { get; private set; }

        /// <summary>
        /// Whether the UVs have been manipulated.
        /// </summary>
        public bool uvsDirty { get; private set; }

        /// <summary>
        /// Get or set the color of the bottom left vertex.<br/>
        /// Note that this will mark both colors and alphas as dirty.<br/>
        /// Use <see cref="SetColor(int, Color32, bool)"/> if you want to only set color.
        /// </summary>
        public Color32 BL_Color
        {
            get => modifiers.BL_Color.GetValue(vertex_BL.color);
            set
            {
                var coloroverride = modifiers.BL_Color;
                coloroverride.Override |= ColorOverride.OverrideMode.Alpha | ColorOverride.OverrideMode.Color;
                coloroverride.Color = value;
                modifiers.BL_Color = coloroverride;
                colorsDirty = true;
                alphasDirty = true;
            }
        }

        /// <summary>
        /// Get or set the color of the top left vertex.<br/>
        /// Note that this will mark both colors and alphas as dirty.<br/>
        /// Use <see cref="SetColor(int, Color32, bool)"/> if you want to only set color.
        /// </summary>
        public Color32 TL_Color
        {
            get => modifiers.TL_Color.GetValue(vertex_TL.color);
            set
            {
                var coloroverride = modifiers.TL_Color;
                coloroverride.Override |= ColorOverride.OverrideMode.Alpha | ColorOverride.OverrideMode.Color;
                coloroverride.Color = value;
                modifiers.TL_Color = coloroverride;
                colorsDirty = true;
                alphasDirty = true;
            }
        }

        /// <summary>
        /// Get or set the color of the top right vertex.<br/>
        /// Note that this will mark both colors and alphas as dirty.<br/>
        /// Use <see cref="SetColor(int, Color32, bool)"/> if you want to only set color.
        /// </summary>
        public Color32 TR_Color
        {
            get => modifiers.TR_Color.GetValue(vertex_TR.color);
            set
            {
                var coloroverride = modifiers.TR_Color;
                coloroverride.Override |= ColorOverride.OverrideMode.Alpha | ColorOverride.OverrideMode.Color;
                coloroverride.Color = value;
                modifiers.TR_Color = coloroverride;
                colorsDirty = true;
                alphasDirty = true;
            }
        }

        /// <summary>
        /// Get or set the color of the bottom right vertex.<br/>
        /// Note that this will mark both colors and alphas as dirty.<br/>
        /// Use <see cref="SetColor(int, Color32, bool)"/> if you want to only set color.
        /// </summary>
        public Color32 BR_Color
        {
            get => modifiers.BR_Color.GetValue(vertex_BR.color);
            set
            {
                var coloroverride = modifiers.BR_Color;
                coloroverride.Override |= ColorOverride.OverrideMode.Alpha | ColorOverride.OverrideMode.Color;
                coloroverride.Color = value;
                modifiers.BR_Color = coloroverride;
                colorsDirty = true;
                alphasDirty = true;
            }
        }

        /// <summary>
        /// Get or set the alpha of the bottom left vertex.
        /// </summary>
        public byte BL_Alpha
        {
            get => modifiers.BL_Color.GetValue(vertex_BL.color).a;
            set
            {
                var coloroverride = modifiers.BL_Color;
                coloroverride.Override |= ColorOverride.OverrideMode.Alpha;
                coloroverride.Color.a = value;
                modifiers.BL_Color = coloroverride;
                alphasDirty = true;
            }
        }

        /// <summary>
        /// Get or set the alpha of the top left vertex.
        /// </summary>
        public byte TL_Alpha
        {
            get => modifiers.TL_Color.GetValue(vertex_TL.color).a;
            set
            {
                var coloroverride = modifiers.TL_Color;
                coloroverride.Override |= ColorOverride.OverrideMode.Alpha;
                coloroverride.Color.a = value;
                modifiers.TL_Color = coloroverride;
                alphasDirty = true;
            }
        }

        /// <summary>
        /// Get or set the alpha of the top right vertex.
        /// </summary>
        public byte TR_Alpha
        {
            get => modifiers.TR_Color.GetValue(vertex_TR.color).a;
            set
            {
                var coloroverride = modifiers.TR_Color;
                coloroverride.Override |= ColorOverride.OverrideMode.Alpha;
                coloroverride.Color.a = value;
                modifiers.TR_Color = coloroverride;
                alphasDirty = true;
            }
        }

        /// <summary>
        /// Get or set the alpha of the bottom right vertex.
        /// </summary>
        public byte BR_Alpha
        {
            get => modifiers.BR_Color.GetValue(vertex_BR.color).a;
            set
            {
                var coloroverride = modifiers.BR_Color;
                coloroverride.Override |= ColorOverride.OverrideMode.Alpha;
                coloroverride.Color.a = value;
                modifiers.BR_Color = coloroverride;
                alphasDirty = true;
            }
        }

        /// <summary>
        /// Get or set the position of the bottom left vertex.
        /// </summary>
        public Vector3 BL_Position
        {
            get => vertex_BL.position + modifiers.BL_Delta;
            set
            {
                modifiers.BL_Delta = value - vertex_BL.position;
                positionsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the position of the top left vertex.
        /// </summary>
        public Vector3 TL_Position
        {
            get => vertex_TL.position + modifiers.TL_Delta;
            set
            {
                modifiers.TL_Delta = value - vertex_TL.position;
                positionsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the position of the top right vertex.
        /// </summary>
        public Vector3 TR_Position
        {
            get => vertex_TR.position + modifiers.TR_Delta;
            set
            {
                modifiers.TR_Delta = value - vertex_TR.position;
                positionsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the position of the bottom right vertex.
        /// </summary>
        public Vector3 BR_Position
        {
            get => vertex_BR.position + modifiers.BR_Delta;
            set
            {
                modifiers.BR_Delta = value - vertex_BR.position;
                positionsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the UV0 of the bottom left vertex.
        /// </summary>
        public Vector3 BL_UV0
        {
            get => modifiers.BL_UV0.GetValue(vertex_BL.uv);
            set
            {
                modifiers.BL_UV0 =
                    new Vector3Override(value); // TODO maybe rewrite overrides so they store the fallback value? ever so slightly faster
                uvsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the UV0 of the bottom right vertex.
        /// </summary>
        public Vector3 TL_UV0
        {
            get => modifiers.TL_UV0.GetValue(vertex_TL.uv);
            set
            {
                modifiers.TL_UV0 = new Vector3Override(value);
                uvsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the UV0 of the top right vertex.
        /// </summary>
        public Vector3 TR_UV0
        {
            get => modifiers.TR_UV0.GetValue(vertex_TR.uv);
            set
            {
                modifiers.TR_UV0 = new Vector3Override(value);
                uvsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the UV0 of the bottom right vertex.
        /// </summary>
        public Vector3 BR_UV0
        {
            get => modifiers.BR_UV0.GetValue(vertex_BR.uv);
            set
            {
                modifiers.BR_UV0 = new Vector3Override(value);
                uvsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the UV2 of the bottom left vertex.
        /// </summary>
        public Vector3 BL_UV2
        {
            get => modifiers.BL_UV2.GetValue(vertex_BL.uv2);
            set
            {
                modifiers.BL_UV2 = new Vector3Override(value);
                uvsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the UV2 of the bottom right vertex.
        /// </summary>
        public Vector3 TL_UV2
        {
            get => modifiers.TL_UV2.GetValue(vertex_TL.uv2);
            set
            {
                modifiers.TL_UV2 = new Vector3Override(value);
                uvsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the UV2 of the top right vertex.
        /// </summary>
        public Vector3 TR_UV2
        {
            get => modifiers.TR_UV2.GetValue(vertex_TR.uv2);
            set
            {
                modifiers.TR_UV2 = new Vector3Override(value);
                uvsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the UV2 of the bottom right vertex.
        /// </summary>
        public Vector3 BR_UV2
        {
            get => modifiers.BR_UV2.GetValue(vertex_BR.uv2);
            set
            {
                modifiers.BR_UV2 = new Vector3Override(value);
                uvsDirty = true;
            }
        }

        public VertexData(TMP_Vertex bl, TMP_Vertex tl, TMP_Vertex tr, TMP_Vertex br)
        {
            positionsDirty = false;
            uvsDirty = false;
            colorsDirty = false;
            alphasDirty = false;

            initial = new ReadOnlyVertexData(bl, tl, tr, br);
            this.vertex_BL = bl;
            this.vertex_TL = tl;
            this.vertex_TR = tr;
            this.vertex_BR = br;
            modifiers = new TMPMeshModifiers();
        }

        public VertexData(TMP_CharacterInfo info)
        {
            positionsDirty = false;
            uvsDirty = false;
            colorsDirty = false;
            alphasDirty = false;

            initial = new ReadOnlyVertexData(info);
            this.vertex_BL = info.vertex_BL;
            this.vertex_TL = info.vertex_TL;
            this.vertex_TR = info.vertex_TR;
            this.vertex_BR = info.vertex_BR;
            modifiers = new TMPMeshModifiers();
        }

        /// <summary>
        /// Get the position of the vertex associated with the index.<br/>
        /// 0 => bottom left<br/>
        /// 1 => top left<br/>
        /// 2 => top right<br/>
        /// 3 => bottom right<br/>
        /// </summary>
        /// <param name="i">The index.</param>
        /// <returns>The position of the vertex associated with the index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public Vector3 GetPosition(int i)
        {
            switch (i)
            {
                case 0: return BL_Position;
                case 1: return TL_Position;
                case 2: return TR_Position;
                case 3: return BR_Position;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Set the position of the vertex associated with the index.<br/>
        /// 0 => bottom left<br/>
        /// 1 => top left<br/>
        /// 2 => top right<br/>
        /// 3 => bottom right<br/>
        /// </summary>
        /// <param name="i">The index.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public void SetPosition(int i, Vector3 value)
        {
            switch (i)
            {
                case 0:
                    BL_Position = value;
                    break;
                case 1:
                    TL_Position = value;
                    break;
                case 2:
                    TR_Position = value;
                    break;
                case 3:
                    BR_Position = value;
                    break;
                default: throw new System.ArgumentOutOfRangeException();
            }

            positionsDirty = true;
        }

        /// <summary>
        /// Get the color of the vertex associated with the index.<br/>
        /// 0 => bottom left<br/>
        /// 1 => top left<br/>
        /// 2 => top right<br/>
        /// 3 => bottom right<br/>
        /// </summary>
        /// <param name="i">The index.</param>
        /// <returns>The color of the vertex associated with the index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public Color32 GetColor(int i)
        {
            switch (i)
            {
                case 0: return BL_Color;
                case 1: return TL_Color;
                case 2: return TR_Color;
                case 3: return BR_Color;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Set the color of the vertex associated with the index.<br/>
        /// 0 => bottom left<br/>
        /// 1 => top left<br/>
        /// 2 => top right<br/>
        /// 3 => bottom right<br/>
        /// </summary>
        /// <param name="i">The index.</param>
        /// <param name="value">The color to set the vertex too.</param>
        /// <param name="ignoreAlpha">Whether to ignore the alpha of the passed in color.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public void SetColor(int i, Color32 value, bool ignoreAlpha = false)
        {
            if (!ignoreAlpha)
            {
                switch (i)
                {
                    case 0:
                        BL_Color = value;
                        break;
                    case 1:
                        TL_Color = value;
                        break;
                    case 2:
                        TR_Color = value;
                        break;
                    case 3:
                        BR_Color = value;
                        break;
                    default: throw new System.ArgumentOutOfRangeException();
                }

                colorsDirty = true;
                alphasDirty = true;
                return;
            }

            ColorOverride colorOverride;
            Color32 currentColor;
            switch (i)
            {
                case 0:
                    colorOverride = modifiers.BL_Color;
                    colorOverride.Override |= ColorOverride.OverrideMode.Color;
                    currentColor = colorOverride.Color;
                    colorOverride.Color = new Color32(value.r, value.g, value.b, currentColor.a);
                    modifiers.BL_Color = colorOverride;
                    break;
                case 1:
                    colorOverride = modifiers.TL_Color;
                    colorOverride.Override |= ColorOverride.OverrideMode.Color;
                    currentColor = colorOverride.Color;
                    colorOverride.Color = new Color32(value.r, value.g, value.b, currentColor.a);
                    modifiers.TL_Color = colorOverride;
                    break;
                case 2:
                    colorOverride = modifiers.TR_Color;
                    colorOverride.Override |= ColorOverride.OverrideMode.Color;
                    currentColor = colorOverride.Color;
                    colorOverride.Color = new Color32(value.r, value.g, value.b, currentColor.a);
                    modifiers.TR_Color = colorOverride;
                    break;
                case 3:
                    colorOverride = modifiers.BR_Color;
                    colorOverride.Override |= ColorOverride.OverrideMode.Color;
                    currentColor = colorOverride.Color;
                    colorOverride.Color = new Color32(value.r, value.g, value.b, currentColor.a);
                    modifiers.BR_Color = colorOverride;
                    break;
                default: throw new System.ArgumentOutOfRangeException();
            }

            colorsDirty = true;
        }

        /// <summary>
        /// Get the alpha of the vertex associated with the index.<br/>
        /// 0 => bottom left<br/>
        /// 1 => top left<br/>
        /// 2 => top right<br/>
        /// 3 => bottom right<br/>
        /// </summary>
        /// <param name="i">The index.</param>
        /// <returns>The alpha of the vertex associated with the index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public byte GetAlpha(int i)
        {
            switch (i)
            {
                case 0: return BL_Color.a;
                case 1: return TL_Color.a;
                case 2: return TR_Color.a;
                case 3: return BR_Color.a;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Set the alpha of the vertex associated with the index.<br/>
        /// 0 => bottom left<br/>
        /// 1 => top left<br/>
        /// 2 => top right<br/>
        /// 3 => bottom right<br/>
        /// </summary>
        /// <param name="i">The index.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public void SetAlpha(int i, float value)
        {
            switch (i)
            {
                case 0:
                    BL_Alpha = (byte)value;
                    break;
                case 1:
                    TL_Alpha = (byte)value;
                    break;
                case 2:
                    TR_Alpha = (byte)value;
                    break;
                case 3:
                    BR_Alpha = (byte)value;
                    break;
                default: throw new System.ArgumentOutOfRangeException();
            }

            alphasDirty = true;
        }

        /// <summary>
        /// Get the UV0 of the vertex associated with the index.<br/>
        /// 0 => bottom left<br/>
        /// 1 => top left<br/>
        /// 2 => top right<br/>
        /// 3 => bottom right<br/>
        /// </summary>
        /// <param name="i">The index.</param>
        /// <returns>The UV0 of the vertex associated with the index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public Vector2 GetUV0(int i)
        {
            switch (i)
            {
                case 0: return BL_UV0;
                case 1: return TL_UV0;
                case 2: return TR_UV0;
                case 3: return BR_UV0;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Set the UV0 of the vertex associated with the index.<br/>
        /// 0 => bottom left<br/>
        /// 1 => top left<br/>
        /// 2 => top right<br/>
        /// 3 => bottom right<br/>
        /// </summary>
        /// <param name="i">The index.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public void SetUV0(int i, Vector2 value)
        {
            switch (i)
            {
                case 0:
                    BL_UV0 = value;
                    break;
                case 1:
                    TL_UV0 = value;
                    break;
                case 2:
                    TR_UV0 = value;
                    break;
                case 3:
                    BR_UV0 = value;
                    break;
                default: throw new System.ArgumentOutOfRangeException();
            }

            uvsDirty = true;
        }

        /// <summary>
        /// Get the UV2 of the vertex associated with the index.<br/>
        /// 0 => bottom left<br/>
        /// 1 => top left<br/>
        /// 2 => top right<br/>
        /// 3 => bottom right<br/>
        /// </summary>
        /// <param name="i">The index.</param>
        /// <returns>The UV2 of the vertex associated with the index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public Vector2 GetUV2(int i)
        {
            switch (i)
            {
                case 0: return BL_UV2;
                case 1: return TL_UV2;
                case 2: return TR_UV2;
                case 3: return BR_UV2;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Set the UV2 of the vertex associated with the index.<br/>
        /// 0 => bottom left<br/>
        /// 1 => top left<br/>
        /// 2 => top right<br/>
        /// 3 => bottom right<br/>
        /// </summary>
        /// <param name="i">The index.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public void SetUV2(int i, Vector2 value)
        {
            switch (i)
            {
                case 0:
                    BL_UV2 = value;
                    break;
                case 1:
                    TL_UV2 = value;
                    break;
                case 2:
                    TR_UV2 = value;
                    break;
                case 3:
                    BR_UV2 = value;
                    break;
                default: throw new System.ArgumentOutOfRangeException();
            }

            uvsDirty = true;
        }

        /// <summary>
        /// Reset the mesh to <see cref="initial"/>.
        /// </summary>
        public void Reset()
        {
            modifiers.ClearModifiers();
        }

        /// <summary>
        /// Reset the vertex colors to <see cref="initial"/>.
        /// </summary>
        public void ResetColors()
        {
            if (!colorsDirty) return;
            if (alphasDirty)
            {
                if (modifiers.BL_Color.OverrideAlpha)
                    modifiers.BL_Color = new ColorOverride(modifiers.BL_Color.Color,
                        modifiers.BL_Color.Override & ~ColorOverride.OverrideMode.Alpha);
                else modifiers.BL_Color = new ColorOverride(initial.GetColor(0), 0);

                if (modifiers.TL_Color.OverrideAlpha)
                    modifiers.TL_Color = new ColorOverride(modifiers.TL_Color.Color,
                        modifiers.BL_Color.Override & ~ColorOverride.OverrideMode.Alpha);
                else modifiers.TL_Color = new ColorOverride(initial.GetColor(1), 0);

                if (modifiers.TR_Color.OverrideAlpha)
                    modifiers.TR_Color = new ColorOverride(modifiers.TR_Color.Color,
                        modifiers.BL_Color.Override & ~ColorOverride.OverrideMode.Alpha);
                else modifiers.TR_Color = new ColorOverride(initial.GetColor(2), 0);

                if (modifiers.BR_Color.OverrideAlpha)
                    modifiers.BR_Color = new ColorOverride(modifiers.BR_Color.Color,
                        modifiers.BL_Color.Override & ~ColorOverride.OverrideMode.Alpha);
                else modifiers.BR_Color = new ColorOverride(initial.GetColor(3), 0);
            }
            else
            {
                modifiers.BL_Color = new ColorOverride(initial.GetColor(0), 0);
                modifiers.TL_Color = new ColorOverride(initial.GetColor(1), 0);
                modifiers.TR_Color = new ColorOverride(initial.GetColor(2), 0);
                modifiers.BR_Color = new ColorOverride(initial.GetColor(3), 0);
            }

            colorsDirty = false;
        }

        public void ResetAlphas()
        {
            if (!alphasDirty) return;

            if (colorsDirty)
            {
                if (modifiers.BL_Color.OverrideColor)
                    modifiers.BL_Color = new ColorOverride(modifiers.BL_Color.Color,
                        modifiers.BL_Color.Override & ~ColorOverride.OverrideMode.Color);
                else modifiers.BL_Color = new ColorOverride(initial.GetColor(0), 0);
                
                if (modifiers.TL_Color.OverrideColor)
                    modifiers.TL_Color = new ColorOverride(modifiers.TL_Color.Color,
                        modifiers.BL_Color.Override & ~ColorOverride.OverrideMode.Color);
                else modifiers.TL_Color = new ColorOverride(initial.GetColor(1), 0);
                
                if (modifiers.TR_Color.OverrideColor)
                    modifiers.TR_Color = new ColorOverride(modifiers.TR_Color.Color,
                        modifiers.BL_Color.Override & ~ColorOverride.OverrideMode.Color);
                else modifiers.TR_Color = new ColorOverride(initial.GetColor(2), 0);
                
                if (modifiers.BR_Color.OverrideColor)
                    modifiers.BR_Color = new ColorOverride(modifiers.BR_Color.Color,
                        modifiers.BL_Color.Override & ~ColorOverride.OverrideMode.Color);
                else modifiers.BR_Color = new ColorOverride(initial.GetColor(1), 0);
            }
            else
            {
                modifiers.BL_Color = new ColorOverride(initial.GetColor(0), 0);
                modifiers.TL_Color = new ColorOverride(initial.GetColor(1), 0);
                modifiers.TR_Color = new ColorOverride(initial.GetColor(2), 0);
                modifiers.BR_Color = new ColorOverride(initial.GetColor(3), 0);
            }

            alphasDirty = false;
        }

        /// <summary>
        /// Reset the vertices to <see cref="initial"/>.
        /// </summary>
        public void ResetPositions()
        {
            if (!positionsDirty) return;
            modifiers.ClearModifiers(TMPMeshModifiers.ModifierFlags.Deltas);
            positionsDirty = false;
        }

        /// <summary>
        /// Reset the UVs to <see cref="initial"/>.
        /// </summary>
        public void ResetUVs()
        {
            if (!uvsDirty) return;
            modifiers.ClearModifiers(TMPMeshModifiers.ModifierFlags.UVs);
            uvsDirty = false;
        }
    }
}