using TMPro;
using UnityEngine;

namespace TMPEffects.CharacterData
{
    /// <summary>
    /// Holds data about a character's mesh.
    /// </summary>
    public struct VertexData
    {
        /// <summary>
        /// The bottom left vertex. Index = 0
        /// </summary>
        public TMP_Vertex vertex_BL;
        /// <summary>
        /// The top left vertex. Index = 1
        /// </summary>
        public TMP_Vertex vertex_TL;
        /// <summary>
        /// The top right vertex. Index = 2
        /// </summary>
        public TMP_Vertex vertex_TR;
        /// <summary>
        /// The bottom right vertex. Index = 3
        /// </summary>
        public TMP_Vertex vertex_BR;

        /// <summary>
        /// The initial vertex of the character.
        /// </summary>
        public readonly ReadOnlyVertexData initial;

        /// <summary>
        /// Whether the vertices have been manipulated.
        /// </summary>
        public bool verticesDirty { get; private set; }
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
        /// Index based access of the vertices.<br/>
        /// 0 => bottom left<br/>
        /// 1 => top left<br/>
        /// 2 => top right<br/>
        /// 3 => bottom right<br/>
        /// </summary>
        /// <param name="i">The index.</param>
        /// <returns>The vertex associated with the index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public TMP_Vertex this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return vertex_BL;
                    case 1: return vertex_TL;
                    case 2: return vertex_TR;
                    case 3: return vertex_BR;
                    default: throw new System.ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (i)
                {
                    case 0: vertex_BL = value; break;
                    case 1: vertex_TL = value; break;
                    case 2: vertex_TR = value; break;
                    case 3: vertex_BR = value; break;
                    default: throw new System.ArgumentOutOfRangeException();
                }

                verticesDirty = true;
            }
        }

        public VertexData(TMP_Vertex bl, TMP_Vertex tl, TMP_Vertex tr, TMP_Vertex br)
        {
            verticesDirty = false;
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
            verticesDirty = false;
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
        public Vector3 GetVertex(int i)
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
        public void SetVertex(int i, Vector3 value)
        {
            switch (i)
            {
                case 0: vertex_BL.position = value; break;
                case 1: vertex_TL.position = value; break;
                case 2: vertex_TR.position = value; break;
                case 3: vertex_BR.position = value; break;
                default: throw new System.ArgumentOutOfRangeException();
            }

            verticesDirty = true;
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
            ResetVertices();
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
        public void ResetVertices()
        {
            if (!verticesDirty) return;
            vertex_BL.position = initial.GetVertex(0);
            vertex_TL.position = initial.GetVertex(1);
            vertex_TR.position = initial.GetVertex(2);
            vertex_BR.position = initial.GetVertex(3);
            verticesDirty = false;
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
