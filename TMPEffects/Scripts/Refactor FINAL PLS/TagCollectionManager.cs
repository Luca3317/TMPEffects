using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using TMPEffects;
using System.Linq;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using static Codice.CM.Common.CmCallContext;
using System.Threading.Tasks;
using System;

public interface ITagCollectionManager<TKey> : IReadOnlyDictionary<TKey, ObservableTagCollection>
{
    public ObservableTagCollection AddKey(TKey key);
    public bool RemoveKey(TKey key);
}

public class TagCollectionManager<TKey> : ITagCollection, ITagCollectionManager<TKey>, IReadOnlyDictionary<TKey, ObservableTagCollection>, INotifyCollectionChanged where TKey : ITMPPrefixSupplier, ITMPTagValidator
{
    private NonAdjustingTagCollection union;
    private readonly Dictionary<TKey, ObservableTagCollection> collections;
    private readonly Dictionary<char, TKey> prefixToKey;

    private bool autoSync;

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

    public ObservableTagCollection AddKey(TKey key)
    {
        ObservableTagCollection collection = new NonAdjustingTagCollection();

        collection.CollectionChanged += OnCollectionChanged;
        prefixToKey.Add(key.Prefix, key);
        collections.Add(key, collection);

        return collection;
    }

    public bool RemoveKey(TKey key)
    {
        if (!collections.ContainsKey(key)) return false;

        collections[key].CollectionChanged -= OnCollectionChanged;
        collections.Remove(key);
        prefixToKey.Remove(key.Prefix);

        return true;
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        if (autoSync) return;

        Debug.Log("Lemme fix it");
        EffectTagTuple tuple;
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                tuple = (EffectTagTuple)args.NewItems[0];
                if (!union.TryAdd(tuple.Tag, tuple.Indices))
                {
                    Debug.LogError("Failed to add to union; now undefined");
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (var current in args.OldItems)
                {
                    tuple = (EffectTagTuple)current;
                    if (!union.RemoveAt(tuple.Indices.StartIndex, tuple.Indices.OrderAtIndex))
                    {
                        Debug.LogError("Failed to remove from union; now undefined");
                    }
                }
                break;

            case NotifyCollectionChangedAction.Reset:
                IEnumerable<EffectTagTuple> concat = new List<EffectTagTuple>();

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
                        Debug.Log("Failed to add tag to union; Now undefined");
                    }
                }

                break;

            case NotifyCollectionChangedAction.Move:
                throw new System.NotImplementedException();

