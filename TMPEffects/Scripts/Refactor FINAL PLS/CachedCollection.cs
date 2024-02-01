using IntervalTree;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using TMPEffects.Components;
using TMPEffects.Tags;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

public interface ITagCacher<T> where T : ITagWrapper
{
    public T CacheTag(EffectTag tag, EffectTagIndices indices);
}

public class CachedCollection<T> where T : ITagWrapper
{
    private List<T> cache = new List<T>();
    private ObservableTagCollection tagCollection;
    private ITagCacher<T> cacher;

    public int Count => cache.Count;

    public CachedCollection(ITagCacher<T> cacher, ObservableTagCollection tagCollection)
    {
        this.cacher = cacher;
        this.tagCollection = tagCollection;

        foreach (var tagData in tagCollection)
        {
            cache.Add(cacher.CacheTag(tagData.Tag, tagData.Indices));
        }

        tagCollection.CollectionChanged += OnCollectionChanged;
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems.Count > 1) Debug.LogWarning("Added more than one element; Should be impossible");
                EffectTagTuple tuple = (EffectTagTuple)e.NewItems[0];
                Debug.LogWarning("Adding at " + e.NewStartingIndex);
                cache.Insert(e.NewStartingIndex, cacher.CacheTag(tuple.Tag, tuple.Indices));
                //Add(tuple.Indices, tuple.Tag);
                break;

            case NotifyCollectionChangedAction.Remove:
                int index = e.OldStartingIndex;
                for (int i = 0; i < cache.Count; i++)
                {
                    cache.RemoveAt(index);
                }
                break;

            case NotifyCollectionChangedAction.Reset:
                cache.Clear();
                break;

            case NotifyCollectionChangedAction.Move:
                throw new NotImplementedException();

            case NotifyCollectionChangedAction.Replace:
                index = e.NewStartingIndex;

                for (int i = 0; i < cache.Count; i++)
                {
                    tuple = (EffectTagTuple)e.NewItems[i];
                    cache[index + i] = cacher.CacheTag(tuple.Tag, tuple.Indices);
                }

                //tuple = (EffectTagTuple)e.OldItems[0];
                //Remove(tuple);
                //tuple = (EffectTagTuple)e.NewItems[0];
                //Add(tuple.Indices, tuple.Tag);
                break;
        }
    }

    public IEnumerable<T> GetCached(int index)
    {
        int i = 0;
        for (; i < cache.Count; i++)
        {
            T cached = cache[i];

            if (cached.Indices.StartIndex > index) yield break;

            if (cached.Indices.EndIndex > index || cached.Indices.IsOpen)
                yield return cached;
        }
    }
}


// TODO
// Better name for this?
//public class CachedCollection<T> where T : ITagWrapper
//{
//    private IntervalTree<OrderedIndex, T> lol;
//    private ObservableTagCollection tagCollection;
//    private ITagCacher<T> cacher;

//    public int Count => lol.Count;

//    public CachedCollection(ITagCacher<T> cacher, ObservableTagCollection tagCollection)
//    {
//        id = new OrderedIndex(0, int.MaxValue);
//        this.cacher = cacher;
//        this.tagCollection = tagCollection;
//        lol = new IntervalTree<OrderedIndex, T>(new OrderedIndexComparer());

//        foreach (var tagData in tagCollection)
//        {
//            Add(tagData.Indices, tagData.Tag);
//        }

//        tagCollection.CollectionChanged += OnCollectionChanged;
//    }

//    private void Add(EffectTagIndices indices, EffectTag tag)
//    {
//        lol.Add(new OrderedIndex(indices.StartIndex, indices.OrderAtIndex), new OrderedIndex(indices.EndIndex, indices.OrderAtIndex), cacher.CacheTag(tag, indices));
//    }

//    private void Remove(EffectTagTuple tuple)
//    {
//        Debug.Log("Im trying to remove Tag: " + tuple.Tag.Name + " Start: " + tuple.Indices.StartIndex + " with order " + tuple.Indices.OrderAtIndex + " and end " + tuple.Indices.EndIndex);

//        Debug.Log("will remove " + lol.Where(x => x.From.index == tuple.Indices.StartIndex && x.From.order == tuple.Indices.OrderAtIndex).Select(x => x.Value).Count() + " items; those being");
//        foreach (var ting in lol.Where(x => x.From.index == tuple.Indices.StartIndex && x.From.order == tuple.Indices.OrderAtIndex).Select(x => x.Value))
//        {
//            var tag = (ITagWrapper)ting;
//            Debug.Log("Element: " + tag.Tag.Name);
//        }

//        lol.RemoveWhere(x => x.From.index == tuple.Indices.StartIndex && x.From.order == tuple.Indices.OrderAtIndex);

//        Debug.Log("Post removal there are " + lol.Where(x => x.From.index == tuple.Indices.StartIndex && x.From.order == tuple.Indices.OrderAtIndex).Select(x => x.Value).Count() + " matching items");

//        var res = lol.Query(new OrderedIndex(tuple.Indices.StartIndex, tuple.Indices.OrderAtIndex));
//        Debug.Log("Query got me ");
//        foreach (var l in res)
//        {
//            Debug.Log(l.Tag.Name);
//        }

//    }

//    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
//    {
//        switch (e.Action)
//        {
//            case NotifyCollectionChangedAction.Add:
//                if (e.NewItems.Count > 1) Debug.LogWarning("Added more than one element; Should be impossible");
//                EffectTagTuple tuple = (EffectTagTuple)e.NewItems[0];
//                Add(tuple.Indices, tuple.Tag);
//                break;

//            case NotifyCollectionChangedAction.Remove:
//                foreach (var item in e.OldItems)
//                {
//                    tuple = (EffectTagTuple)item;
//                    Remove(tuple);
//                }
//                break;

//            case NotifyCollectionChangedAction.Reset:
//                lol.Clear();
//                break;

//            case NotifyCollectionChangedAction.Move:
//                throw new NotImplementedException();

//            case NotifyCollectionChangedAction.Replace:
//                tuple = (EffectTagTuple)e.OldItems[0];
//                Remove(tuple);
//                tuple = (EffectTagTuple)e.NewItems[0];
//                Add(tuple.Indices, tuple.Tag);
//                break;
//        }
//    }

//    private OrderedIndex id;
//    public IEnumerable<T> GetCached(int index)
//    {
//        id.index = index;
//        return lol.Query(id);
//    }

//    private struct OrderedIndex
//    {
//        public int index;
//        public int order;

//        public OrderedIndex(int index, int order)
//        {
//            this.index = index;
//            this.order = order;
//        }
//    }

//    private class OrderedIndexComparer : IComparer<OrderedIndex>
//    {
//        public int Compare(OrderedIndex x, OrderedIndex y)
//        {
//            //if (x.index == -1)
//            //{
//            //    if (y.index == -1) return 0;
//            //    return 1;
//            //}
//            //if (y.index == -1) 
//            //{
//            //    return -1;
//            //}
//            int res = (x.index.CompareTo(y.index));
//            if (res == 0) return x.order.CompareTo(y.order);

//            return res;
//        }
//    }
//}