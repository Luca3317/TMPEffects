using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.Parameters;
using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class AnimationStep
{
    public string name;
    public bool animate = true;

    public AnimationCurve entryCurve;
    public float entryDuration;

    public AnimationCurve exitCurve;
    public float exitDuration;

    public float extrapolateFrom;
    public ExtrapolationMode preExtrapolation;
    public float extrapolateUntil;
    public ExtrapolationMode postExtrapolation;

    [Tooltip("Whether this animation step should automatically loop over time. " +
             "For GenericAnimations, this can be disregarded if the animation itself is set to repeat.")]
    public bool loops;

    [Tooltip("How many times this animation step should automatically loop over time. " +
             "For GenericAnimations, this can be disregarded if the animation itself is set to repeat.")]
    public uint repetitions;

    public bool useWave;
    public ParameterTypes.WaveOffsetType waveOffsetType;
    public AnimationUtility.Wave wave;

    public float startTime;
    public float duration;

    // Get end time of this step; if loops forever (repetitions == 0), float.MaxValue
    public float EndTime =>
        !loops
            ? startTime + duration
            : (repetitions != 0 ? startTime + duration * repetitions : float.MaxValue);

    public bool useInitialModifiers = false;
    public EditorFriendlyCharDataModifiers initModifiers;
    public EditorFriendlyCharDataModifiers modifiers;

    public enum ExtrapolationMode
    {
        None,
        Hold,
        Loop,
        PingPong,
        Continue
    }
}