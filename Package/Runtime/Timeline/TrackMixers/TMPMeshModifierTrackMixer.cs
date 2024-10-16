using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components;
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
        animator.UpdateAnimations(0f);
    }

    private void OnAnimatedCallback(CharData cdata)
    {
        for (int i = 0; i < active.Count; i++)
        {
            if (!active[i].IsValid()) continue;
            var behaviour = active[i].GetBehaviour();
            if (behaviour == null) continue;
            float currTime = (float)active[i].GetTime();
            float duration = (float)active[i].GetDuration();
            float weight = 1;

            if (currTime <= behaviour.Step.entryDuration)
            {
                weight = behaviour.Step.entryCurve.Evaluate(currTime / behaviour.Step.entryDuration);
            }
            else if (currTime >= duration - behaviour.Step.exitDuration)
            {
                float preTime = duration - behaviour.Step.exitDuration;
                weight = 1f - behaviour.Step.exitCurve.Evaluate((currTime - preTime) / behaviour.Step.exitDuration);
            }

            var result = Calc(animator, behaviour.Step, cdata, weight, time);
            cdata.CharacterModifierss.Combine(result.CharacterModifiers);
            cdata.MeshModifiers.Combine(result.MeshModifiers);
        }
    }


    private CharDataModifiers storage;

    private CharDataModifiers Calc(TMPAnimator animator, TimelineAnimationStep step, CharData cData, float weight,
        float t)
    {
        // CharDataModifiers result = new CharDataModifiers(); // TODO Cache
        storage ??= new CharDataModifiers();

        if (step.useWave)
        {
            var offset = AnimationUtility.GetWaveOffset(cData, animator.AnimatorContext, step.waveOffsetType,
                cData.info.index);

            CharDataModifiers.LerpUnclamped(cData,
                step.editorModifiers.ToCharDataModifiers(cData, animator.AnimatorContext),
                step.wave.Evaluate(t, offset).Value * weight,
                storage);
        }
        else
        {
            var res = step.editorModifiers.ToCharDataModifiers(cData, animator.AnimatorContext);
            CharDataModifiers.LerpUnclamped(cData, res, weight, storage);
        }
        

        // if (weight != 1)
        // {
        //     CharDataModifiers.LerpUnclamped(cData, result,
        //         weight, result);
        // }

        return storage;
    }
}