using System.Collections;
using System.Collections.Generic;
using TMPEffects.Tags.Collections;
using UnityEngine;

namespace TMPEffects.Tags.Collections
{
    /// <summary>
    /// Interface for a manager of <see cref="ITagCollection"/>.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface ITagCollectionManager<TKey>
    {
        /// <summary>
        /// Get the <see cref="ITagCollection"/> associated with the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="ITagCollection"/> associated with the given key.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public ITagCollection this[TKey key] { get; }
        /// <summary>
        /// Add a key. This will automatically create an associated <see cref="ITagCollection"/>.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <returns>The automatically created <see cref="ITagCollection"/>.</returns>
        public ITagCollection AddKey(TKey key);
        /// <summary>
        /// Remove a key.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>true if the key was removed; false otherwise.</returns>
        public bool RemoveKey(TKey key);
    }
}