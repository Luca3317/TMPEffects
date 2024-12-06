using System;
using System.Collections.Generic;
using System.Linq;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using TMPEffects.Components.Animator;
using TMPEffects.Parameters;
using TMPEffects.Tags;
using TMPEffects.TMPAnimations;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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