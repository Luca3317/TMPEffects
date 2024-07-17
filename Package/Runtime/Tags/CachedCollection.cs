using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace TMPEffects.Tags.Collections
{
    /// <summary>
    /// Manages a collection of <see cref="ITagWrapper"/> that represent a cached tag.<br/>
    /// Will keep itself synchronized with the given <see cref="ObservableTagCollection"/>.<br/>
    /// </summary>
    /// <remarks>
    /// Designed for fast "Get tags that contain index" operations.<br/>
    /// Ideally use along with <see cref="TagCollectionManager{TKey}"/> to get the <see cref="ObservableTagCollection"/> 
    /// (see implementations of both <see cref="Components.TMPAnimator"/> and <see cref="Components.TMPWriter"/> for examples).
    /// </remarks>
    /// <typeparam name="T">The type of tag wrapper / cached tags.</typeparam>
    public class CachedCollection<T> : IEnumerable<T> where T : ITagWrapper
    {
        /// <summary>
        /// Amount of cached tags contained in this collection.
        /// </summary>
        public int Count => cache.Count;

        public CachedCollection(ITagCacher<T> cacher, ObservableTagCollection tagCollection)
        {
            if (cacher == null) throw new System.ArgumentNullException(nameof(cacher));
            if (tagCollection == null) throw new System.ArgumentNullException(nameof(tagCollection));

            this.cacher = cacher;

            List<T> cached = new List<T>();
            foreach (var tagData in tagCollection)
            {
                cached.Add(cacher.CacheTag(tagData.Tag, tagData.Indices));
            }

            int counter = 0;
            foreach (var tagData in tagCollection)
            {
                Add(counter, cached[counter]);
                counter++;
            }

            tagCollection.CollectionChanged += OnCollectionChanged;
        }

        /// <summary>
        /// The minimum and maximum collection index of cached tags potentially relevant to a given text index.
        /// </summary>
        public class MinMax
        {
            /// <summary>
            /// The maximum collection index of cached tags potentially relevant to a given text index.
            /// </summary>
            public int MaxIndex;
            /// <summary>
            /// The minimum collection index of cached tags potentially relevant to a given text index.
            /// </summary>
            public int MinIndex;

            public MinMax(int textIndex)
            {
                MaxIndex = textIndex;
                MinIndex = textIndex;
            }
        }

        /// <summary>
        /// Get the <see cref="MinMax"/> for a text index.
        /// </summary>
        /// <param name="textIndex">The text index.</param>
        /// <returns>The <see cref="MinMax"/> for the text index, if one exists; otherwise null.</returns>
        public MinMax MinMaxAt(int textIndex)
        {
            if (!minMax.TryGetValue(textIndex, out var mm))
            {
                return null;
            }
            return mm;
        }

        /// <summary>
        /// Get the cached tag of type <see cref="T"/> at the given collection index.
        /// </summary>
        /// <param name="index">The collection index.</param>
        /// <returns>The cached tag of type <see cref="T"/> at the given collection index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public T this[int index]
        {
            get => cache[index];
        }

        /// <summary>
        /// Whether this collection has any cached tags.
        /// </summary>
        /// <returns>true if there is at least one cached tag; false otherwise.</returns>
        public bool HasAny() => cache.Count > 0;
        /// <summary>
        /// Whether this collection has any cached tags that contain the given text index.
        /// </summary>
        /// <param name="textIndex">The text index.</param>
        /// <returns>true if there is at least one cached tag that contains the given text index; false otherwise.</returns>
        public bool HasAnyContaining(int textIndex)
        {
            if (textIndex < min) return false;
            if (textIndex > max) return false;
            return minMax.ContainsKey(textIndex);
        }
        /// <summary>
        /// Whether this collection has any cached tags that start at the given text index.
        /// </summary>
        /// <param name="textIndex">The text index.</param>
        /// <returns>true if there is at least one cached tag that starts at the given text index; false otherwise.</returns>
        public bool HasAnyAt(int index)
        {
            if (!minMax.TryGetValue(index, out MinMax mm))
            {
                return false;
            }

            for (int i = mm.MinIndex; i <= mm.MaxIndex; i++)
            {
                if (cache[i].Indices.StartIndex == index) return true;
            }

            return false;
        }

        /// <summary>
        /// Enumerates all cached tags that contain the given text index.
        /// </summary>
        /// <param name="textIndex">The text index.</param>
        /// <returns>All cached tags that contain the given text index.</returns>
        public IEnumerable<T> GetContaining(int textIndex)
        {
            if (!minMax.TryGetValue(textIndex, out MinMax mm))
            {
                yield break;
            }

            for (int i = mm.MinIndex; i <= mm.MaxIndex; i++)
            {
                T cached = cache[i];

                if (cached.Indices.StartIndex > textIndex) yield break;

                if (cached.Indices.Contains(textIndex)) yield return cached;
            }
        }
        /// <summary>
        /// Enumerates all cached tags that start at the given text index.
        /// </summary>
        /// <param name="textIndex">The text index.</param>
        /// <returns>All cached tags that start at the given text index.</returns>
        public IEnumerable<T> GetAt(int textIndex)
        {
            if (!minMax.TryGetValue(textIndex, out MinMax mm))
            {
                yield break;
            }

            for (int i = mm.MinIndex; i <= mm.MaxIndex; i++)
            {
                T cached = cache[i];

                if (cached.Indices.StartIndex > textIndex)
                {
                    yield break;
                }

                if (cached.Indices.StartIndex == textIndex)
                {
                    yield return cached;
                }
            }
        }
        /// <summary>
        /// Enumerates all cached tags that contain the given text index, without allocating any memory.
        /// </summary>
        /// <param name="textIndex">The text index.</param>
        /// <returns>All cached tags that contain the given text index.</returns>
        public StructContainingEnumerable GetContaining_NonAlloc(int textIndex)
        {
            if (!minMax.TryGetValue(textIndex, out MinMax mm))
            {
                return new StructContainingEnumerable(null, 0, 0, 0);
            }

            return new StructContainingEnumerable(cache, textIndex, mm.MaxIndex, mm.MinIndex);
        }
        /// <summary>
        /// Enumerates all cached tags that contain the given text index in reversed order, without allocating any memory.
        /// </summary>
        /// <param name="textIndex">The text index.</param>
        /// <returns>All cached tags that contain the given text index.</returns>
        public StructReversedContainingEnumerable GetContainingReversed_NonAlloc(int textIndex)
        {
            if (!minMax.TryGetValue(textIndex, out MinMax mm))
            {
                return new StructReversedContainingEnumerable(null, 0, 0, 0);
            }

            return new StructReversedContainingEnumerable(cache, textIndex, mm.MaxIndex, mm.MinIndex);
        }

        /// <summary>
        /// Helper struct for <see cref="GetContainingReversed_NonAlloc(int)"/>.
        /// </summary>
        public struct StructReversedContainingEnumerable
        {
            private readonly List<T> pool;
            private int containedIndex;
            private int minIndex;
            private int maxIndex;

            public StructReversedContainingEnumerable(List<T> pool, int containedIndex, int maxIndex, int minIndex)
            {
                this.pool = pool;
                this.containedIndex = containedIndex;
                this.minIndex = minIndex;
                this.maxIndex = maxIndex;
            }

            public StructReversedContainingEnumerator GetEnumerator()
            {
                return new StructReversedContainingEnumerator(this.pool, containedIndex, maxIndex, minIndex);
            }
        }

        /// <summary>
        /// Helper struct for <see cref="GetContaining_NonAlloc(int)(int)"/>.
        /// </summary>
        public struct StructContainingEnumerable
        {
            private readonly List<T> pool;
            private int containedIndex;
            private int minIndex;
            private int maxIndex;

            public StructContainingEnumerable(List<T> pool, int containedIndex, int maxIndex, int minIndex)
            {
                this.pool = pool;
                this.containedIndex = containedIndex;
                this.minIndex = minIndex;
                this.maxIndex = maxIndex;
            }

            public StructContainingEnumerator GetEnumerator()
            {
                return new StructContainingEnumerator(this.pool, containedIndex, maxIndex, minIndex);
            }
        }

        /// <summary>
        /// Helper struct for <see cref="GetContainingReversed_NonAlloc(int)"/>.
        /// </summary>
        public struct StructReversedContainingEnumerator
        {
            private readonly List<T> pool;
            private readonly int containedIndex;
            private readonly int maxIndex;
            private readonly int minIndex;
            private int index;

            internal StructReversedContainingEnumerator(List<T> pool, int containedIndex, int maxIndex, int minIndex)
            {
                this.pool = pool;
                this.containedIndex = containedIndex;
                this.index = maxIndex + 1;
                this.maxIndex = maxIndex;
                this.minIndex = minIndex;
            }

            public T Current
            {
                get
                {
                    return this.pool[this.index];
                }
            }

            public bool MoveNext()
            {
                if (pool == null) return false;
                while (--index >= minIndex && !pool[index].Indices.Contains(containedIndex)) { }
                return this.minIndex <= this.index;
            }

            public void Reset()
            {
                this.index = maxIndex + 1;
            }
        }

        /// <summary>
        /// Helper struct for <see cref="GetContaining_NonAlloc(int)(int)"/>.
        /// </summary>
        public struct StructContainingEnumerator
        {
            private readonly List<T> pool;
            private readonly int containedIndex;
            private readonly int maxIndex;
            private readonly int minIndex;
            private int index;

            internal StructContainingEnumerator(List<T> pool, int containedIndex, int maxIndex, int minIndex)
            {
                this.pool = pool;
                this.containedIndex = containedIndex;
                this.index = minIndex - 1;
                this.maxIndex = maxIndex;
                this.minIndex = minIndex;
            }

            public T Current
            {
                get
                {
                    return this.pool[this.index];
                }
            }

            public bool MoveNext()
            {
                if (pool == null) return false;
                while (++index <= maxIndex && !pool[index].Indices.Contains(containedIndex)) { }
                return this.maxIndex >= this.index;
            }

            public void Reset()
            {
                this.index = minIndex - 1;
            }
        }

        private Dictionary<int, MinMax> minMax = new();

        private List<T> cache = new List<T>();
        private ITagCacher<T> cacher;

        private int max = int.MinValue;
        private int min = int.MaxValue;

        private void Add(int cachedIndex, T tuple)
        {
            foreach (var kvp in minMax)
            {
                if (kvp.Value.MinIndex >= cachedIndex)
                {
                    kvp.Value.MinIndex += 1;
                }
                if (kvp.Value.MaxIndex >= cachedIndex)
                {
                    kvp.Value.MaxIndex += 1;
                }
            }

            for (int index = tuple.Indices.StartIndex; index < tuple.Indices.EndIndex; index++)
            {
                if (!minMax.TryGetValue(index, out MinMax mm))
                {
                    minMax.Add(index, new MinMax(cachedIndex));
                    continue;
                }

                if (mm.MaxIndex < cachedIndex)
                {
                    mm.MaxIndex = cachedIndex;
                }
                if (mm.MinIndex > cachedIndex)
                {
                    mm.MinIndex = cachedIndex;
                }
            }

            if (tuple.Indices.EndIndex > max) max = tuple.Indices.EndIndex;
            if (tuple.Indices.StartIndex < min) min = tuple.Indices.StartIndex;

            cache.Insert(cachedIndex, tuple);
        }

        private void Remove(int cachedIndex)
        {
            T tuple = cache[cachedIndex];

            for (int index = tuple.Indices.StartIndex; index < tuple.Indices.EndIndex; index++)
            {
                MinMax mm = minMax[index];

                // If removing the tag that serves as min tag for the current index
                if (mm.MinIndex == cachedIndex)
                {
                    // If this tag also serves as max tag => is the only tag for the current index
                    if (mm.MaxIndex == cachedIndex)
                    {
                        minMax.Remove(index);
                        continue;
                    }

                    bool found = false;

                    // Find new min tag
                    for (int i = mm.MinIndex + 1; i <= mm.MaxIndex; i++)
                    {
                        if (cache[i].Indices.Contains(index))
                        {
                            mm.MinIndex = i;
                            found = true;
                            break;
                        }
                    }

                    if (!found) Debug.LogError("Failed to find new min tag; BUG");

                }
                else if (mm.MaxIndex == cachedIndex)
                {
                    // Find new max tag

                    bool found = false;

                    for (int i = mm.MaxIndex - 1; i >= mm.MinIndex; i--)
                    {
                        if (cache[i].Indices.Contains(index))
                        {
                            mm.MaxIndex = i;
                            found = true;
                            break;
                        }
                    }

                    if (!found) Debug.LogError("Failed to find new max tag; BUG");
                }
            }

            foreach (var kvp in minMax)
            {
                if (kvp.Value.MinIndex > cachedIndex)
                {
                    kvp.Value.MinIndex -= 1;
                }
                if (kvp.Value.MaxIndex > cachedIndex)
                {
                    kvp.Value.MaxIndex -= 1;
                }
            }

            cache.RemoveAt(cachedIndex);

            // Update max/min
            if (cache.Count == 0)
            {
                max = int.MinValue;
                min = int.MaxValue;
                return;
            }
            min = cache[0].Indices.StartIndex;
            if (tuple.Indices.EndIndex == max)
            {
                max = int.MinValue;
                foreach (var i in cache)
                {
                    if (i.Indices.EndIndex > max)
                        max = i.Indices.EndIndex;
                }
            }
        }

        private void Set(int cachedIndex, T tuple)
        {
            Remove(cachedIndex);
            Add(cachedIndex, tuple);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems.Count > 1) Debug.LogWarning("Added more than one element; Should be impossible");
                    var tuple = (TMPEffectTagTuple)e.NewItems[0];
                    Add(e.NewStartingIndex, cacher.CacheTag(tuple.Tag, tuple.Indices));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    int index = e.OldStartingIndex;
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        Remove(index);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    cache.TrimExcess();
                    minMax.TrimExcess();
                    cache.Clear();
                    minMax.Clear();
                    break;

                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();

                case NotifyCollectionChangedAction.Replace:
                    index = e.NewStartingIndex;

                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        tuple = (TMPEffectTagTuple)e.NewItems[i];
                        Set(index + i, cacher.CacheTag(tuple.Tag, tuple.Indices));
                    }
                    break;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return cache.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return cache.GetEnumerator();
        }
    }
}
