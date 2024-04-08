using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.Tags
{
    /// <summary>
    /// Generic interface for caching tags.
    /// </summary>
    /// <typeparam name="T">Type that will be used to represent a cached tag.</typeparam>
    public interface ITagCacher<T> where T : ITagWrapper
    {
        /// <summary>
        /// Cache the given tag and its associated indices.
        /// </summary>
        /// <param name="tag">The tag to cache.</param>
        /// <param name="indices">The associated indices.</param>
        /// <returns>The cached tag.</returns>
        public T CacheTag(TMPEffectTag tag, TMPEffectTagIndices indices);
    }
}