using System.ComponentModel;
using TMPEffects.Components.Animator;
using UnityEngine;
using UnityEngine.Timeline;

namespace TMPEffects.Timeline.Markers
{
    [CustomStyle("TMPSettingsMarkerStyle")]
    [TrackBindingType(typeof(TMPAnimatorTrack))]
    [DisplayName("TMPEffects Marker/TMPAnimator/SetUpdateFrom")]
    public class TMPSetUpdateFromMarker : TMPEffectsMarker
    {
        public override PropertyName id => new PropertyName();

        public override NotificationFlags flags =>
            (retroactive ? NotificationFlags.Retroactive : default) |
            (triggerOnce ? NotificationFlags.TriggerOnce : default) |
            (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);

        [Space] [Tooltip("Where the TMPAnimator should be updated from.")]
        [SerializeField] private UpdateFrom updateFrom;
    
        public UpdateFrom UpdateFrom => updateFrom;
    }
}