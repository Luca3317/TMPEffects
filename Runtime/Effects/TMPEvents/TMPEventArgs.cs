using System;
using TMPEffects.Components;
using TMPEffects.Tags;

namespace TMPEffects.TMPEvents
{
    public class TMPEventArgs : EventArgs
    {
        public TMPEffectTag Tag { get; private set; }
        public TMPEffectTagIndices Indices { get; private set; }
        public TMPWriter Writer { get; private set; }

        public TMPEventArgs(TMPEffectTag tag, TMPEffectTagIndices indices, TMPWriter writer)
        {
            this.Tag = tag;
            this.Indices = indices;
            this.Writer = writer;
        }
    }
}