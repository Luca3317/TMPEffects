using System;

namespace TMPEffects.Tags
{
    /// <summary>
    /// Readonly struct that combines an <see cref="EffectTag"/> and an <see cref="EffectTagIndices"/>.
    /// </summary>
    public struct EffectTagTuple : IEquatable<EffectTagTuple>
    {
        /// <summary>
        /// The tag.
        /// </summary>
        public readonly EffectTag Tag;
        /// <summary>
        /// The tag indices.
        /// </summary>
        public readonly EffectTagIndices Indices;

        public EffectTagTuple(EffectTag tag, EffectTagIndices indices)
        {
            Tag = tag;
            Indices = indices;
        }

        public bool Equals(EffectTagTuple other)
        {
            return Tag.Equals(other.Tag) && Indices.Equals(other.Indices);
        }
    }
}