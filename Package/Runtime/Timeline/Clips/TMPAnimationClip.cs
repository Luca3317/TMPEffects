using System;
using System.ComponentModel;
using TMPEffects.Parameters;
using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TMPEffects.Timeline
{
    [DisplayName("TMPEffects Clip/TMPAnimation Clip")]
    public class TMPAnimationClip : TMPEffectsClip, ITimelineClipAsset
    {
        public ITMPAnimation Animation => animation as ITMPAnimation;
        public UnityEngine.Object animation;
        [NonSerialized] public TimelineClip Clip = null;

        public ClipCaps clipCaps => ClipCaps.Extrapolation;

        // HideInInspector so it doesnt show "Add from TMPOffsetProvider" in context menu of timeline
        [HideInInspector] public TMPBlendCurve entryCurve;
        public float entryDuration;

        [HideInInspector] public TMPBlendCurve exitCurve;
        public float exitDuration;
    
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TMPAnimationBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.Clip = Clip;
            behaviour.animation = Animation;
            behaviour.entryCurve = entryCurve;
            behaviour.exitCurve = exitCurve;
            behaviour.entryDuration = entryDuration;
            behaviour.exitDuration = exitDuration;
            
            return playable;
        }

        private void OnValidate()
        {
            if (animation != null)
            {
                if (!(animation is ITMPAnimation)) animation = null;
            }
        }

#if UNITY_EDITOR
        public int lastMovedEntry;
#endif
    }
}