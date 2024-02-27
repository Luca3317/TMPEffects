using TMPro;
using UnityEngine;

namespace TMPEffects.Components.CharacterData
{
    /// <summary>
    /// Holds readonly data about a character's mesh.
    /// </summary>
    public struct ReadOnlyVertexData
    {
        /// <summary>
        /// The bottom left vertex. Index = 0
        /// </summary>
        public TMP_Vertex vertex_BL => _vertex_BL;
        /// <summary>
        /// The top left vertex. Index = 1
        /// </summary>
        public TMP_Vertex vertex_TL => _vertex_TL;
        /// <summary>
        /// The top right vertex. Index = 2
        /// </summary>
        public TMP_Vertex vertex_TR => _vertex_TR;
        /// <summary>
        /// The bottom right vertex. Index = 3
        /// </summary>
        public TMP_Vertex vertex_BR => _vertex_BR;

        private TMP_Vertex _vertex_TL;
        private TMP_Vertex _vertex_TR;
        private TMP_Vertex _vertex_BR;
        private TMP_Vertex _vertex_BL;

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
                    case 0: return _vertex_BL;
                    case 1: return _vertex_TL;
                    case 2: return _vertex_TR;
                    case 3: return _vertex_BR;
                    default: throw new System.ArgumentOutOfRangeException();
                }
            }
        }

        public ReadOnlyVertexData(TMP_Vertex bl, TMP_Vertex tl, TMP_Vertex tr, TMP_Vertex br)
        {
            this._vertex_BL = bl;
            this._vertex_TL = tl;
            this._vertex_TR = tr;
            this._vertex_BR = br;
        }

        public ReadOnlyVertexData(TMP_CharacterInfo info)
        {
            this._vertex_BL = info.vertex_BL;
            this._vertex_TL = info.vertex_TL;
            this._vertex_TR = info.vertex_TR;
            this._vertex_BR = info.vertex_BR;
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
                case 0: return _vertex_BL.position;
                case 1: return _vertex_TL.position;
                case 2: return _vertex_TR.position;
                case 3: return _vertex_BR.position;
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
                case 0: return _vertex_BL.color;
                case 1: return _vertex_TL.color;
                case 2: return _vertex_TR.color;
                case 3: return _vertex_BR.color;
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
                case 0: return _vertex_BL.uv;
                case 1: return _vertex_TL.uv;
                case 2: return _vertex_TR.uv;
                case 3: return _vertex_BR.uv;
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
                case 0: return _vertex_BL.uv2;
                case 1: return _vertex_TL.uv2;
                case 2: return _vertex_TR.uv2;
                case 3: return _vertex_BR.uv2;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }
    }
}
