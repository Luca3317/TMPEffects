using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.Tags;
using TMPEffects.TMPEvents;
using UnityEngine;

namespace TMPEffects.Components.Writer
{
    internal class CachedEvent : ITagWrapper, ICachedInvokable
    {
        public TMPEffectTag Tag => args.Tag;
        public TMPEffectTagIndices Indices => args.Indices;

        public TMPEventArgs args { get; private set; }
        public bool Triggered { get; private set; }

        public bool ExecuteInstantly => false;
        public bool ExecuteOnSkip => true;
        public bool ExecuteRepeatable => true;
        public bool ExecuteInPreview => true;

        private TMPEvent tmpEvent;

        public void Trigger()
        {
            if (Triggered) return;

            Triggered = true;
            tmpEvent.Invoke(args);
        }

        public void Reset()
        {
            Triggered = false;
        }

        public CachedEvent(TMPEventArgs args, TMPEvent tmpEvent) => Reset(args, tmpEvent);
        public void Reset(TMPEventArgs args, TMPEvent tmpEvent)
        {
            this.tmpEvent = tmpEvent;
            this.args = args;
            this.Triggered = false;
        }
    }
}
