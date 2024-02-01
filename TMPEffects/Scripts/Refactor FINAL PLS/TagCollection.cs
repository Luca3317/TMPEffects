using PlasticGui.Gluon.WorkspaceWindow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TMPEffects;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class ObservableTagCollection : TagCollection, INotifyCollectionChanged
{
    public ObservableTagCollection(IList<EffectTagTuple> tags, ITMPTagValidator validator = null) : base(tags, validator)
    { }
    public ObservableTagCollection(ITMPTagValidator validator = null) : base(validator)
    { }

    public event NotifyCollectionChangedEventHandler CollectionChanged;


    protected void InvokeEvent(NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
    }

    public override bool TryAdd(EffectTag tag, EffectTagIndices indices)
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

        tags.Insert(index, new EffectTagTuple(tag, indices));
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, tags[index], index));
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
            else
            {
                AdjustOrderAtIndexAt(index, indices);
            }
        }

        tags.Insert(index, new EffectTagTuple(tag, indices));
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, tags[index], index));
        return true;
    }


    public override bool Remove(EffectTag tag, EffectTagIndices? indices = null)
    {
        int index;
        EffectTagTuple tuple;
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

    public override int RemoveAllAt(int startIndex, EffectTagTuple[] buffer = null, int bufferIndex = 0)
    {
        List<EffectTagTuple> removed = new List<EffectTagTuple>();
        int first = BinarySearchIndexFirstIndexOf(new StartIndexOnly(startIndex));
        if (first == -1) return 0;

        if (buffer != null)
        {
            int i = 0;
            int len = Mathf.Min(tags.Count, buffer.Length - bufferIndex);
            for (i = first; i < len;)
            {
                EffectTagTuple tuple = tags[i];
                if (tuple.Indices.StartIndex != startIndex) break;
                buffer[i] = tuple;
                removed.Add(tuple);
                tags.RemoveAt(i);
            }

            for (; i < tags.Count;)
            {
                EffectTagTuple kvp = tags[i];
                if (kvp.Indices.StartIndex != startIndex) break;
                removed.Add(kvp);
                tags.RemoveAt(i);
            }
        }
        else
        {
            for (int i = first; i < tags.Count;)
            {
                EffectTagTuple kvp = tags[i];
                if (kvp.Indices.StartIndex != startIndex) break;
                removed.Add(kvp);
                tags.RemoveAt(i);
            }
        }

        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, first));

        return removed.Count;
    }

    public override void Clear()
    {
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        tags.Clear();
    }
}

public class TagCollection : ITagCollection
{
    protected IList<EffectTagTuple> tags;
    protected readonly ITMPTagValidator validator;

    public TagCollection(IList<EffectTagTuple> tags, ITMPTagValidator validator = null)
    {
        this.validator = validator;
        this.tags = tags;
    }
    public TagCollection(ITMPTagValidator validator = null)
    {
        this.validator = validator;
        this.tags = new List<EffectTagTuple>();
    }

    public virtual bool TryAdd(EffectTag tag, EffectTagIndices indices)
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

