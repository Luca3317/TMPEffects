using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

namespace TMPEffects.Timeline.Markers
{
    [CustomStyle("TMPResetTimeMarkerStyle")]
    [TrackBindingType(typeof(TMPAnimatorTrack))]
    [DisplayName("TMPEffects Marker/TMPAnimator/ResetTime")]
    public class TMPResetTimeMarker : TMPEffectsMarker
    {
        public override PropertyName id => new PropertyName();

        public override NotificationFlags flags =>
            (retroactive ? NotificationFlags.Retroactive : default) |
            (triggerOnce ? NotificationFlags.TriggerOnce : default) |
            (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);

        [Space] [Tooltip("The time value to reset the TMPAnimator to.")]
        [SerializeField] private new float time;
    
        public float Time => time;
    
        private void OnValidate()
        {
            if (time < 0) time = 0;
        }
    }
}