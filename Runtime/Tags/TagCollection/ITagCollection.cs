using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.Tags.Collections
{
    /// <summary>
    /// A writable collection of <see cref="TMPEffectTagTuple"/>.
    /// </summary>
    public interface ITagCollection : IReadOnlyTagCollection
    {
        /// <summary>
        /// Attempt to add a new tag to the collection.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        /// <param name="indices">The tag's indices.</param>
        /// <returns>true if the tag was successfully added; false otherwise.</returns>
        public bool TryAdd(TMPEffectTag tag, TMPEffectTagIndices indices);
        /// <summary>
        /// Attempt to add a new tag to the collection.<br/>
        /// If <paramref name="orderAtIndex"/> is left to default, the order will be assigned so it is the first tag at the given <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        /// <param name="startIndex">The tag's start index.</param>
        /// <param name="endIndex">The tag's end index.</param>
        /// <param name="orderAtIndex">The tag's order at index.</param>
        /// <returns>true if the tag was successfully added; false otherwise.</returns>
        public bool TryAdd(TMPEffectTag tag, int startIndex = 0, int endIndex = -1, int? orderAtIndex = null);

        /// <summary>
        /// Remove all tags starting at the given <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="buffer">Buffer to save the removed tags into. Leave at default if you do not want to get the tags.</param>
        /// <param name="bufferIndex">The offset index of the buffer, i.e. the index at which the tags should be inserted into <paramref name="buffer"/>.</param>
        /// <returns>The amount of removed tags.</returns>
        public int RemoveAllAt(int startIndex, TMPEffectTagTuple[] buffer = null, int bufferIndex = 0);
        /// <summary>
        /// Remove the tag starting at the given <paramref name="startIndex"/> with the correct <paramref name="order"/>.<br/>
        /// If <paramref name="order"/> is left to default, the first tag at <paramref name="startIndex"/> will be removed.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="order">The order at the start index.</param>
        /// <returns>true if a tag was removed; false otherwise.</returns>
        public bool RemoveAt(int startIndex, int? order = null);

        /// <summary>
        /// Remove the given tag, with the specified indices (if supplied).
        /// </summary>
        /// <param name="tag">The tag to remove.</param>
        /// <param name="indices">The indices of the tag.</param>
        /// <returns>true if the tag was removed; false otherwise.</returns>
        public bool Remove(TMPEffectTag tag, TMPEffectTagIndices? indices = null);

        /// <summary>
        /// Clear the entire collection.
        /// </summary>
        public void Clear();
    }
}