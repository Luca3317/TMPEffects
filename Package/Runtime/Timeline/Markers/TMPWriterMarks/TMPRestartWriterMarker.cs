using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

[CustomStyle("TMPRestartWriterMarkerStyle")]
[TrackBindingType(typeof(TMPWriterTrack))]
[DisplayName("TMPEffects Marker/TMPWriter/Restart writer")]
public class TMPRestartWriterMarker : TMPEffectsMarker
{
    public override PropertyName id => new PropertyName();

    public override NotificationFlags flags =>
        (retroactive ? NotificationFlags.Retroactive : default) |
        (triggerOnce ? NotificationFlags.TriggerOnce : default) |
        (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);
}