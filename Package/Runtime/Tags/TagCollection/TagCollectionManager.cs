using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using System.Linq;

namespace TMPEffects.Tags.Collections
{
    /// <summary>
    /// Manager class for multiple, keyed <see cref="ObservableTagCollection"/>.<br/>
    /// Also maintains a union of all contained collections.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    internal class TagCollectionManager<TKey> : ITagCollection, ITagCollectionManager<TKey>, IReadOnlyDictionary<TKey, ObservableTagCollection>, INotifyCollectionChanged where TKey : ITMPPrefixSupplier, ITMPTagValidator
    {
        /// <summary>
        /// Raised when the union collection changes (i.e. when any of the <see cref="ObservableTagCollection"/> change).
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => union.CollectionChanged += value;
            remove => union.CollectionChanged -= value;
        }

        public TagCollectionManager()
        {
            union = new NonAdjustingTagCollection();
            collections = new Dictionary<TKey, ObservableTagCollection>();
            prefixToKey = new Dictionary<char, TKey>();
            autoSync = false;
        }

        public TagCollectionManager(params KeyValuePair<TKey, IEnumerable<KeyValuePair<TMPEffectTagIndices, TMPEffectTag>>>[] entries)
        {
            List<TMPEffectTagTuple> union = new List<TMPEffectTagTuple>();
            foreach (var coll in entries)
            {
                foreach (var kvp in coll.Value)
                {
                    union.Add(new TMPEffectTagTuple(kvp.Value, kvp.Key));
                }
            }

            union = union.OrderBy(x => x.Indices).ToList();
            this.union = new NonAdjustingTagCollection();
            this.union.SetItems(union);

            collections = new();
            prefixToKey = new();
            foreach (var coll in entries)
            {
                if (coll.Key == null) throw new System.ArgumentNullException(nameof(coll.Key));
                if (collections.ContainsKey(coll.Key)) throw new System.ArgumentException(nameof(coll.Key));
                if (prefixToKey.ContainsKey(coll.Key.Prefix)) throw new System.ArgumentException(nameof(coll.Key.Prefix));

                NonAdjustingTagCollection collection = new NonAdjustingTagCollection(coll.Key);
                collection.SetItems(coll.Value.Select(x => new TMPEffectTagTuple(x.Value, x.Key)));
                prefixToKey.Add(coll.Key.Prefix, coll.Key);
                collections.Add(coll.Key, collection);
                collection.CollectionChanged += OnCollectionChanged;
            }
        }

        ITagCollection ITagCollectionManager<TKey>.AddKey(TKey key) => AddKey(key);
        /// <summary>
        /// Add a key. This will automatically create an associated <see cref="ObservableTagCollection"/>.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <returns>The automatically created <see cref="ObservableTagCollection"/>.</returns>
        public ObservableTagCollection AddKey(TKey key)
        {
            if (key == null) throw new System.ArgumentNullException(nameof(key));
            if (collections.ContainsKey(key)) throw new System.ArgumentException(nameof(key));
            if (prefixToKey.ContainsKey(key.Prefix)) throw new System.ArgumentException(nameof(key.Prefix));

            ObservableTagCollection collection = new NonAdjustingTagCollection(key);

            collection.CollectionChanged += OnCollectionChanged;
            prefixToKey.Add(key.Prefix, key);
            collections.Add(key, collection);

            return collection;
        }

        ///<inheritdoc/>
        public bool RemoveKey(TKey key)
        {
            if (!collections.ContainsKey(key)) return false;

            collections[key].CollectionChanged -= OnCollectionChanged;
            collections.Remove(key);
            prefixToKey.Remove(key.Prefix);

            return true;
        }

        #region IReadOnlyDictionary
        ///<inheritdoc/>
        public IEnumerable<TKey> Keys => collections.Keys;

        ///<inheritdoc/>
        public IEnumerable<ObservableTagCollection> Values => collections.Values;

        ///<inheritdoc/>
        public int KeyCount => collections.Count;
        ///<inheritdoc/>
        int IReadOnlyCollection<KeyValuePair<TKey, ObservableTagCollection>>.Count => collections.Count;

        ITagCollection ITagCollectionManager<TKey>.this[TKey key] => collections[key];
        /// <summary>
        /// Get the <see cref="ObservableTagCollection"/> associated with the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="ObservableTagCollection"/> associated with the given key.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public ObservableTagCollection this[TKey key] => collections[key];

