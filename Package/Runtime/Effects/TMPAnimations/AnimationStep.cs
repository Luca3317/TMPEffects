using System;
using TMPEffects.Parameters;
using TMPEffects.TMPAnimations;
using UnityEngine;

[Serializable]
public class AnimationStep
{
    public string name;
    public bool animate = true;

    // HideInInspector so it doesnt show "Add from TMPOffsetProvider" in context menu of timeline
    [HideInInspector] public TMPBlendCurve entryCurve;
    public float entryDuration;

    [HideInInspector] public TMPBlendCurve exitCurve;
    public float exitDuration;

    public ExtrapolationMode preExtrapolation;
    public ExtrapolationMode postExtrapolation;

    // TODO These dont really make much sense anymore right now since itll just overwrite the following steps / clips
    // Then: Need to figure out how to do oneshot vs looping animations. Maybe loop per track. or smth idk.
    [Tooltip("Whether this animation step should automatically loop over time. " +
             "For GenericAnimations, this can be disregarded if the animation itself is set to repeat.")]
    public bool loops;
    [Tooltip("How many times this animation step should automatically loop over time. " +
             "For GenericAnimations, this can be disregarded if the animation itself is set to repeat.")]
    public uint repetitions;

    public bool useWave;
    public Wave wave; 
    public OffsetBundle waveOffset;

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

    public AnimationStep()
    {
    }

    public AnimationStep(AnimationStep original)
    {
        name = original.name;
        animate = original.animate;
        
        entryCurve = new TMPBlendCurve(original.entryCurve);
        entryDuration = original.entryDuration;
        exitCurve = new TMPBlendCurve(original.exitCurve);
        exitDuration = original.exitDuration;
        
        preExtrapolation = original.preExtrapolation;
        postExtrapolation = original.postExtrapolation;
        
        loops = original.loops;
        repetitions = original.repetitions;
        
        useWave = original.useWave;
        waveOffset = original.waveOffset;
        wave = new Wave(original.wave); // TODO Wave is class so need to copy here
        
        startTime = original.startTime;
        duration = original.duration;
        
        useInitialModifiers = original.useInitialModifiers;
        initModifiers = new EditorFriendlyCharDataModifiers(original.initModifiers);
        modifiers = new EditorFriendlyCharDataModifiers(original.modifiers);
    }

    public enum ExtrapolationMode
    {
        None,
        Hold,
        Loop,
        PingPong,
        Continue
    }
}