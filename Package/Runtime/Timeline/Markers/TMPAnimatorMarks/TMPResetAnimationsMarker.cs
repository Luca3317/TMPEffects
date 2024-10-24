using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

[CustomStyle("TMPResetAnimationsMarkerStyle")]
[TrackBindingType(typeof(TMPAnimatorTrack))]
[DisplayName("TMPEffects Marker/TMPAnimator/ResetAnimations")]
public class TMPResetAnimationsMarker : TMPEffectsMarker
{
    public override PropertyName id => new PropertyName();

    public override NotificationFlags flags =>
        (retroactive ? NotificationFlags.Retroactive : default) |
        (triggerOnce ? NotificationFlags.TriggerOnce : default) |
        (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);
}    