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
    private CharDataModifiers modifiersStorage;
    private TMPAnimator animator;
    private float weight;
    private PlayableDirector director;
    
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        animator ??= info.output.GetUserData() as TMPAnimator;
        
        if (animator == null) return;
        animator.OnCharacterAnimated -= OnCharacterAnimated;
        animator.OnCharacterAnimated += OnCharacterAnimated;
        
        animator.SetUpdateFrom(UpdateFrom.Script);
    }

    public void Initialize(PlayableDirector playableDirector)
    {
        director = playableDirector;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        animator ??= info.output.GetUserData() as TMPAnimator;

        weight = info.effectiveWeight;
        animator.ResetTime((float)director.time);
        animator.UpdateAnimations(0f);
    }

    private void OnCharacterAnimated(CharData cdata)
    {
        if (animator == null) return;
        var mod = Calc(Step, cdata, weight, (float)director.time);
        cdata.MeshModifiers.Combine(mod.MeshModifiers);
        cdata.CharacterModifierss.Combine(mod.CharacterModifiers);
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        animator ??= info.output.GetUserData() as TMPAnimator;
        animator.OnCharacterAnimated -= OnCharacterAnimated;
    }

    private CharDataModifiers Calc(TimelineAnimationStep step, CharData cData, float weight, float t)
    {
        modifiersStorage ??= new CharDataModifiers();
        CharDataModifiers result;

        if (step.useWave)
        {
            result =
                CharDataModifiers.LerpUnclamped(cData, step.charModifiers,
                    step.wave.Evaluate(t,
                        AnimationUtility.GetWaveOffset(cData, step.waveOffsetType, cData.info.index)).Value,
                    modifiersStorage);
        }
        else
        {
            result = new CharDataModifiers(step.charModifiers);
        }

        if (weight != 1)
        {
            result = CharDataModifiers.LerpUnclamped(cData, result,
                weight, modifiersStorage);
        }

        return result;
    }
}