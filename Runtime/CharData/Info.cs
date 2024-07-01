using TMPEffects.CharacterData;
using TMPro;
using UnityEngine;

namespace TMPEffects.CharacterData
{
    public partial class CharData
    {
        /// <summary>
        /// Holds a selection of data of <see cref="TMP_CharacterInfo"/>, as well as some data about the initial mesh of the character.
        /// </summary>
        public struct Info
        {
            /// <summary>
            /// The index of the character within the source text.
            /// </summary>
            public readonly int index;

            /// <summary>
            /// The first index of the word this character belongs to.
            /// </summary>
            public readonly int wordFirstIndex;
            /// <summary>
            /// The last index of the word this character belongs to.
            /// </summary>
            public readonly int wordLastIndex;
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
            /// The inde
            /// </summary>
            public readonly int wordNumber;

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
            /// The sprite asset used for this sprite.
            /// </summary>
            public readonly TMP_SpriteAsset spriteAsset;

            public readonly TMP_TextElementType elementType;

            public readonly float origin;

            /// <summary>
            /// The reference scale of this character.
            /// </summary>
            public readonly float referenceScale;

            internal Info(int index, TMP_CharacterInfo cInfo, int wordIndex)
            {
                this.index = index; /*cInfo.index;*/
                isVisible = cInfo.isVisible;

                this.wordNumber = wordIndex;
                wordFirstIndex = -1;
                wordLastIndex = -1;
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
                fontAsset = cInfo.fontAsset;

#if UNITY_2023_2_OR_NEWER

            if (cInfo.elementType == TMP_TextElementType.Sprite)
            {
                TMP_SpriteCharacter sprite = (TMP_SpriteCharacter)cInfo.textElement;
                spriteAsset = sprite.textAsset as TMP_SpriteAsset;
            }
            else
            {
                spriteAsset = null;
            }
#else
                spriteAsset = cInfo.spriteAsset;
#endif

                elementType = cInfo.elementType;
                origin = cInfo.origin;
            }

            public Info(int index, TMP_CharacterInfo cInfo, int wordIndex, TMP_WordInfo wInfo) : this(index, cInfo, wordIndex)
            {
                wordFirstIndex = wInfo.firstCharacterIndex;
                wordLastIndex = wInfo.lastCharacterIndex;
                wordLen = wInfo.characterCount;
            }
        }
    }

}