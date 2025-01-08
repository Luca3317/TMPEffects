using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

namespace TMPEffects.Timeline.Markers
{
    [CustomStyle("TMPWriterWaitMarkerStyle")]
    [DisplayName("TMPEffects Marker/TMPWriter/Wait")]
    public class TMPWriterWaitMarker : TMPEffectsMarker
    {
        public override PropertyName id => new PropertyName();

        public override NotificationFlags flags =>
            (retroactive ? NotificationFlags.Retroactive : default) |
            (triggerOnce ? NotificationFlags.TriggerOnce : default) |
            (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);

        [Space] [Tooltip("The amount of time the TMPWriter should wait before continuing to write.")]
        [SerializeField] private float waitTime = 0.5f;
        public float WaitTime => waitTime;

        private void OnValidate()
        {
            if (waitTime < 0) waitTime = 0;
        }
    }
}