using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

namespace TMPEffects.Timeline.Markers
{
    [CustomStyle("TMPSetSkippableMarkerStyle")]
    [DisplayName("TMPEffects Marker/TMPWriter/SetSkippable")]
    public class TMPWriterSetSkippableMarker : TMPEffectsMarker
    {
        public override PropertyName id => new PropertyName();

        public override NotificationFlags flags =>
            (retroactive ? NotificationFlags.Retroactive : default) |
            (triggerOnce ? NotificationFlags.TriggerOnce : default) |
            (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);

        [Space] [Tooltip("Whether the current text should be skippable.")]
        [SerializeField] private bool skippable = false;
    
        public bool Skippable => skippable;
    }
}