using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.Tags;
using UnityEngine;

namespace TMPEffects
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