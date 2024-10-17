using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Playables;

public class TMPMeshModifierTrackMixer : PlayableBehaviour
{
    private ExposedReference<PlayableDirector> director;
    private List<ScriptPlayable<TMPMeshModifierBehaviour>> active;
    private TMPAnimator animator;
    private float time;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        animator = playerData as TMPAnimator;

        if (animator == null) return;

        PlayableDirector director = (PlayableDirector)playable.GetGraph().GetResolver();

        active ??= new List<ScriptPlayable<TMPMeshModifierBehaviour>>();
        active.Clear();

        animator.OnCharacterAnimated -= OnAnimatedCallback;

        time = (float)director.time;


        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float weight = playable.GetInputWeight(i);
            if (weight <= 0) continue;

            ScriptPlayable<TMPMeshModifierBehaviour> behaviour =
                (ScriptPlayable<TMPMeshModifierBehaviour>)playable.GetInput(i);

            active.Add(behaviour);
        }

        if (active.Count > 0) animator.OnCharacterAnimated += OnAnimatedCallback;

        if (animator.UpdateFrom == UpdateFrom.Script)
            animator.UpdateAnimations(0f);
    }

    private void OnAnimatedCallback(CharData cdata)
    {
        modifiersStorage ??= new CharDataModifiers();
        modifiersStorage2 ??= new CharDataModifiers();
        accModifier ??= new CharDataModifiers();
        current ??= new CharDataModifiers();

        for (int i = 0; i < active.Count; i++)
        {
            if (!active[i].IsValid()) continue;
            var behaviour = active[i].GetBehaviour();
            if (behaviour == null) continue;
            float currTime = (float)active[i].GetTime();
            float duration = (float)active[i].GetDuration();
            float weight = 1;


            if (currTime <= behaviour.Step.Step.entryDuration)
            {
                weight = behaviour.Step.Step.entryCurve.Evaluate(currTime / behaviour.Step.Step.entryDuration);
            }
            else if (currTime >= duration - behaviour.Step.Step.exitDuration)
            {
                float preTime = duration - behaviour.Step.Step.exitDuration;
                weight = behaviour.Step.Step.exitCurve.Evaluate(1f - (currTime - preTime) /
                    behaviour.Step.Step.exitDuration);
            }

            if (behaviour.Step.Step.useWave)
            {
                var offset = AnimationUtility.GetWaveOffset(cdata, animator.AnimatorContext,
                    behaviour.Step.Step.waveOffsetType);
                weight *= behaviour.Step.Step.wave.Evaluate(currTime, offset).Value;
            }


            GenericAnimation.ApplyAnimationStepWeighted(behaviour.Step.Step, weight, cdata, animator.AnimatorContext,
                modifiersStorage, modifiersStorage2, current);

            // var result = Calc(animator, behaviour.Step, cdata, weight, time);
            cdata.CharacterModifierss.Combine(current.CharacterModifiers);
            cdata.MeshModifiers.Combine(current.MeshModifiers);
        }
    }


    // TODO
    // Calc and genericanimation should use some shared implementation
    // Probably generated animations as well?
    private CharDataModifiers modifiersStorage, modifiersStorage2;
    private CharDataModifiers accModifier, current;
}