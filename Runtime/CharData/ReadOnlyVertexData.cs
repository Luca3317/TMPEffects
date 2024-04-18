using TMPro;
using UnityEngine;

namespace TMPEffects.CharacterData
{
    /// <summary>
    /// Holds data about a character's mesh.
    /// </summary>
    public class ReadOnlyVertexData
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
        /// Get or set the color of the bottom left vertex.<br/>
        /// Note that this will mark both colors and alphas as dirty.<br/>
        /// Use <see cref="SetColor(int, Color32, bool)"/> if you want to only set color.
        /// </summary>
        public Color32 BL_Color
        {
            get => vertex_BL.color;
        }
        /// <summary>
        /// Get or set the color of the top left vertex.<br/>
        /// Note that this will mark both colors and alphas as dirty.<br/>
        /// Use <see cref="SetColor(int, Color32, bool)"/> if you want to only set color.
        /// </summary>
        public Color32 TL_Color
        {
            get => vertex_TL.color;
        }
        /// <summary>
        /// Get or set the color of the top right vertex.<br/>
        /// Note that this will mark both colors and alphas as dirty.<br/>
        /// Use <see cref="SetColor(int, Color32, bool)"/> if you want to only set color.
        /// </summary>
        public Color32 TR_Color
        {
            get => vertex_TR.color;
        }
        /// <summary>
        /// Get or set the color of the bottom right vertex.<br/>
        /// Note that this will mark both colors and alphas as dirty.<br/>
        /// Use <see cref="SetColor(int, Color32, bool)"/> if you want to only set color.
        /// </summary>
        public Color32 BR_Color
        {
            get => vertex_BR.color;
        }

        /// <summary>
        /// Get or set the alpha of the bottom left vertex.
        /// </summary>
        public byte BL_Alpha
        {
            get => vertex_BL.color.a;
        }
        /// <summary>
        /// Get or set the alpha of the top left vertex.
        /// </summary>
        public byte TL_Alpha
        {
            get => vertex_TL.color.a;
        }
        /// <summary>
        /// Get or set the alpha of the top right vertex.
        /// </summary>
        public byte TR_Alpha
        {
            get => vertex_TR.color.a;
        }
        /// <summary>
        /// Get or set the alpha of the bottom right vertex.
        /// </summary>
        public byte BR_Alpha
        {
            get => vertex_BR.color.a;
        }

        /// <summary>
        /// Get or set the position of the bottom left vertex.
        /// </summary>
        public Vector3 BL_Position
        {
            get => vertex_BL.position;
        }
        /// <summary>
        /// Get or set the position of the top left vertex.
        /// </summary>
        public Vector3 TL_Position
        {
            get => vertex_TL.position;
        }
        /// <summary>
        /// Get or set the position of the top right vertex.
        /// </summary>
        public Vector3 TR_Position
        {
            get => vertex_TR.position;
        }
        /// <summary>
        /// Get or set the position of the bottom right vertex.
        /// </summary>
        public Vector3 BR_Position
        {
            get => vertex_BR.position;
        }

        /// <summary>
        /// Get or set the UV0 of the bottom left vertex.
        /// </summary>
        public Vector3 BL_UV0
        {
            get => vertex_BL.uv;
        }
        /// <summary>
        /// Get or set the UV0 of the bottom right vertex.
        /// </summary>
        public Vector3 TL_UV0
        {
            get => vertex_TL.uv;
        }
        /// <summary>
        /// Get or set the UV0 of the top right vertex.
        /// </summary>
        public Vector3 TR_UV0
        {
            get => vertex_TR.uv;
        }
        /// <summary>
        /// Get or set the UV0 of the bottom right vertex.
        /// </summary>
        public Vector3 BR_UV0
        {
            get => vertex_BR.uv;
        }

        /// <summary>
        /// Get or set the UV2 of the bottom left vertex.
        /// </summary>
        public Vector3 BL_UV2
        {
            get => vertex_BL.uv2;
        }
        /// <summary>
        /// Get or set the UV2 of the bottom right vertex.
        /// </summary>
        public Vector3 TL_UV2
        {
            get => vertex_TL.uv2;
        }
        /// <summary>
        /// Get or set the UV2 of the top right vertex.
        /// </summary>
        public Vector3 TR_UV2
        {
            get => vertex_TR.uv2;
        }
        /// <summary>
        /// Get or set the UV2 of the bottom right vertex.
        /// </summary>
        public Vector3 BR_UV2
        {
            get => vertex_BR.uv2;
        }

        public ReadOnlyVertexData(TMP_Vertex bl, TMP_Vertex tl, TMP_Vertex tr, TMP_Vertex br)
        {
            this.vertex_BL = bl;
            this.vertex_TL = tl;
            this.vertex_TR = tr;
            this.vertex_BR = br;
        }

        public ReadOnlyVertexData(TMP_CharacterInfo info)
        {
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
    }
}
