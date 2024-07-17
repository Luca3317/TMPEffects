using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace TMPEffects.Tags.Collections
{
    /// <summary>
    /// An observable <see cref="ITagCollection"/>.
    /// </summary>
    public class ObservableTagCollection : TagCollection, INotifyCollectionChanged
    {
        public ObservableTagCollection(IList<TMPEffectTagTuple> tags, ITMPTagValidator validator = null) : base(tags, validator)
        { }
        public ObservableTagCollection(ITMPTagValidator validator = null) : base(validator)
        { }

        /// <summary>
        /// Raised when the collection changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void InvokeEvent(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        ///<inheritdoc/>
        public override bool TryAdd(TMPEffectTag tag, TMPEffectTagIndices indices)
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
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, tags[index], index));
            return true;
        }

        ///<inheritdoc/>
        public override bool TryAdd(TMPEffectTag tag, int startIndex = 0, int endIndex = -1, int? orderAtIndex = null)
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
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, tags[index], index));
            return true;
        }


        ///<inheritdoc/>
        public override bool Remove(TMPEffectTag tag, TMPEffectTagIndices? indices = null)
        {
            int index;
            TMPEffectTagTuple tuple;
            if (indices == null)
            {
                index = FindIndex(tag);

                if (index < 0) return false;
                tuple = tags[index];
                tags.RemoveAt(index);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, tuple, index));
                return true;
            }

            index = BinarySearchIndexOf(indices);
            if (index < 0) return false;
            if (tags[index].Tag != tag) return false;

            tuple = tags[index];
            tags.RemoveAt(index);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, tuple, index));
            return true;
        }

        ///<inheritdoc/>
        public override bool RemoveAt(int startIndex, int? order = null)
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

            var kvp = tags[index];
            tags.RemoveAt(index);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, kvp, index));
            return true;
        }

        ///<inheritdoc/>
        public override int RemoveAllAt(int startIndex, TMPEffectTagTuple[] buffer = null, int bufferIndex = 0)
        {
            List<TMPEffectTagTuple> removed = new List<TMPEffectTagTuple>();
            int first = BinarySearchIndexFirstIndexOf(new StartIndexOnly(startIndex));
            if (first < 0) return 0;

            if (buffer != null)
            {
                int i = 0;
                int len = Mathf.Min(tags.Count, buffer.Length - bufferIndex);
                for (i = first; i < len;)
                {
                    TMPEffectTagTuple tuple = tags[i];
                    if (tuple.Indices.StartIndex != startIndex) break;
                    buffer[i] = tuple;
                    removed.Add(tuple);
                    tags.RemoveAt(i);
                }

                for (; i < tags.Count;)
                {
                    TMPEffectTagTuple kvp = tags[i];
                    if (kvp.Indices.StartIndex != startIndex) break;
                    removed.Add(kvp);
                    tags.RemoveAt(i);
                }
            }
            else
            {
                for (int i = first; i < tags.Count;)
                {
                    TMPEffectTagTuple kvp = tags[i];
                    if (kvp.Indices.StartIndex != startIndex) break;
                    removed.Add(kvp);
                    tags.RemoveAt(i);
                }
            }

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, first));

            return removed.Count;
        }

        ///<inheritdoc/>
        public override void Clear()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            tags.Clear();
        }
    }
}