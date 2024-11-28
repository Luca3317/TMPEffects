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
    public interface ITMPSegmentData
    {
        public int StartIndex { get; }
        public int Length { get; }
        public int EndIndex { get; }
        
        public IReadOnlyDictionary<int, CharData.Info> CharInfo { get; }

        public int IndexToSegmentIndex(int index);
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
        /// <summary>
        /// The first index of the segment within the containing text.
        /// </summary>
        public int StartIndex { get; }

        /// <summary>
        /// The length of the animation segment.
        /// </summary>
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

        public IReadOnlyDictionary<int, CharData.Info> CharInfo { get; }
        
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
            var dict = new Dictionary<int, CharData.Info>();
            for (int i = StartIndex; i < count; i++)
            {
                var cd = cData[i];
                dict.Add(cd.info.index, cd.info);
            }
            CharInfo = new ReadOnlyDictionary<int, CharData.Info>(dict);
        }
    }
}