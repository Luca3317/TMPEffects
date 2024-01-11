using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.Tags;
using UnityEngine;

namespace TMPEffects
{
    public class TMPEventArgs : EventArgs
    {
        public TMPEventTag tag { get; private set; }

        public TMPEventArgs(TMPEventTag tag)
        {
            this.tag = tag;
        }
    }
}