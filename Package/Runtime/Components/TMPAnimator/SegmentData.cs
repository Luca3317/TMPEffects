using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;
using TMPEffects.Tags;
using TMPEffects.CharacterData;
using System.Threading;

namespace TMPEffects.Components.Animator
{
    /// <summary>
    /// Provides information about text segment.
    /// </summary>
    public interface ITMPSegmentData
    {
        /// <summary>
        /// The first index of the segment within the containing text.
        /// </summary>
        public int StartIndex { get; }
        /// <summary>
        /// The length of the animation segment.
        /// </summary>
        public int Length { get; }
        /// <summary>
        /// The last index of the segment within the containing text.
        /// </summary>
        public int EndIndex { get; }
        
        /// <summary>
        /// The <see cref="CharData.Info"/> of each character in the segment.
        /// </summary>
        public IEnumerable<CharData.Info> CharInfo { get; }
        /// <summary>
        /// Get a specific <see cref="CharData.Info"/> from a segment index.
        /// </summary>
        /// <param name="segmentIndex"></param>
        /// <returns>The <see cref="CharData.Info"/> associated with the segment index.</returns>
        public CharData.Info GetCharInfo(int segmentIndex);

        /// <summary>
        /// Convert a text index to the index within the segment.
        /// </summary>
        /// <param name="index">The text index to convert.</param>
        /// <returns>The segment index.</returns>
        public int IndexToSegmentIndex(int index);
        /// <summary>
        /// Get the segment index of the given <see cref="CharData"/>.
        /// </summary>
        /// <param name="cData"></param>
        /// <returns></returns>
        public int SegmentIndexOf(CharData cData);
    }
    
    /// <summary>
    /// To be used with <see cref="TMPAnimator"/> and its animations.<br/>
    /// Contains data about a given animation segment.<br/>
    /// Only does some basic info parsing. For more specialised information, e.g. word-count,
    /// you can manually derive the info from <see cref="CharInfo"/>.
    /// </summary>
    public readonly struct SegmentData : ITMPSegmentData
    {
        public int StartIndex { get; }

        public int Length { get; }
        
        public int EndIndex { get; }

        /// <summary>
        /// The effective length of the animation segment; i.e. the length of the segment from <see cref="firstAnimationIndex"/> to <see cref="lastAnimationIndex"/>.
        /// </summary>
        public readonly int effectiveLength;

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


        public IEnumerable<CharData.Info> CharInfo
        {
            get
            {
                for (int i = StartIndex; i < EndIndex; i++)
                {
                    yield return charDatas[i].info;
                }
            }   
        }

        public CharData.Info GetCharInfo(int segmentIndex)
        {
            if (segmentIndex > Length) throw new ArgumentOutOfRangeException(nameof(segmentIndex));
            return charDatas[segmentIndex + StartIndex].info;
        }

        private readonly IList<CharData> charDatas;
        
        public int IndexToSegmentIndex(int index)
        {
            index -= StartIndex;
            if (index < 0 || index >= Length) return -1;
            else return index;
        }

        public int SegmentIndexOf(CharData cData)
        {
            return IndexToSegmentIndex(cData.info.index);
        }

        public SegmentData(TMPEffectTagIndices indices, IList<CharData> cData, Predicate<char> animates)
        {
            StartIndex = indices.StartIndex;
            Length = indices.Length;
            EndIndex = indices.EndIndex;
            firstVisibleIndex = -1;
            lastVisibleIndex = -1;
            firstAnimationIndex = -1;
            lastAnimationIndex = -1;

            // Clamp needed for when text ends with tags; will cause (startIndex + length)
            // to be equal to cData.Count + 1
            int count = Mathf.Min(cData.Count, StartIndex + Length);

            for (int i = StartIndex; i < count; i++)
            {
                if (!cData[i].info.isVisible) continue;
                if (firstVisibleIndex == -1) firstVisibleIndex = i;
                if (animates(cData[i].info.character))
                {
                    firstAnimationIndex = i;
                    break;
                }
            }

            for (int i = count - 1; i >= StartIndex; i--)
            {
                if (!cData[i].info.isVisible) continue;
                if (lastVisibleIndex == -1) lastVisibleIndex = i;
                if (animates(cData[i].info.character))
                {
                    lastAnimationIndex = i;
                    break;
                }
            }

            effectiveLength = lastAnimationIndex - firstAnimationIndex + 1;

            charDatas = cData;
        }
    }
}