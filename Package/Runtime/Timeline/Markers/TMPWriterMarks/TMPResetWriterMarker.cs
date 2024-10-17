using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

[CustomStyle("TMPResetWriterMarkerStyle")]
[TrackBindingType(typeof(TMPWriterTrack))]
[DisplayName("TMPEffects Marker/TMPWriter/Reset writer")]
public class TMPResetWriterMarker : TMPEffectsMarker
{
    public override PropertyName id => new PropertyName();

    public override NotificationFlags flags =>
        (retroactive ? NotificationFlags.Retroactive : default) |
        (triggerOnce ? NotificationFlags.TriggerOnce : default) |
        (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);

    [SerializeField] private int textIndex;
    
    public int TextIndex => textIndex;
}