using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

[CustomStyle("TMPSkipWriterMarkerStyle")]
[TrackBindingType(typeof(TMPWriterTrack))]
[DisplayName("TMPEffects Marker/TMPWriter/Skip writer")]
public class TMPSkipWriterMarker : TMPEffectsMarker
{
    public override PropertyName id => new PropertyName();

    public override NotificationFlags flags =>
        (retroactive ? NotificationFlags.Retroactive : default) |
        (triggerOnce ? NotificationFlags.TriggerOnce : default) |
        (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);

    [SerializeField] private bool skipShowAnimation = false;
    
    public bool SkipShowAnimation => skipShowAnimation;
}