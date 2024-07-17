using System;

namespace TMPEffects.Tags
{
    /// <summary>
    /// Readonly struct that combines an <see cref="TMPEffectTag"/> and an <see cref="TMPEffectTagIndices"/>.
    /// </summary>
    public struct TMPEffectTagTuple : IEquatable<TMPEffectTagTuple>
    {
        /// <summary>
        /// The tag.
        /// </summary>
        public readonly TMPEffectTag Tag;
        /// <summary>
        /// The tag indices.
        /// </summary>
        public readonly TMPEffectTagIndices Indices;

        public TMPEffectTagTuple(TMPEffectTag tag, TMPEffectTagIndices indices)
        {
            Tag = tag;
            Indices = indices;
        }

        public bool Equals(TMPEffectTagTuple other)
        {
            return Tag.Equals(other.Tag) && Indices.Equals(other.Indices);
        }
    }
}