using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.Tags
{
    /// <summary>
    /// Interface that wraps an <see cref="TMPEffectTag"/> as well as an <see cref="TMPEffectTagIndices"/> instance.
    /// </summary>
    public interface ITagWrapper
    {
        /// <summary>
        /// The wrapped tag.
        /// </summary>
        public TMPEffectTag Tag { get; }
        /// <summary>
        /// The wrapped tag indices.
        /// </summary>
        public TMPEffectTagIndices Indices { get; }
    }
}
