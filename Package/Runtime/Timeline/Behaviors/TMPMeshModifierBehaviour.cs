using System;
using TMPEffects.Components;
using TMPEffects.Modifiers;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TMPEffects.Timeline
{
    public class TMPMeshModifierBehaviour : PlayableBehaviour
    {
        public TimelineAnimationStep Step;
        [NonSerialized] public TimelineClip Clip;
    
        private CharDataModifiers modifiersStorage;
        private TMPAnimator animator;
        private float weight;
        private PlayableDirector director;
    }
}