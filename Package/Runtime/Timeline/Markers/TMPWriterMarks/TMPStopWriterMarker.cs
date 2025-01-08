using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

namespace TMPEffects.Timeline.Markers
{
    [CustomStyle("TMPStopWriterMarkerStyle")]
    [DisplayName("TMPEffects Marker/TMPWriter/Stop writer")]
    public class TMPStopWriterMarker : TMPEffectsMarker
    {
        public override PropertyName id => new PropertyName();

        public override NotificationFlags flags =>
            (retroactive ? NotificationFlags.Retroactive : default) |
            (triggerOnce ? NotificationFlags.TriggerOnce : default) |
            (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);
    }
}