            case NotifyCollectionChangedAction.Replace:
                throw new System.NotImplementedException();
        }
    }

    #region IReadOnlyDictionary
    public IEnumerable<TKey> Keys => collections.Keys;

    public IEnumerable<ObservableTagCollection> Values => collections.Values;

    public int KeyCount => collections.Count;
    int IReadOnlyCollection<KeyValuePair<TKey, ObservableTagCollection>>.Count => collections.Count;

    public ObservableTagCollection this[TKey key] => collections[key];

    public bool ContainsKey(TKey key)
    {
        return collections.ContainsKey(key);
    }

    public bool TryGetValue(TKey key, out ObservableTagCollection value)
    {
        return collections.TryGetValue(key, out value);
    }

    IEnumerator<KeyValuePair<TKey, ObservableTagCollection>> IEnumerable<KeyValuePair<TKey, ObservableTagCollection>>.GetEnumerator()
    {
        return collections.GetEnumerator();
    }
    #endregion

    #region ITagCollection
    public bool TryAdd(EffectTag tag, EffectTagIndices indices)
    {
        return TryAdd(tag, indices.StartIndex, indices.EndIndex, indices.OrderAtIndex);
    }

    public bool TryAdd(EffectTag tag, int startIndex = 0, int endIndex = -1, int? orderAtIndex = null)
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

            Debug.Log("added at " + union.IndicesOf(tag).Value.StartIndex + " - " + union.IndicesOf(tag).Value.OrderAtIndex);

            ValidateIndices(startIndex);
            return true;
        }
        finally
        {
            autoSync = false;
        }
    }


    private void ValidateIndices(int index)
    {
        List<EffectTagTuple> list = union.ToList();

        Debug.Log("Pre Validation: ");
        foreach (var item in list)
        {
            if (item.Indices.StartIndex == index)
            {
                Debug.Log(item.Tag.Name + ": " + item.Indices.StartIndex + " - " + item.Indices.OrderAtIndex + " : " + item.Indices.EndIndex);
            }
        }

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
                        Debug.LogWarning("FAILED TO SET ORDER IN UNION?");
                    }
                    if (!(collections[prefixToKey[current.Tag.Prefix]] as NonAdjustingTagCollection).SetOrder(current.Tag, current.Indices, lastOrder))
                    {
                        Debug.LogWarning("FAILED TO SET ORDER IN SUBCOLLECTION?");
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

    public int RemoveAllAt(int startIndex, EffectTagTuple[] buffer = null, int bufferIndex = 0)
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

    public bool RemoveAt(int startIndex, int? order = null)
    {
        var tag = union.TagAt(startIndex, order);
        if (tag == null) return false;
        return Remove(tag);
    }

    public bool Remove(EffectTag tag, EffectTagIndices? indices = null)
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

    public int TagCount => union.TagCount;
    public bool Contains(EffectTag tag, EffectTagIndices? indices = null) => union.Contains(tag, indices);
    public EffectTagIndices? IndicesOf(EffectTag tag) => union.IndicesOf(tag);
    public int TagsAt(int startIndex, EffectTagTuple[] buffer, int bufferIndex = 0) => union.TagsAt(startIndex, buffer, bufferIndex);
    public IEnumerable<EffectTagTuple> TagsAt(int startIndex) => union.TagsAt(startIndex);
    public EffectTag TagAt(int startIndex, int? order = null) => union.TagAt(startIndex, order);
    public IEnumerator<EffectTagTuple> GetEnumerator() => union.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => union.GetEnumerator();
    #endregion

    // TODO
    // Right now, (Im pretty sure) other listeners (specifically CachedCollection) rely on 
    // indices only being changed via removing and reinsertion
    // => Implement NotifyCollectionChanged more robustly and raise replace / move events
    private class NonAdjustingTagCollection : ObservableTagCollection
    {
        internal bool SetOrder(EffectTag tag, EffectTagIndices indices, int newOrder)
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
            tags[index] = new EffectTagTuple(tag, new EffectTagIndices(indices.StartIndex, indices.EndIndex, newOrder));

            InvokeEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, prev, tags[index], index));

            return true;

        }

        public override bool TryAdd(EffectTag tag, EffectTagIndices indices)
        {
            Debug.Log("DO CALL THIS YES");
            if (validator != null && !validator.ValidateTag(tag)) return false;

            int index;
            if ((index = BinarySearchIndexOf(indices)) < 0)
                index = ~index;

            tags.Insert(index, new EffectTagTuple(tag, indices));
            InvokeEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, tags[index], index));
            return true;
        }

        public override bool TryAdd(EffectTag tag, int startIndex = 0, int endIndex = -1, int? orderAtIndex = null)
        {
            if (validator != null && !validator.ValidateTag(tag)) return false;

            int index;
            EffectTagIndices indices;

            // If no order is specified, add it as first element at current index
            if (orderAtIndex == null)
            {
                index = BinarySearchIndexFirstIndexOf(new StartIndexOnly(startIndex));

                // If no tag with that startindex
                if (index < 0)
                {
                    index = ~index;
                    indices = new EffectTagIndices(startIndex, endIndex, 0);
                }
                // Otherwise
                else
                {
                    indices = new EffectTagIndices(startIndex, endIndex, tags[index].Indices.OrderAtIndex - 1);
                }
            }
            // If order is specified
            else
            {
                index = BinarySearchIndexOf(new TempIndices(startIndex, orderAtIndex.Value));
                indices = new EffectTagIndices(startIndex, endIndex, orderAtIndex.Value);

                // If no tag with these indices
                if (index < 0)
                {
                    index = ~index;
                }
            }

            tags.Insert(index, new EffectTagTuple(tag, indices));
            InvokeEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, tags[index], index));
            return true;
        }
    }
}