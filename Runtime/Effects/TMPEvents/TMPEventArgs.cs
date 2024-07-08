using System;
using TMPEffects.Components;
using TMPEffects.Tags;

namespace TMPEffects.TMPEvents
{
    /// <summary>
    /// The argument object for <see cref="TMPEvent"/>.
    /// </summary>
    public class TMPEventArgs : EventArgs
    {
        /// <summary>
        /// The tag of the event.
        /// </summary>
        public TMPEffectTag Tag { get; private set; }
        /// <summary>
        /// The indices of the tag.
        /// </summary>
        public TMPEffectTagIndices Indices { get; private set; }
        /// <summary>
        /// The <see cref="TMPWriter"/> that invoked this event.
        /// </summary>
        public TMPWriter Writer { get; private set; }

        public TMPEventArgs(TMPEffectTag tag, TMPEffectTagIndices indices, TMPWriter writer)
        {
            this.Tag = tag;
            this.Indices = indices;
            this.Writer = writer;
        }
    }
}