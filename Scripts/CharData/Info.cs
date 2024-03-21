using TMPEffects.CharacterData;
using TMPro;
using UnityEngine;

namespace TMPEffects.CharacterData
{
    /// <summary>
    /// Holds a selection of data of <see cref="TMP_CharacterInfo"/>, as well as some data about the initial mesh of the character.
    /// </summary>
    public struct Info
    {
        /// <summary>
        /// The initial scale of the character.
        /// </summary>
        public Vector3 initialScale => defaultScale;
        /// <summary>
        /// The default scale.
        /// </summary>
        public static readonly Vector3 defaultScale = new Vector3(1, 1, 1);

        /// <summary>
        /// The index of the character within the source text.
        /// </summary>
        public readonly int index;

        /// <summary>
        /// The first index of the word this character belongs to.
        /// </summary>
        public readonly int wordFirstIndex;
        /// <summary>
        /// The length of the word this character belongs to.
        /// </summary>
        public readonly int wordLen;
        /// <summary>
        /// The color of this character.
        /// </summary>
        public readonly Color32 color;

        /// <summary>
        /// The point size.
        /// </summary>
        public readonly float pointSize;

        /// <summary>
        /// The actual character.
        /// </summary>
        public readonly char character;

        /// <summary>
        /// Whether the character is inherently visible; i.e. is no whitespace.
        /// </summary>
        public readonly bool isVisible;

        /// <summary>
        /// The line number of the line this character belongs to.
        /// </summary>
        public readonly int lineNumber;
        /// <summary>
        /// The page number of the page this character belongs to.
        /// </summary>
        public readonly int pageNumber;

        /// <summary>
        /// The baseline of the character.
        /// </summary>
        public readonly float baseLine;
        /// <summary>
        /// The ascender of the character.
        /// </summary>
        public readonly float ascender;
        /// <summary>
        /// The descender of the character.
        /// </summary>
        public readonly float descender;
        /// <summary>
        /// The xAdvance factor of the character.
        /// </summary>
        public readonly float xAdvance;

        /// <summary>
        /// The font asset used for this character.
        /// </summary>
        public readonly TMP_FontAsset fontAsset;

        /// <summary>
        /// The initial position of this character.
        /// </summary>
        public readonly Vector3 initialPosition;
        /// <summary>
        /// The initial rotation of this character.
        /// </summary>
        public readonly Quaternion initialRotation;
        /// <summary>
        /// The reference scale of this character.
        /// </summary>
        public readonly float referenceScale;

        internal Info(int index, TMP_CharacterInfo cInfo, VertexData mesh)
        {
            this.index = index; /*cInfo.index;*/
            isVisible = cInfo.isVisible;

            wordFirstIndex = -1;
            wordLen = -1;
            color = cInfo.color;

            lineNumber = cInfo.lineNumber;
            pageNumber = cInfo.pageNumber;

            pointSize = cInfo.pointSize;
            character = cInfo.character;

            baseLine = cInfo.baseLine;
            ascender = cInfo.ascender;
            descender = cInfo.descender;
            xAdvance = cInfo.xAdvance;

            referenceScale = cInfo.scale;
            initialPosition = default;
            initialRotation = Quaternion.identity;

            fontAsset = cInfo.fontAsset;

            initialPosition = GetCenter(in mesh.initial);
        }

        public Info(int index, TMP_CharacterInfo cInfo, TMP_WordInfo wInfo, VertexData mesh) : this(index, cInfo, mesh)
        {
            wordFirstIndex = wInfo.firstCharacterIndex;
            wordLen = wInfo.characterCount;
        }

        private Vector3 GetCenter(in ReadOnlyVertexData data)
        {
            Vector3 center = Vector3.zero;
            for (int i = 0; i < 4; i++)
            {
                center += data.GetVertex(i);
            }
            return center / 4;
        }
    }
}