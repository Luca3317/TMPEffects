using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.Tags.Collections
{
    ///<inheritdoc/>
    public class TagCollection : ITagCollection
    {
        public TagCollection(IList<TMPEffectTagTuple> tags, ITMPTagValidator validator = null)
        {
            this.validator = validator;
            this.tags = tags;
        }
        public TagCollection(ITMPTagValidator validator = null)
        {
            this.validator = validator;
            this.tags = new List<TMPEffectTagTuple>();
        }

        ///<inheritdoc/>
        public virtual bool TryAdd(TMPEffectTag tag, TMPEffectTagIndices indices)
        {
            if (validator != null && !validator.ValidateTag(tag)) return false;

            int index;
            // If there already is a tag with these exact indices, adjust indices
            if ((index = BinarySearchIndexOf(indices)) > 0)
            {
                AdjustOrderAtIndexAt(index, indices);
            }
            // Otherwise adjust the index
            else index = ~index;

            tags.Insert(index, new TMPEffectTagTuple(tag, indices));
            return true;
        }

        ///<inheritdoc/>
        public virtual bool TryAdd(TMPEffectTag tag, int startIndex = 0, int endIndex = -1, int? orderAtIndex = null)
        {
            if (validator != null && !validator.ValidateTag(tag)) return false;

            int index;
            TMPEffectTagIndices indices;

            // If no order is specified, add it as first element at current index
            if (orderAtIndex == null)
            {
                index = BinarySearchIndexFirstIndexOf(new StartIndexOnly(startIndex));

                // If no tag with that startindex
                if (index < 0)
                {
                    index = ~index;
                    indices = new TMPEffectTagIndices(startIndex, endIndex, 0);
                }
                // Otherwise
                else
                {
                    indices = new TMPEffectTagIndices(startIndex, endIndex, tags[index].Indices.OrderAtIndex - 1);
                }
            }
            // If order is specified
            else
            {
                index = BinarySearchIndexOf(new TempIndices(startIndex, orderAtIndex.Value));
                indices = new TMPEffectTagIndices(startIndex, endIndex, orderAtIndex.Value);

                // If no tag with these indices
                if (index < 0)
                {
                    index = ~index;
                }
                else
                {
                    AdjustOrderAtIndexAt(index, indices);
                }
            }

            tags.Insert(index, new TMPEffectTagTuple(tag, indices));
            return true;
        }

        protected void AdjustOrderAtIndexAt(int listIndex, TMPEffectTagIndices indices)
        {
            TMPEffectTagTuple current;
            TMPEffectTagIndices last = indices;

            while ((current = tags[listIndex]).Indices.StartIndex == last.StartIndex && current.Indices.OrderAtIndex == last.OrderAtIndex)
            {
                tags[listIndex++] = new TMPEffectTagTuple(current.Tag, new TMPEffectTagIndices(current.Indices.StartIndex, current.Indices.EndIndex, current.Indices.OrderAtIndex + 1));
                //tags[listIndex++] = new KeyValuePair<EffectTagIndices, EffectTag>(new EffectTagIndices(current.Key.StartIndex, current.Key.EndIndex, current.Key.OrderAtIndex + 1), current.Value);
                last = current.Indices;
            }
        }

        ///<inheritdoc/>
        public virtual int RemoveAllAt(int startIndex, TMPEffectTagTuple[] buffer = null, int bufferIndex = 0)
        {
            int firstIndex = BinarySearchIndexFirstIndexOf(new StartIndexOnly(startIndex));
            if (firstIndex < 0) return 0;

            int lastIndex = firstIndex;

            do lastIndex++;
            while (lastIndex < tags.Count && tags[lastIndex].Indices.StartIndex == startIndex);

            int count = lastIndex - firstIndex;
            if (buffer != null)
            {
                if (buffer == null) throw new ArgumentNullException(nameof(buffer));
                if (bufferIndex < 0) throw new ArgumentOutOfRangeException(nameof(bufferIndex));

                int len = Mathf.Min(count, buffer.Length - bufferIndex);
                for (int i = 0; i < len; i++)
                {
                    buffer[bufferIndex + i] = tags[firstIndex];
                    tags.RemoveAt(firstIndex);
                }
            }

            for (int i = 0; i < count; i++)
            {
                tags.RemoveAt(firstIndex);
            }

            return count;
        }

        ///<inheritdoc/>
        public virtual bool RemoveAt(int startIndex, int? order = null)
        {
            int index;
            if (order == null)
            {
                index = BinarySearchIndexFirstIndexOf(new StartIndexOnly(startIndex));
            }
            else
            {
                index = BinarySearchIndexOf(new TempIndices(startIndex, order.Value));
            }

            if (index < 0) return false;

            tags.RemoveAt(index);
            return true;
        }

        ///<inheritdoc/>
        public virtual void Clear()
        {
            tags.Clear();
        }

        ///<inheritdoc/>
        public virtual bool Remove(TMPEffectTag tag, TMPEffectTagIndices? indices = null)
        {
            int index;
            if (indices == null)
            {
                index = FindIndex(tag);

                if (index < 0) return false;
                tags.RemoveAt(index);
                return true;
            }

            index = BinarySearchIndexOf(indices);
            if (index < 0) return false;
            if (tags[index].Tag != tag) return false;

            tags.RemoveAt(index);
            return true;
        }

        public void CopyTo(TMPEffectTag[] array, int arrayIndex)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < tags.Count) throw new ArgumentException(nameof(array));

            for (int i = 0; i < tags.Count; i++)
            {
                array[arrayIndex + i] = tags[i].Tag;
            }
        }

        ///<inheritdoc/>
        public int TagCount => tags.Count;

        ///<inheritdoc/>
        public bool Contains(TMPEffectTag tag, TMPEffectTagIndices? indices = null)
        {
            if (indices == null) return FindIndex(tag) >= 0;
            return FindIndex(tag) >= 0;
        }

        public IEnumerator<TMPEffectTagTuple> GetEnumerator() => tags.GetEnumerator();

        ///<inheritdoc/>
        public TMPEffectTag TagAt(int startIndex, int? order = null)
        {
            int index;
            if (order == null)
            {
                index = BinarySearchIndexFirstIndexOf(new StartIndexOnly(startIndex));
            }
            else
            {
                index = BinarySearchIndexOf(new TempIndices(startIndex, order.Value));
            }

            if (index < 0) return null; // Throw?

            return tags[index].Tag;
        }

        ///<inheritdoc/>
        public int TagsAt(int startIndex, TMPEffectTagTuple[] buffer, int bufferIndex = 0)
        {
            int firstIndex = BinarySearchIndexOf(new StartIndexOnly(startIndex));
            if (firstIndex < 0) return 0;

            int lastIndex = firstIndex;

            do lastIndex++;
            while (lastIndex < tags.Count && tags[lastIndex].Indices.StartIndex != startIndex);

            int count = lastIndex - firstIndex;
            if (buffer != null)
            {
                if (buffer == null) throw new ArgumentNullException(nameof(buffer));
                if (bufferIndex < 0) throw new ArgumentOutOfRangeException(nameof(bufferIndex));

                int len = Mathf.Min(count, buffer.Length - bufferIndex);
                for (int i = 0; i < len; i++)
                {
                    buffer[bufferIndex + i] = tags[firstIndex];
                }
            }

            return count;
        }

        ///<inheritdoc/>
        public IEnumerable<TMPEffectTagTuple> TagsAt(int startIndex)
        {
            int firstIndex = BinarySearchIndexFirstIndexOf(new StartIndexOnly(startIndex));
            if (firstIndex < 0) yield break;

            int lastIndex = firstIndex;

            do yield return tags[lastIndex++];
            while (lastIndex < tags.Count && tags[lastIndex].Indices.StartIndex == startIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        ///<inheritdoc/>
        public TMPEffectTagIndices? IndicesOf(TMPEffectTag tag)
        {
            for (int i = 0; i < tags.Count; i++)
            {
                if (tags[i].Tag == tag)
                    return tags[i].Indices;
            }
            return null;
        }


        protected int FindIndex(TMPEffectTag tag)
        {
            for (int i = 0; i < tags.Count; i++)
            {
                if (tag == tags[i].Tag) return i;
            }

            return -1;
        }

        protected int BinarySearchIndexOf(IComparable<TMPEffectTagIndices> indices)
        {
            int lower = 0;
            int upper = tags.Count - 1;

            while (lower <= upper)
            {
                int middle = lower + (upper - lower) / 2;
                int comparisonResult = indices.CompareTo(tags[middle].Indices);
                if (comparisonResult == 0)
                    return middle;
                else if (comparisonResult < 0)
                    upper = middle - 1;
                else
                    lower = middle + 1;
            }

            return ~lower;
        }

        protected int BinarySearchIndexFirstIndexOf(StartIndexOnly indices)
        {
            int res = BinarySearchIndexOf(indices);
            if (res < 0) return res;

            while (res >= 0 && tags[res].Indices.StartIndex == indices.startIndex)
            {
                res--;
            }

            return res + 1;
        }

        protected struct TempIndices : IComparable<TMPEffectTagIndices>
        {
            private readonly int startIndex;
            private readonly int orderAtIndex;

            public TempIndices(int startIndex, int orderAtIndex)
            {
                this.startIndex = startIndex;
                this.orderAtIndex = orderAtIndex;
            }

            public int CompareTo(TMPEffectTagIndices other)
            {
                int res = startIndex.CompareTo(other.StartIndex);
                if (res == 0) return orderAtIndex.CompareTo(other.OrderAtIndex);
                return res;
            }
        }

        protected struct StartIndexOnly : IComparable<TMPEffectTagIndices>
        {
            public readonly int startIndex;

            public StartIndexOnly(int startIndex)
            {
                this.startIndex = startIndex;
            }

            public int CompareTo(TMPEffectTagIndices other)
            {
                return startIndex.CompareTo(other.StartIndex);
            }
        }

        protected IList<TMPEffectTagTuple> tags;
        protected readonly ITMPTagValidator validator;
    }
}
        