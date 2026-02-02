using System.ComponentModel;
using TMPEffects.Components;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TMPEffects.Timeline
{
    [TrackBindingType(typeof(TMPAnimator))]
    [TrackClipType(typeof(TMPAnimationClip))]
    [DisplayName("TMPEffects/TMPAnimatorTrack")]
    public class TMPAnimatorTrack : TrackAsset
    {
        private const string TooltipPauseOption = @"If true, all animations will pause when Timeline is paused.
If false, timeline animations will be canceled and inline styles will play as normal.
If the same TMP Animator is used in multiple tracks, please ensure they have the same option.";
        
        [SerializeField, Tooltip(TooltipPauseOption)]
        private bool preserveAnimationStateOnPause;

        private const string TooltipUpdateOption = @"If true, this track will update the TMP Animator state.
If the same TMP Animator is used in multiple tracks, only one track should be set to true.";

        [SerializeField, Tooltip(TooltipUpdateOption)]
        private bool shouldUpdateAnimations = true;
        
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var clips = GetClips();
            foreach (var clip in clips)
            {
                var currClip = clip.asset as TMPAnimationClip;
                currClip.Clip = clip;
            }
        
            ScriptPlayable<TMPAnimatorTrackMixer> playable = ScriptPlayable<TMPAnimatorTrackMixer>.Create(graph, inputCount);
            TMPAnimatorTrackMixer mixer = playable.GetBehaviour();
            mixer.PreserveAnimationStateOnPause = preserveAnimationStateOnPause;
            mixer.ShouldUpdateAnimations = shouldUpdateAnimations;
            
            return playable;
        }
    }
}