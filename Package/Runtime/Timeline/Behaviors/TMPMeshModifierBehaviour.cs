using System;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TMPMeshModifierBehaviour : PlayableBehaviour
{
    public TimelineAnimationStep Step;
    [NonSerialized] public TimelineClip Clip;
    
    private CharDataModifiers modifiersStorage;
    private TMPAnimator animator;
    private float weight;
    private PlayableDirector director;
}