        ///<inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            return collections.ContainsKey(key);
        }

        ///<inheritdoc/>
        public bool TryGetValue(TKey key, out ObservableTagCollection value)
        {
            return collections.TryGetValue(key, out value);
        }

        ///<inheritdoc/>
        IEnumerator<KeyValuePair<TKey, ObservableTagCollection>> IEnumerable<KeyValuePair<TKey, ObservableTagCollection>>.GetEnumerator()
        {
            return collections.GetEnumerator();
        }
        #endregion

        #region ITagCollection
        ///<inheritdoc/>
        public bool TryAdd(TMPEffectTag tag, TMPEffectTagIndices indices)
        {
            return TryAdd(tag, indices.StartIndex, indices.EndIndex, indices.OrderAtIndex);
        }

        ///<inheritdoc/>
        public bool TryAdd(TMPEffectTag tag, int startIndex = 0, int endIndex = -1, int? orderAtIndex = null)
        {
            try
            {
                autoSync = true;

                if (!prefixToKey.TryGetValue(tag.Prefix, out TKey key))
                    return false;

                if (!collections[key].TryAdd(tag, startIndex, endIndex, orderAtIndex))
                    return false;

                if (!union.TryAdd(tag, startIndex, endIndex, orderAtIndex))
                {
                    Debug.LogError("Added to collection but failed to add to union; now undefined");
                    return false;
                }

                ValidateIndices(startIndex);
                return true;
            }
            finally
            {
                autoSync = false;
            }
        }

        ///<inheritdoc/>
        public int RemoveAllAt(int startIndex, TMPEffectTagTuple[] buffer = null, int bufferIndex = 0)
        {
            try
            {
                autoSync = true;
                foreach (var collection in collections.Values)
                {
                    collection.RemoveAllAt(startIndex);
                }

                return union.RemoveAllAt(startIndex, buffer, bufferIndex);
            }
            finally
            {
                autoSync = false;
            }
        }

        ///<inheritdoc/>
        public bool RemoveAt(int startIndex, int? order = null)
        {
            var tag = union.TagAt(startIndex, order);
            if (tag == null) return false;
            return Remove(tag);
        }

        ///<inheritdoc/>
        public bool Remove(TMPEffectTag tag, TMPEffectTagIndices? indices = null)
        {
            if (!prefixToKey.TryGetValue(tag.Prefix, out TKey key)) return false;

            try
            {
                autoSync = true;
                bool success = collections[key].Remove(tag, indices);

                if (success)
                {
                    if (!union.Remove(tag, indices))
                    {
                        Debug.LogError("Failed to remove from union but did remove from subcollection; now undefined");
                    }

                    return true;
                }

                return false;
            }
            finally
            {
                autoSync = false;
            }
        }

        ///<inheritdoc/>
        public void Clear()
        {
            try
            {
                autoSync = true;

                union.Clear();
                foreach (var collection in collections.Values)
                    collection.Clear();
            }
            finally
            {
                autoSync = false;
            }
        }

        ///<inheritdoc/>
        public int TagCount => union.TagCount;
        ///<inheritdoc/>
        public bool Contains(TMPEffectTag tag, TMPEffectTagIndices? indices = null) => union.Contains(tag, indices);
        ///<inheritdoc/>
        public TMPEffectTagIndices? IndicesOf(TMPEffectTag tag) => union.IndicesOf(tag);
        ///<inheritdoc/>
        public int TagsAt(int startIndex, TMPEffectTagTuple[] buffer, int bufferIndex = 0) => union.TagsAt(startIndex, buffer, bufferIndex);
        ///<inheritdoc/>
        public IEnumerable<TMPEffectTagTuple> TagsAt(int startIndex) => union.TagsAt(startIndex);
        ///<inheritdoc/>
        public TMPEffectTag TagAt(int startIndex, int? order = null) => union.TagAt(startIndex, order);
        ///<inheritdoc/>
        public IEnumerator<TMPEffectTagTuple> GetEnumerator() => union.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => union.GetEnumerator();
        #endregion

        private NonAdjustingTagCollection union;
        private readonly Dictionary<TKey, ObservableTagCollection> collections;
        private readonly Dictionary<char, TKey> prefixToKey;

        private bool autoSync;

        private void ValidateIndices(int index)
        {
            List<TMPEffectTagTuple> list = union.ToList();

            if (list.Count == 0) return;

            bool prevSync = autoSync;

            try
            {
                autoSync = true;

                int i = 0;
                for (; i < list.Count; i++)
                {
                    if (list[i].Indices.StartIndex == index) break;
                    if (list[i].Indices.StartIndex > index) return;
                }

                int lastOrder = list[i].Indices.OrderAtIndex;
                i++;


                for (; i < list.Count; i++)
                {
                    var current = list[i];
                    if (current.Indices.StartIndex != index) break;

                    if (current.Indices.OrderAtIndex <= lastOrder)
                    {
                        lastOrder += 1;

                        if (!union.SetOrder(current.Tag, current.Indices, lastOrder))
                        {
                            Debug.LogError("Failed to set order in union; now undefined");
                        }
                        if (!(collections[prefixToKey[current.Tag.Prefix]] as NonAdjustingTagCollection).SetOrder(current.Tag, current.Indices, lastOrder))
                        {
                            Debug.LogError("Failed to set order in subcollection; now undefined");
                        }
                    }
                    else
                    {
                        lastOrder = current.Indices.OrderAtIndex;
                    }
                }
            }
            finally
            {
                autoSync = prevSync;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (autoSync) return;

            TMPEffectTagTuple tuple;
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    tuple = (TMPEffectTagTuple)args.NewItems[0];
                    if (!union.TryAdd(tuple.Tag, tuple.Indices))
                    {
                        Debug.LogError("Failed to add to union; now undefined");
                    }
                    ValidateIndices(((TMPEffectTagTuple)args.NewItems[0]).Indices.StartIndex);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var current in args.OldItems)
                    {
                        tuple = (TMPEffectTagTuple)current;
                        if (!union.RemoveAt(tuple.Indices.StartIndex, tuple.Indices.OrderAtIndex))
                        {
                            Debug.LogError("Failed to remove from union; now undefined");
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    IEnumerable<TMPEffectTagTuple> concat = new List<TMPEffectTagTuple>();

                    foreach (var coll in collections.Values)
                    {
                        concat.Concat(coll);
                    }

                    concat = concat.OrderBy(x => x.Indices).ToList();
                    union = new NonAdjustingTagCollection();
                    foreach (var item in concat)
                    {
                        if (!union.TryAdd(item.Tag, item.Indices))
                        {
                            Debug.LogError("Failed to add tag to union; Now undefined");
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Move:
                    throw new System.NotImplementedException();

                case NotifyCollectionChangedAction.Replace:
                    throw new System.NotImplementedException();
            }
        }

        // TODO
        // Right now, other listeners (specifically CachedCollection) rely on 
        // indices only being changed via removing and reinsertion
        // => Implement NotifyCollectionChanged more robustly and raise replace / move events
        private class NonAdjustingTagCollection : ObservableTagCollection
        {
            public NonAdjustingTagCollection(ITMPTagValidator validator = null) : base(validator)
            {
                //if (validator == null) throw new System.ArgumentNullException(nameof(validator)); 
            }

            internal bool SetOrder(TMPEffectTag tag, TMPEffectTagIndices indices, int newOrder)
            {
                int index;
                if ((index = BinarySearchIndexOf(indices)) < 0) return false;

                do
                {
                    if (tag == tags[index].Tag) break;
                    if (tags[index].Indices.StartIndex != indices.StartIndex) break;
                    index++;
                }
                while (index < tags.Count);

                if (index == tags.Count || tags[index].Indices.StartIndex != indices.StartIndex) return false;

                var prev = tags[index];
                tags[index] = new TMPEffectTagTuple(tag, new TMPEffectTagIndices(indices.StartIndex, indices.EndIndex, newOrder));

                InvokeEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, prev, tags[index], index));

                return true;

            }

            public override bool TryAdd(TMPEffectTag tag, TMPEffectTagIndices indices)
            {
                if (validator != null && !validator.ValidateTag(tag)) return false;

                int index;
                if ((index = BinarySearchIndexOf(indices)) < 0)
                    index = ~index;

                tags.Insert(index, new TMPEffectTagTuple(tag, indices));
                InvokeEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, tags[index], index));
                return true;
            }

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
                }

                tags.Insert(index, new TMPEffectTagTuple(tag, indices));
                InvokeEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, tags[index], index));
                return true;
            }

            internal void SetItems(IEnumerable<TMPEffectTagTuple> items)
            {
                tags.Clear();
                foreach (var tag in items) tags.Add(tag);
            }
        }
    }
}
