using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

[CustomStyle("TMPUpdateAnimationsMarkerStyle")]
[TrackBindingType(typeof(TMPAnimatorTrack))]
[DisplayName("TMPEffects Marker/TMPAnimator/UpdateAnimations")]
public class TMPUpdateAnimationsMarker : TMPEffectsMarker
{
    public override PropertyName id => new PropertyName();

    public override NotificationFlags flags =>
        (retroactive ? NotificationFlags.Retroactive : default) |
        (triggerOnce ? NotificationFlags.TriggerOnce : default) |
        (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);

    [Space] [Tooltip("The delta time value to update the animations with.")]
    [SerializeField] private float deltaTime;
    
    public float DeltaTime => deltaTime;
}    