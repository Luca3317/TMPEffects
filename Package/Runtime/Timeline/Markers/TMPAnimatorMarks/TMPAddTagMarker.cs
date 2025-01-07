// using System.ComponentModel;
// using TMPEffects.Tags;
// using UnityEngine;
// using UnityEngine.Timeline;
//
// [CustomStyle("TMPResetAnimationsMarkerStyle")]
// [TrackBindingType(typeof(TMPAnimatorTrack))]
// [DisplayName("TMPEffects Marker/TMPAnimator/Add Tag")]
// public class TMPAddTagMarker : TMPEffectsMarker
// {
//     public override PropertyName id => new PropertyName();
//
//     public override NotificationFlags flags =>
//         (retroactive ? NotificationFlags.Retroactive : default) |
//         (triggerOnce ? NotificationFlags.TriggerOnce : default) |
//         (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);
//
//     [SerializeField] private TMPEffectTag tag;
//     [SerializeField] private TMPEffectTagIndices indices;
//     
//     public TMPEffectTag Tag => tag;
//     public TMPEffectTagIndices Indices => indices;
// }     