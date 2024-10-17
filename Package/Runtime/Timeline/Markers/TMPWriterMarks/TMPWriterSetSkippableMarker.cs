using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

[CustomStyle("TMPWriterMarkerStyle")]
[TrackBindingType(typeof(TMPWriterTrack))]
[DisplayName("TMPEffects Marker/TMPWriter/SetSkippable")]
public class TMPWriterSetSkippableMarker : TMPEffectsMarker
{
    public override PropertyName id => new PropertyName();

    public override NotificationFlags flags =>
        (retroactive ? NotificationFlags.Retroactive : default) |
        (triggerOnce ? NotificationFlags.TriggerOnce : default) |
        (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);


    [SerializeField] private bool skippable = false;
    public bool Skippable => skippable;
}
