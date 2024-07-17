using System;
using System.Collections.Generic;

namespace TMPEffects.Tags
{
    /// <summary>
    /// The indices of an <see cref="TMPEffectTag"/>.<br/>
    /// The indices can be regarded as a half-open interval of [<see cref="StartIndex"/>, <see cref="EndIndex"/>).<br/>
    /// For example, an instance with <see cref="StartIndex"/> == 5 and <see cref="EndIndex"/> == 10 "contains" the indices 5, 6, 7, 8 and 9.
    /// </summary>
    public struct TMPEffectTagIndices : IComparable<TMPEffectTagIndices>, IEquatable<TMPEffectTagIndices>
    {
        /// <summary>
        /// The (inclusive) start index of the tag.
        /// </summary>
        public int StartIndex => startIndex;
        /// <summary>
        /// The (exclusive) end index of the tag.
        /// </summary>
        public int EndIndex => endIndex;
        /// <summary>
        /// The order at the start index.<br/>
        /// If there are multiple tags with the same start index, this will define their order:<br/>
        /// tags with lower order come first, tags with higher index come later.<br/>
        /// Note that the order may have gaps (e.g. there are three tags at a given index, with order -5, 4 and 10 respectively).
        /// </summary>
        public int OrderAtIndex => orderAtIndex;

        /// <summary>
        /// Whether the tag is open, meaning it is never closed and therefore does not have a defined end index;<br/>
        /// the tag's index interval ranges from the <see cref="StartIndex"/> to the end of whatever text it is applied to.
        /// </summary>
        public bool IsOpen => endIndex == -1;
        /// <summary>
        /// The length of the interval, meaning the difference between <see cref="EndIndex"/> and <see cref="StartIndex"/>.<br/>
        /// Is -1 if the tag is open.
        /// </summary>
        public int Length => IsOpen ? endIndex : endIndex - startIndex/* + 1*/;
        /// <summary>
        /// Whether the tag is empty, i.e. doesn't contain any indices.
        /// </summary>
        public bool IsEmpty => startIndex == endIndex;
        /// <summary>
        /// Whether the indices contain the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>true if the indices containg the given index; false otherwise.</returns>
        public bool Contains(int index) => !IsEmpty && index >= startIndex && index < endIndex;
        /// <summary>
        /// Enumeration of all contained indices.
        /// </summary>
        public IEnumerable<int> ContainedIndices
        {
            get
            {
                for (int i = startIndex; i < EndIndex; i++)
                {
                    yield return i;
                }
            }
        }

        private int startIndex;
        private int endIndex;
        private int orderAtIndex;

        public TMPEffectTagIndices(int startIndex, int endIndex, int orderAtIndex)
        {
            if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (endIndex < -1) throw new ArgumentOutOfRangeException(nameof(endIndex));
            if (endIndex != -1 && endIndex < startIndex) throw new ArgumentOutOfRangeException(nameof(endIndex));

            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.orderAtIndex = orderAtIndex;
        }

        /// <summary>
        /// Compares this instance to another instance of <see cref="TMPEffectTagIndices"/>.<br/>
        /// First compares <see cref="StartIndex"/>. If those are equal, compares <see cref="OrderAtIndex"/>. The <see cref="EndIndex"/> is not considered.
        /// </summary>
        /// <param name="other">The instance to compare this instance to.</param>
        /// <returns>
        /// Less than zero => This instance precedes <paramref name="other"/> in the sort order.<br/> 
        /// Zero => This instance occurs in the same position in the sort order as <paramref name="other"/>.<br/>
        /// Greater than zero => This instance follows <paramref name="other"/> in the sort order.
        /// </returns>
        public int CompareTo(TMPEffectTagIndices other)
        {
            int res = startIndex.CompareTo(other.startIndex);
            if (res == 0) return orderAtIndex.CompareTo(other.orderAtIndex);
            return res;
        }

        public static bool operator ==(TMPEffectTagIndices c1, TMPEffectTagIndices c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(TMPEffectTagIndices c1, TMPEffectTagIndices c2)
        {
            return !c1.Equals(c2);
        }

        public static bool operator >(TMPEffectTagIndices c1, TMPEffectTagIndices c2)
        {
            return c1.CompareTo(c2) > 0;
        }

        public static bool operator <(TMPEffectTagIndices c1, TMPEffectTagIndices c2)
        {
            return c1.CompareTo(c2) < 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is TMPEffectTagIndices)
            {
                return Equals((TMPEffectTagIndices)obj);
            }
            return false;
        }
        public bool Equals(TMPEffectTagIndices other)
        {
            return startIndex == other.startIndex && endIndex == other.endIndex && orderAtIndex == other.orderAtIndex;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(startIndex, endIndex, orderAtIndex);
        }
    }
}