        tags.Insert(index, new EffectTagTuple(tag, indices));
        return true;
    }

    public virtual bool TryAdd(EffectTag tag, int startIndex = 0, int endIndex = -1, int? orderAtIndex = null)
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
            else
            {
                AdjustOrderAtIndexAt(index, indices);
            }
        }

        tags.Insert(index, new EffectTagTuple(tag, indices));
        return true;
    }

    protected void AdjustOrderAtIndexAt(int listIndex, EffectTagIndices indices)
    {
        EffectTagTuple current;
        EffectTagIndices last = indices;

        while ((current = tags[listIndex]).Indices.StartIndex == last.StartIndex && current.Indices.OrderAtIndex == last.OrderAtIndex)
        {
            tags[listIndex++] = new EffectTagTuple(current.Tag, new EffectTagIndices(current.Indices.StartIndex, current.Indices.EndIndex, current.Indices.OrderAtIndex + 1));
            //tags[listIndex++] = new KeyValuePair<EffectTagIndices, EffectTag>(new EffectTagIndices(current.Key.StartIndex, current.Key.EndIndex, current.Key.OrderAtIndex + 1), current.Value);
            last = current.Indices;
        }
    }

    public virtual int RemoveAllAt(int startIndex, EffectTagTuple[] buffer = null, int bufferIndex = 0)
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

    public virtual void Clear()
    {
        tags.Clear();
    }

    public virtual bool Remove(EffectTag tag, EffectTagIndices? indices = null)
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

    public void CopyTo(EffectTag[] array, int arrayIndex)
    {
        if (array is null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < tags.Count) throw new ArgumentException(nameof(array));

        for (int i = 0; i < tags.Count; i++)
        {
            array[arrayIndex + i] = tags[i].Tag;
        }
    }

    public int TagCount => tags.Count;

    public bool Contains(EffectTag tag, EffectTagIndices? indices = null)
    {
        if (indices == null) return FindIndex(tag) >= 0;
        return FindIndex(tag) >= 0;

        //int index = BinarySearchIndexOf(new TempIndices(tag.StartIndex, tag.OrderAtIndex));
        //if (index < 0) return false;
        //if (tags[index].Value != tag) return false;
        //return true;
    }

    public IEnumerator<EffectTagTuple> GetEnumerator() => tags.GetEnumerator();

    public EffectTag TagAt(int startIndex, int? order = null)
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

    public int TagsAt(int startIndex, EffectTagTuple[] buffer, int bufferIndex = 0)
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

    public IEnumerable<EffectTagTuple> TagsAt(int startIndex)
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

    public EffectTagIndices? IndicesOf(EffectTag tag)
    {
        for (int i = 0; i < tags.Count; i++)
        {
            if (tags[i].Tag == tag)
                return tags[i].Indices;
        }
        return null;
    }


    protected int FindIndex(EffectTag tag)
    {
        for (int i = 0; i < tags.Count; i++)
        {
            if (tag == tags[i].Tag) return i;
        }

        return -1;
    }

    protected int BinarySearchIndexOf(IComparable<EffectTagIndices> indices)
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

    protected struct TempIndices : IComparable<EffectTagIndices>
    {
        private readonly int startIndex;
        private readonly int orderAtIndex;

        public TempIndices(int startIndex, int orderAtIndex)
        {
            this.startIndex = startIndex;
            this.orderAtIndex = orderAtIndex;
        }

        public int CompareTo(EffectTagIndices other)
        {
            int res = startIndex.CompareTo(other.StartIndex);
            if (res == 0) return orderAtIndex.CompareTo(other.OrderAtIndex);
            return res;
        }
    }

    protected struct StartIndexOnly : IComparable<EffectTagIndices>
    {
        public readonly int startIndex;

        public StartIndexOnly(int startIndex)
        {
            this.startIndex = startIndex;
        }

        public int CompareTo(EffectTagIndices other)
        {
            return startIndex.CompareTo(other.StartIndex);
        }
    }
}

public class ReadOnlyTagCollection : IReadOnlyTagCollection
{
    private IReadOnlyTagCollection collection;

    internal ReadOnlyTagCollection(List<EffectTagTuple> tags)
    {
        this.collection = new TagCollection(tags);
    }

    internal ReadOnlyTagCollection(IReadOnlyTagCollection collection)
    {
        this.collection = collection;
    }

    public int TagCount => collection.TagCount;

    public bool Contains(EffectTag tag, EffectTagIndices? indices = null)
    {
        return collection.Contains(tag, indices);
    }

    public IEnumerator<EffectTagTuple> GetEnumerator()
    {
        return collection.GetEnumerator();
    }

    public EffectTagIndices? IndicesOf(EffectTag tag)
    {
        return collection.IndicesOf(tag);
    }

    public EffectTag TagAt(int startIndex, int? order = null)
    {
        return collection.TagAt(startIndex, order);
    }

    public int TagsAt(int startIndex, EffectTagTuple[] buffer, int bufferIndex = 0)
    {
        return collection.TagsAt(startIndex, buffer, bufferIndex);
    }

    public IEnumerable<EffectTagTuple> TagsAt(int startIndex)
    {
        return collection.TagsAt(startIndex);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return collection.GetEnumerator();
    }
}

/// <summary>
/// A writable collection of <see cref="EffectTagTuple"/>.
/// </summary>
public interface ITagCollection : IReadOnlyTagCollection
{
    public bool TryAdd(EffectTag tag, EffectTagIndices indices);
    public bool TryAdd(EffectTag tag, int startIndex = 0, int endIndex = -1, int? orderAtIndex = null);

    public int RemoveAllAt(int startIndex, EffectTagTuple[] buffer = null, int bufferIndex = 0);
    public bool RemoveAt(int startIndex, int? order = null);

    public bool Remove(EffectTag tagData, EffectTagIndices? indices = null);

    public void Clear();
}


/*
 * TODO
 * Maybe: Make ITagCollection => ITagCollection<T> where T : ITagWrapper
 * 
 * Then: TagCollection : ITagCollection<EffectTagTuple>
 *          (same with observable/reeadonly)
 *          
 * Allows for easier cacher
 */

/// <summary>
/// A readonly collection of <see cref="EffectTagTuple"/>.
/// </summary>
public interface IReadOnlyTagCollection : IReadOnlyCollection<EffectTagTuple>
{
    public int TagCount { get; }

    public bool Contains(EffectTag tag, EffectTagIndices? indices = null);

    public EffectTagIndices? IndicesOf(EffectTag tag);

    public int TagsAt(int startIndex, EffectTagTuple[] buffer, int bufferIndex = 0);
    public IEnumerable<EffectTagTuple> TagsAt(int startIndex);
    public EffectTag TagAt(int startIndex, int? order = null);

    int IReadOnlyCollection<EffectTagTuple>.Count => TagCount;
}