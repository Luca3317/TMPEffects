using System.Collections;
using System.Collections.Generic;

namespace TMPEffects.Tags.Collections
{
    ///<inheritdoc/>
    public class ReadOnlyTagCollection : IReadOnlyTagCollection
    {
        internal ReadOnlyTagCollection(List<TMPEffectTagTuple> tags)
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
        public bool Contains(TMPEffectTag tag, TMPEffectTagIndices? indices = null)
        {
            return collection.Contains(tag, indices);
        }

        public IEnumerator<TMPEffectTagTuple> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        ///<inheritdoc/>
        public TMPEffectTagIndices? IndicesOf(TMPEffectTag tag)
        {
            return collection.IndicesOf(tag);
        }

        ///<inheritdoc/>
        public TMPEffectTag TagAt(int startIndex, int? order = null)
        {
            return collection.TagAt(startIndex, order);
        }

        ///<inheritdoc/>
        public int TagsAt(int startIndex, TMPEffectTagTuple[] buffer, int bufferIndex = 0)
        {
            return collection.TagsAt(startIndex, buffer, bufferIndex);
        }

        ///<inheritdoc/>
        public IEnumerable<TMPEffectTagTuple> TagsAt(int startIndex)
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