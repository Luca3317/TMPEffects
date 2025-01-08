using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

namespace TMPEffects.Timeline.Markers
{
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

        [Space]
        [Tooltip(
            "The delta time value to update the animations with. Set to -1 to use Time.deltaTime, -2 to use Time.fixedDeltaTime")]
        [SerializeField]
        private float deltaTime;
    
        public float DeltaTime => deltaTime;
    }
}