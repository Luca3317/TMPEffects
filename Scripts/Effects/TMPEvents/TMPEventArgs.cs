using System;
using TMPEffects.Tags;

namespace TMPEffects.TMPEvents
{
    public class TMPEventArgs : EventArgs
    {
        public EffectTag Tag { get; private set; }
        public EffectTagIndices Indices { get; private set; }

        public TMPEventArgs(EffectTag tag, EffectTagIndices indices)
        {
            this.Tag = tag;
            this.Indices = indices;
        }
    }
}