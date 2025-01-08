using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

namespace TMPEffects.Timeline.Markers
{
    [CustomStyle("TMPStartWriterMarkerStyle")]
    [DisplayName("TMPEffects Marker/TMPWriter/Start writer")]
    public class TMPStartWriterMarker : TMPEffectsMarker
    {
        public override PropertyName id => new PropertyName();
        public override NotificationFlags flags =>
            (retroactive ? NotificationFlags.Retroactive : default) |
            (triggerOnce ? NotificationFlags.TriggerOnce : default) |
            (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);
    }
}