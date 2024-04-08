using System.Collections.Generic;

namespace TMPEffects.Tags.Collections
{
    /// <summary>
    /// A readonly collection of <see cref="TMPEffectTagTuple"/>.
    /// </summary>
    public interface IReadOnlyTagCollection : IReadOnlyCollection<TMPEffectTagTuple>
    {
        /// <summary>
        /// The amount of <see cref="TMPEffectTagTuple"/> in this collection.
        /// </summary>
        public int TagCount { get; }

        /// <summary>
        /// Whether this collection contains the given tag with the given indices (if supplied).
        /// </summary>
        /// <param name="tag">The tag to check.</param>
        /// <param name="indices">The tag indices. If you don't care about the tag's indices, leave as default.</param>
        /// <returns>true if this collection contains the given tag with the given indices (if supplied); otherwise false.</returns>
        public bool Contains(TMPEffectTag tag, TMPEffectTagIndices? indices = null);

        /// <summary>
        /// Get the associated indices of the given tag.
        /// </summary>
        /// <param name="tag">The tag to get the indices of.</param>
        /// <returns>The indices of the given tag, if the tag is contained in the collection; otherwise null.</returns>
        public TMPEffectTagIndices? IndicesOf(TMPEffectTag tag);

        /// <summary>
        /// Get the tags starting at the given <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="buffer">The buffer the tags will be stored in.</param>
        /// <param name="bufferIndex">The offset index of the buffer, i.e. the index at which the tags should be inserted into <paramref name="buffer"/>.</param>
        /// <returns>The amount of tags starting at the given <paramref name="startIndex"/>.</returns>
        public int TagsAt(int startIndex, TMPEffectTagTuple[] buffer, int bufferIndex = 0);
        /// <summary>
        /// Get the tags starting at the given <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <returns>All tags starting at the given <paramref name="startIndex"/>.</returns>
        public IEnumerable<TMPEffectTagTuple> TagsAt(int startIndex);
        /// <summary>
        /// Get the tag at the given <paramref name="startIndex"/> with the correct <paramref name="order"/>.<br/>
        /// If <paramref name="order"/> is left to default, the first tag at <paramref name="startIndex"/> will be returned.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="order">The order at the start index.</param>
        /// <returns>The tag at the given indices, if it exists; otherwise null.</returns>
        public TMPEffectTag TagAt(int startIndex, int? order = null);

        int IReadOnlyCollection<TMPEffectTagTuple>.Count => TagCount;
    }
}