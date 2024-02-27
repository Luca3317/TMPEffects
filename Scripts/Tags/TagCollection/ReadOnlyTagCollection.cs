using System.Collections;
using System.Collections.Generic;

namespace TMPEffects.Tags.Collections
{
    ///<inheritdoc/>
    public class ReadOnlyTagCollection : IReadOnlyTagCollection
    {
        internal ReadOnlyTagCollection(List<EffectTagTuple> tags)
        {
            this.collection = new TagCollection(tags);
        }

        internal ReadOnlyTagCollection(IReadOnlyTagCollection collection)
        {
            this.collection = collection;
        }

        ///<inheritdoc/>
        public int TagCount => collection.TagCount;

        ///<inheritdoc/>
        public bool Contains(EffectTag tag, EffectTagIndices? indices = null)
        {
            return collection.Contains(tag, indices);
        }

        public IEnumerator<EffectTagTuple> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        ///<inheritdoc/>
        public EffectTagIndices? IndicesOf(EffectTag tag)
        {
            return collection.IndicesOf(tag);
        }

        ///<inheritdoc/>
        public EffectTag TagAt(int startIndex, int? order = null)
        {
            return collection.TagAt(startIndex, order);
        }

        ///<inheritdoc/>
        public int TagsAt(int startIndex, EffectTagTuple[] buffer, int bufferIndex = 0)
        {
            return collection.TagsAt(startIndex, buffer, bufferIndex);
        }

        ///<inheritdoc/>
        public IEnumerable<EffectTagTuple> TagsAt(int startIndex)
        {
            return collection.TagsAt(startIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        private IReadOnlyTagCollection collection;
    }
}