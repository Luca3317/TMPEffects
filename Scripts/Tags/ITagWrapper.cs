using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.Tags
{
    /// <summary>
    /// Interface that wraps an <see cref="EffectTag"/> as well as an <see cref="EffectTagIndices"/> instance.
    /// </summary>
    public interface ITagWrapper
    {
        /// <summary>
        /// The wrapped tag.
        /// </summary>
        public EffectTag Tag { get; }
        /// <summary>
        /// The wrapped tag indices.
        /// </summary>
        public EffectTagIndices Indices { get; }
    }
}
