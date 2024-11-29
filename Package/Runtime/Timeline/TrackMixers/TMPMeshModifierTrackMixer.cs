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
    private List<ScriptPlayable<TMPMeshModifierBehaviour>> active;
    private TMPAnimator animator;

    private bool needsReset = false;
    private CharDataModifiers modifiersStorage, modifiersStorage2;
    private CharDataModifiers accModifier, current;

    private float time;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        animator = playerData as TMPAnimator;

        if (animator == null) return;

        active ??= new List<ScriptPlayable<TMPMeshModifierBehaviour>>();
        active.Clear();

        animator.OnCharacterAnimated -= OnAnimatedCallback;

        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float weight = playable.GetInputWeight(i);
            if (weight <= 0) continue;

            ScriptPlayable<TMPMeshModifierBehaviour> behaviour =
                (ScriptPlayable<TMPMeshModifierBehaviour>)playable.GetInput(i);

            active.Add(behaviour);
        }

        time = (float)playable.GetTime();

        if (active.Count > 0) animator.OnCharacterAnimated += OnAnimatedCallback;
        else if (needsReset)
        {
            needsReset = false;
            animator.QueueCharacterReset();
        }
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

            float currTime;
            if (time < behaviour.Clip.start)
                currTime = time - (float)behaviour.Clip.start;
            else
                currTime = (float)active[i].GetTime();

            float duration = (float)behaviour.Clip.duration;

            float weight = GenericAnimation.CalcWeight(behaviour.Step.Step, currTime, duration, cdata,
                animator.AnimatorContext);
            GenericAnimation.LerpAnimationStepWeighted(behaviour.Step.Step, weight, cdata, animator.AnimatorContext,
                modifiersStorage, modifiersStorage2, current);

            // var result = Calc(animator, behaviour.Step, cdata, weight, time);
            cdata.CharacterModifiers.Combine(current.CharacterModifiers);
            cdata.MeshModifiers.Combine(current.MeshModifiers);

            needsReset = true;
        }
    }
}