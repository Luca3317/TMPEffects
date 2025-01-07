using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

[CustomStyle("TMPStopAnimatingMarkerStyle")]
[TrackBindingType(typeof(TMPAnimatorTrack))]
[DisplayName("TMPEffects Marker/TMPAnimator/StopAnimating")]
public class TMPStopAnimatingMarker : TMPEffectsMarker
{
    public override PropertyName id => new PropertyName();

    public override NotificationFlags flags =>
        (retroactive ? NotificationFlags.Retroactive : default) |
        (triggerOnce ? NotificationFlags.TriggerOnce : default) |
        (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);
}    