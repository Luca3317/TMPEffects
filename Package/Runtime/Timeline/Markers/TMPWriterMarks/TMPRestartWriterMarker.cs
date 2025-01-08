using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

namespace TMPEffects.Timeline.Markers
{
    [CustomStyle("TMPRestartWriterMarkerStyle")]
    [DisplayName("TMPEffects Marker/TMPWriter/Restart writer")]
    public class TMPRestartWriterMarker : TMPEffectsMarker
    {
        public override PropertyName id => new PropertyName();

        public override NotificationFlags flags =>
            (retroactive ? NotificationFlags.Retroactive : default) |
            (triggerOnce ? NotificationFlags.TriggerOnce : default) |
            (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);
    }
}