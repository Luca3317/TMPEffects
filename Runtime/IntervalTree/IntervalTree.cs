using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TMPEffects.Intervaltree
{
    /// <summary>
    ///     A node of the range tree. Given a list of items, it builds
    ///     its subtree. Also contains methods to query the subtree.
    ///     Basically, all interval tree logic is here.
    /// </summary>
    internal class IntervalTreeNode<TKey, TValue> : IComparer<RangeValuePair<TKey, TValue>>
    {
        private readonly TKey center;

        private readonly IComparer<TKey> comparer;
        private readonly RangeValuePair<TKey, TValue>[] items;
        private readonly IntervalTreeNode<TKey, TValue> leftNode;
        private readonly IntervalTreeNode<TKey, TValue> rightNode;

        /// <summary>
        ///     Initializes an empty node.
        /// </summary>
        /// <param name="comparer">The comparer used to compare two items.</param>
        public IntervalTreeNode(IComparer<TKey> comparer)
        {
            this.comparer = comparer ?? Comparer<TKey>.Default;

            center = default;
            leftNode = null;
            rightNode = null;
            items = null;
        }

        /// <summary>
        ///     Initializes a node with a list of items, builds the sub tree.
        /// </summary>
        /// <param name="items">The items that should be added to this node</param>
        /// <param name="comparer">The comparer used to compare two items.</param>
        public IntervalTreeNode(IList<RangeValuePair<TKey, TValue>> items, IComparer<TKey> comparer)
        {
            this.comparer = comparer ?? Comparer<TKey>.Default;

            // first, find the median
            var endPoints = new List<TKey>(items.Count * 2);
            foreach (var item in items)
            {
                endPoints.Add(item.From);
                endPoints.Add(item.To);
            }

            endPoints.Sort(this.comparer);

            // the median is used as center value
            if (endPoints.Count > 0)
            {
                Min = endPoints[0];
                center = endPoints[endPoints.Count / 2];
                Max = endPoints[endPoints.Count - 1];
            }

            var inner = new List<RangeValuePair<TKey, TValue>>();
            var left = new List<RangeValuePair<TKey, TValue>>();
            var right = new List<RangeValuePair<TKey, TValue>>();

            // iterate over all items
            // if the range of an item is completely left of the center, add it to the left items
            // if it is on the right of the center, add it to the right items
            // otherwise (range overlaps the center), add the item to this node's items
            foreach (var o in items)
                if (this.comparer.Compare(o.To, center) < 0)
                    left.Add(o);
                else if (this.comparer.Compare(o.From, center) > 0)
                    right.Add(o);
                else
                    inner.Add(o);

            // sort the items, this way the query is faster later on
            if (inner.Count > 0)
            {
                if (inner.Count > 1)
                    inner.Sort(this);
                this.items = inner.ToArray();
            }
            else
            {
                this.items = null;
            }

            // create left and right nodes, if there are any items
            if (left.Count > 0)
                leftNode = new IntervalTreeNode<TKey, TValue>(left, this.comparer);
            if (right.Count > 0)
                rightNode = new IntervalTreeNode<TKey, TValue>(right, this.comparer);
        }

        public TKey Max { get; }

        public TKey Min { get; }

        /// <summary>
        ///     Returns less than 0 if this range's From is less than the other, greater than 0 if greater.
        ///     If both are equal, the comparison of the To values is returned.
        ///     0 if both ranges are equal.
        /// </summary>
        /// <param name="x">The first item.</param>
        /// <param name="y">The other item.</param>
        /// <returns></returns>
        int IComparer<RangeValuePair<TKey, TValue>>.Compare(RangeValuePair<TKey, TValue> x,
            RangeValuePair<TKey, TValue> y)
        {
            var fromComp = comparer.Compare(x.From, y.From);
            if (fromComp == 0)
                return comparer.Compare(x.To, y.To);
            return fromComp;
        }

        /// <summary>
        ///     Performs a point query with a single value.
        ///     All items with overlapping ranges are returned.
        /// </summary>
        public IEnumerable<TValue> Query(TKey value)
        {
            var results = new List<TValue>();

            // If the node has items, check for leaves containing the value.
            if (items != null)
                foreach (var o in items)
                    if (comparer.Compare(o.From, value) > 0)
                        break;
                    else if (comparer.Compare(value, o.From) >= 0 && comparer.Compare(value, o.To) <= 0)
                        results.Add(o.Value);

            // go to the left or go to the right of the tree, depending
            // where the query value lies compared to the center
            var centerComp = comparer.Compare(value, center);
            if (leftNode != null && centerComp < 0)
                results.AddRange(leftNode.Query(value));
            else if (rightNode != null && centerComp > 0)
                results.AddRange(rightNode.Query(value));

            return results;
        }

        /// <summary>
        ///     Performs a range query.
        ///     All items with overlapping ranges are returned.
        /// </summary>
        public IEnumerable<TValue> Query(TKey from, TKey to)
        {
            var results = new List<TValue>();

            // If the node has items, check for leaves intersecting the range.
            if (items != null)
                foreach (var o in items)
                    if (comparer.Compare(o.From, to) > 0)
                        break;
                    else if (comparer.Compare(to, o.From) >= 0 && comparer.Compare(from, o.To) <= 0)
                        results.Add(o.Value);

            // go to the left or go to the right of the tree, depending
            // where the query value lies compared to the center
            if (leftNode != null && comparer.Compare(from, center) < 0)
                results.AddRange(leftNode.Query(from, to));
            if (rightNode != null && comparer.Compare(to, center) > 0)
                results.AddRange(rightNode.Query(from, to));

            return results;
        }
    }

    /// <summary>
    /// Represents a range of values.
    /// Both values must be of the same type and comparable.
    /// </summary>
    /// <typeparam name="TKey">Type of the values.</typeparam>
    public readonly struct RangeValuePair<TKey, TValue> : IEquatable<RangeValuePair<TKey, TValue>>
    {
        public TKey From { get; }
        public TKey To { get; }
        public TValue Value { get; }

        /// <summary>
        /// Initializes a new <see cref="RangeValuePair&lt;TKey, TValue&gt;"/> instance.
        /// </summary>
        public RangeValuePair(TKey from, TKey to, TValue value) : this()
        {
            From = from;
            To = to;
            Value = value;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[{0} - {1}] {2}", From, To, Value);
        }

        public override int GetHashCode()
        {
            var hash = 23;
            if (From != null)
                hash = hash * 37 + From.GetHashCode();
            if (To != null)
                hash = hash * 37 + To.GetHashCode();
            if (Value != null)
                hash = hash * 37 + Value.GetHashCode();
            return hash;
        }

        public bool Equals(RangeValuePair<TKey, TValue> other)
        {
            return EqualityComparer<TKey>.Default.Equals(From, other.From)
                   && EqualityComparer<TKey>.Default.Equals(To, other.To)
                   && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RangeValuePair<TKey, TValue>))
                return false;

            return Equals((RangeValuePair<TKey, TValue>)obj);
        }

        public static bool operator ==(RangeValuePair<TKey, TValue> left, RangeValuePair<TKey, TValue> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RangeValuePair<TKey, TValue> left, RangeValuePair<TKey, TValue> right)
        {
            return !(left == right);
        }
    }

    /// <summary>
    /// The standard interval tree implementation. Keeps a root node and forwards all queries to it.
    /// Whenever new items are added or items are removed, the tree goes temporarily "out of sync", which means that the
    /// internal index is not updated immediately, but upon the next query operation.    
    /// </summary>
    /// <typeparam name="TKey">The type of the range.</typeparam>
    /// <typeparam name="TValue">The type of the data items.</typeparam>
    public interface IIntervalTree<TKey, TValue> : IEnumerable<RangeValuePair<TKey, TValue>>
    {
        /// <summary>
        /// Returns all items contained in the tree.
        /// </summary>
        IEnumerable<TValue> Values { get; }

        /// <summary>
        /// Gets the number of elements contained in the tree.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Performs a point query with a single value. All items with overlapping ranges are returned.
        /// </summary>
        IEnumerable<TValue> Query(TKey value);

        /// <summary>
        /// Performs a range query. All items with overlapping ranges are returned.
        /// </summary>
        IEnumerable<TValue> Query(TKey from, TKey to);

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        void Add(TKey from, TKey to, TValue value);

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        void Remove(TValue item);

        /// <summary>
        /// Removes the specified items.
        /// </summary>
        void Remove(IEnumerable<TValue> items);

        /// <summary>
        /// Removes all elements from the range tree.
        /// </summary>
        void Clear();
    }

    public class IntervalTree<TKey, TValue> : IIntervalTree<TKey, TValue>
    {
        private IntervalTreeNode<TKey, TValue> root;
        private List<RangeValuePair<TKey, TValue>> items;
        private readonly IComparer<TKey> comparer;
        private bool isInSync;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public TKey Max
        {
            get
            {
                if (!isInSync)
                    Rebuild();

                return root.Max;
            }
        }

        public TKey Min
        {
            get
            {
                if (!isInSync)
                    Rebuild();

                return root.Min;
            }
        }

        public IEnumerable<TValue> Values => items.Select(i => i.Value);

        public int Count => items.Count;

        /// <summary>
        /// Initializes an empty tree.
        /// </summary>
        public IntervalTree() : this(Comparer<TKey>.Default) { }

        /// <summary>
        /// Initializes an empty tree.
        /// </summary>
        public IntervalTree(IComparer<TKey> comparer)
        {
            this.comparer = comparer ?? Comparer<TKey>.Default;
            isInSync = true;
            root = new IntervalTreeNode<TKey, TValue>(this.comparer);
            items = new List<RangeValuePair<TKey, TValue>>();
        }

        public IEnumerable<TValue> Query(TKey value)
        {
            if (!isInSync)
                Rebuild();

            return root.Query(value);
        }

        public IEnumerable<TValue> Query(TKey from, TKey to)
        {
            if (!isInSync)
                Rebuild();

            return root.Query(from, to);
        }

        public void Add(TKey from, TKey to, TValue value)
        {
            if (comparer.Compare(from, to) > 0)
                throw new ArgumentOutOfRangeException($"{nameof(from)} cannot be larger than {nameof(to)}");

            isInSync = false;
            items.Add(new RangeValuePair<TKey, TValue>(from, to, value));
        }

        public void Remove(TValue value)
        {
            isInSync = false;
            items = items.Where(l => !l.Value.Equals(value)).ToList();
        }

        public void Remove(IEnumerable<TValue> items)
        {
            isInSync = false;
            this.items = this.items.Where(l => !items.Contains(l.Value)).ToList();
        }

        public void Clear()
        {
            root = new IntervalTreeNode<TKey, TValue>(comparer);
            items = new List<RangeValuePair<TKey, TValue>>();
            isInSync = true;
        }

        public IEnumerator<RangeValuePair<TKey, TValue>> GetEnumerator()
        {
            if (!isInSync)
                Rebuild();

            return items.GetEnumerator();
        }

        private void Rebuild()
        {
            if (isInSync)
                return;

            if (items.Count > 0)
                root = new IntervalTreeNode<TKey, TValue>(items, comparer);
            else
                root = new IntervalTreeNode<TKey, TValue>(comparer);
            isInSync = true;
        }
    }
}