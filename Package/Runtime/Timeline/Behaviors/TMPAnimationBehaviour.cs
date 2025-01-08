using System;
using TMPEffects.Components;
using TMPEffects.Parameters;
using TMPEffects.TMPAnimations;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TMPEffects.Timeline
{
    public class TMPAnimationBehaviour : PlayableBehaviour
    {
        public ITMPAnimation animation;

        [NonSerialized] public TimelineClip Clip = null;

        private TMPAnimator animator;

        public TMPBlendCurve entryCurve;
        public float entryDuration;

        public TMPBlendCurve exitCurve;
        public float exitDuration;
    }
}