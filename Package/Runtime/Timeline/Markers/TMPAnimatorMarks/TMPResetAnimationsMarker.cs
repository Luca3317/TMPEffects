using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

namespace TMPEffects.Timeline.Markers
{
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
}