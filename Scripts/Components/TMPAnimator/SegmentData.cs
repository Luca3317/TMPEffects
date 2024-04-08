using System;
using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Tags;
using TMPEffects.CharacterData;

namespace TMPEffects.Components.Animator
{
    /// <summary>
    /// To be used with <see cref="TMPAnimator"/> and its animations.<br/>
    /// Contains data about a given animation segment.
    /// </summary>
    public struct SegmentData
    {
        /// <summary>
        /// The first index of the segment within the containing text.
        /// </summary>
        public readonly int startIndex;
        /// <summary>
        /// The length of the animation segment.
        /// </summary>
        public readonly int length;

        /// <summary>
        /// The index of the first visible character (i.e. non-whitespace character).
        /// </summary>
        public readonly int firstVisibleIndex;
        /// <summary>
        /// The index of the last visible character (i.e. non-whitespace character).
        /// </summary>
        public readonly int lastVisibleIndex;

        /// <summary>
        /// The index of the first character that is relevant to the <see cref="TMPAnimator"/> and will be animated.
        /// </summary>
        public readonly int firstAnimationIndex;
        /// <summary>
        /// The index of the last character that is relevant to the <see cref="TMPAnimator"/> and will be animated.
        /// </summary>
        public readonly int lastAnimationIndex;

        public readonly Vector3 max;
        public readonly Vector3 min;

        public int IndexToSegmentIndex(int index)
        {
            index -= startIndex;
            if (index < 0 || index >= length) return -1;
            else return index;
        }

        public int SegmentIndexOf(CharData cData)
        {
            return IndexToSegmentIndex(cData.info.index);
        }

        internal SegmentData(EffectTagIndices indices, IList<CharData> cData, Predicate<char> animates)
        {
            startIndex = indices.StartIndex;
            length = indices.Length;
            firstVisibleIndex = -1;
            lastVisibleIndex = -1;
            firstAnimationIndex = -1;
            lastAnimationIndex = -1;

            max = Vector3.negativeInfinity;
            min = Vector3.positiveInfinity;
            Vector3 leftTop, bottomRight;

            // Clamp needed for when text ends with tags; will cause (startIndex + length)
            // to be equal to cData.Count + 1
            int count = Mathf.Min(cData.Count, startIndex + length);
            for (int i = startIndex; i < count; i++)
            {
                if (!cData[i].info.isVisible) continue;

                leftTop = cData[i].initialMesh.TL_Position;
                bottomRight = cData[i].initialMesh.BR_Position;
                max = new Vector3(Mathf.Max(max.x, leftTop.x, bottomRight.x), Mathf.Max(max.y, leftTop.y, bottomRight.y), Mathf.Max(max.z, leftTop.z, bottomRight.z));
                min = new Vector3(Mathf.Min(min.x, leftTop.x, bottomRight.x), Mathf.Min(min.y, leftTop.y, bottomRight.y), Mathf.Min(min.z, leftTop.z, bottomRight.z));

                if (firstVisibleIndex == -1) firstVisibleIndex = i;
                lastVisibleIndex = i;

                if (animates(cData[i].info.character))
                {
                    if (firstAnimationIndex == -1) firstAnimationIndex = i;
                    lastAnimationIndex = i;
                } 
            }
        }
    }
}
 