using TMPro;
using UnityEngine;

namespace TMPEffects.CharacterData
{
    /// <summary>
    /// Holds data about a character's mesh.
    /// </summary>
    public class VertexData
    {
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
        /// The initial vertex of the character.
        /// </summary>
        public readonly ReadOnlyVertexData initial;

        /// <summary>
        /// Whether the positions have been manipulated.
        /// </summary>
        public bool positionsDirty{ get; private set; }
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
            get => vertex_BL.color;
            set
            {
                vertex_BL.color = value;
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
            get => vertex_TL.color;
            set
            {
                vertex_TL.color = value;
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
            get => vertex_TR.color;
            set
            {
                vertex_TR.color = value;
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
            get => vertex_BR.color;
            set
            {
                vertex_BR.color = value;
                colorsDirty = true;
                alphasDirty = true;
            }
        }

        /// <summary>
        /// Get or set the alpha of the bottom left vertex.
        /// </summary>
        public byte BL_Alpha
        {
            get => vertex_BL.color.a;
            set
            {
                vertex_BL.color.a = value;
                alphasDirty = true;
            }
        }
        /// <summary>
        /// Get or set the alpha of the top left vertex.
        /// </summary>
        public byte TL_Alpha
        {
            get => vertex_TL.color.a;
            set
            {
                vertex_TL.color.a = value;
                alphasDirty = true;
            }
        }
        /// <summary>
        /// Get or set the alpha of the top right vertex.
        /// </summary>
        public byte TR_Alpha
        {
            get => vertex_TR.color.a;
            set
            {
                vertex_TR.color.a = value;
                alphasDirty = true;
            }
        }
        /// <summary>
        /// Get or set the alpha of the bottom right vertex.
        /// </summary>
        public byte BR_Alpha
        {
            get => vertex_BR.color.a;
            set
            {
                vertex_BR.color.a = value;
                alphasDirty = true;
            }
        }

        /// <summary>
        /// Get or set the position of the bottom left vertex.
        /// </summary>
        public Vector3 BL_Position
        {
            get => vertex_BL.position;
            set
            {
                vertex_BL.position = value;
                positionsDirty = true;
            }
        }
        /// <summary>
        /// Get or set the position of the top left vertex.
        /// </summary>
        public Vector3 TL_Position
        {
            get => vertex_TL.position;
            set
            {
                vertex_TL.position = value;
                positionsDirty = true;
            }
        }
        /// <summary>
        /// Get or set the position of the top right vertex.
        /// </summary>
        public Vector3 TR_Position
        {
            get => vertex_TR.position;
            set
            {
                vertex_TR.position = value;
                positionsDirty = true;
            }
        }
        /// <summary>
        /// Get or set the position of the bottom right vertex.
        /// </summary>
        public Vector3 BR_Position
        {
            get => vertex_BR.position;
            set
            {
                vertex_BR.position = value;
                positionsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the UV0 of the bottom left vertex.
        /// </summary>
        public Vector3 BL_UV0
        {
            get => vertex_BL.uv;
            set
            {
                vertex_BL.uv = value;
                uvsDirty = true;
            }
        }
        /// <summary>
        /// Get or set the UV0 of the bottom right vertex.
        /// </summary>
        public Vector3 TL_UV0
        {
            get => vertex_TL.uv;
            set
            {
                vertex_TL.uv = value;
                uvsDirty = true;
            }
        }
        /// <summary>
        /// Get or set the UV0 of the top right vertex.
        /// </summary>
        public Vector3 TR_UV0
        {
            get => vertex_TR.uv;
            set
            {
                vertex_TR.uv = value;
                uvsDirty = true;
            }
        }
        /// <summary>
        /// Get or set the UV0 of the bottom right vertex.
        /// </summary>
        public Vector3 BR_UV0
        {
            get => vertex_BR.uv;
            set
            {
                vertex_BR.uv = value;
                uvsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the UV2 of the bottom left vertex.
        /// </summary>
        public Vector3 BL_UV2
        {
            get => vertex_BL.uv2;
            set
            {
                vertex_BL.uv2 = value;
                uvsDirty = true;
            }
        }
        /// <summary>
        /// Get or set the UV2 of the bottom right vertex.
        /// </summary>
        public Vector3 TL_UV2
        {
            get => vertex_TL.uv2;
            set
            {
                vertex_TL.uv2 = value;
                uvsDirty = true;
            }
        }
        /// <summary>
        /// Get or set the UV2 of the top right vertex.
        /// </summary>
        public Vector3 TR_UV2
        {
            get => vertex_TR.uv2;
            set
            {
                vertex_TR.uv2 = value;
                uvsDirty = true;
            }
        }
        /// <summary>
        /// Get or set the UV2 of the bottom right vertex.
        /// </summary>
        public Vector3 BR_UV2
        {
            get => vertex_BR.uv2;
            set
            {
                vertex_BR.uv2 = value;
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
                case 0: return vertex_BL.position;
                case 1: return vertex_TL.position;
                case 2: return vertex_TR.position;
                case 3: return vertex_BR.position;
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
                case 0: vertex_BL.position = value; break;
                case 1: vertex_TL.position = value; break;
                case 2: vertex_TR.position = value; break;
                case 3: vertex_BR.position = value; break;
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
                case 0: return vertex_BL.color;
                case 1: return vertex_TL.color;
                case 2: return vertex_TR.color;
                case 3: return vertex_BR.color;
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
                    case 0: vertex_BL.color = value; break;
                    case 1: vertex_TL.color = value; break;
                    case 2: vertex_TR.color = value; break;
                    case 3: vertex_BR.color = value; break;
                    default: throw new System.ArgumentOutOfRangeException();
                }

                colorsDirty = true;
                alphasDirty = true;
                return;
            }

            value = new Color32(value.r, value.g, value.b, initial.GetAlpha(i));
            switch (i)
            {
                case 0: vertex_BL.color = value; break;
                case 1: vertex_TL.color = value; break;
                case 2: vertex_TR.color = value; break;
                case 3: vertex_BR.color = value; break;
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
                case 0: return vertex_BL.color.a;
                case 1: return vertex_TL.color.a;
                case 2: return vertex_TR.color.a;
                case 3: return vertex_BR.color.a;
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
                case 0: vertex_BL.color.a = (byte)value; break;
                case 1: vertex_TL.color.a = (byte)value; break;
                case 2: vertex_TR.color.a = (byte)value; break;
                case 3: vertex_BR.color.a = (byte)value; break;
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
                case 0: return vertex_BL.uv;
                case 1: return vertex_TL.uv;
                case 2: return vertex_TR.uv;
                case 3: return vertex_BR.uv;
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
                case 0: vertex_BL.uv = value; break;
                case 1: vertex_TL.uv = value; break;
                case 2: vertex_TR.uv = value; break;
                case 3: vertex_BR.uv = value; break;
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
                case 0: return vertex_BL.uv2;
                case 1: return vertex_TL.uv2;
                case 2: return vertex_TR.uv2;
                case 3: return vertex_BR.uv2;
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
                case 0: vertex_BL.uv2 = value; break;
                case 1: vertex_TL.uv2 = value; break;
                case 2: vertex_TR.uv2 = value; break;
                case 3: vertex_BR.uv2 = value; break;
                default: throw new System.ArgumentOutOfRangeException();
            }

            uvsDirty = true;
        }

        /// <summary>
        /// Reset the mesh to <see cref="initial"/>.
        /// </summary>
        public void Reset()
        {
            ResetColors();
            ResetAlphas();
            ResetPositions();
            ResetUVs();
        }

        /// <summary>
        /// Reset the vertex colors to <see cref="initial"/>.
        /// </summary>
        public void ResetColors()
        {
            if (!colorsDirty) return;
            vertex_BL.color = initial.GetColor(0);
            vertex_TL.color = initial.GetColor(1);
            vertex_TR.color = initial.GetColor(2);
            vertex_BR.color = initial.GetColor(3);
            colorsDirty = false;
        }

        public void ResetAlphas()
        {
            if (!alphasDirty) return;
            vertex_BL.color.a = initial.GetColor(0).a;
            vertex_TL.color.a = initial.GetColor(1).a;
            vertex_TR.color.a = initial.GetColor(2).a;
            vertex_BR.color.a = initial.GetColor(3).a;
            alphasDirty = false;
        }

        /// <summary>
        /// Reset the vertices to <see cref="initial"/>.
        /// </summary>
        public void ResetPositions()
        {
            if (!positionsDirty) return;
            vertex_BL.position = initial.GetPosition(0);
            vertex_TL.position = initial.GetPosition(1);
            vertex_TR.position = initial.GetPosition(2);
            vertex_BR.position = initial.GetPosition(3);
            positionsDirty = false;
        }

        /// <summary>
        /// Reset the UVs to <see cref="initial"/>.
        /// </summary>
        public void ResetUVs()
        {
            if (!uvsDirty) return;
            vertex_BL.uv = initial.GetUV0(0);
            vertex_TL.uv = initial.GetUV0(1);
            vertex_TR.uv = initial.GetUV0(2);
            vertex_BR.uv = initial.GetUV0(3);

            vertex_BL.uv2 = initial.GetUV2(0);
            vertex_TL.uv2 = initial.GetUV2(1);
            vertex_TR.uv2 = initial.GetUV2(2);
            vertex_BR.uv2 = initial.GetUV2(3);
            uvsDirty = false;
        }
    }
}
