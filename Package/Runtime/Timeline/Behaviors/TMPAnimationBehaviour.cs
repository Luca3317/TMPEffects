using System;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TMPAnimationBehaviour : PlayableBehaviour
{
    public GenericAnimation animation;
    [NonSerialized] public TimelineClip Clip = null;
}