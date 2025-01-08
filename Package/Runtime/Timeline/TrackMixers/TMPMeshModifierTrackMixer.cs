using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using TMPEffects.Components.Animator;
using TMPEffects.Modifiers;
using TMPEffects.TMPAnimations;
using UnityEngine.Playables;

namespace TMPEffects.Timeline
{
    public class TMPMeshModifierTrackMixer : PlayableBehaviour
    {
        private List<ScriptPlayable<TMPMeshModifierBehaviour>> active;
        private TMPAnimator animator;

        private bool needsReset = false;
        private CharDataModifiers modifiersStorage, modifiersStorage2;
        private CharDataModifiers accModifier, current;

        private float time;
    
        private Dictionary<AnimationStep, (GenericAnimationUtility.CachedOffset inOffset, GenericAnimationUtility.CachedOffset outOffset)>
            cachedOffsets = new Dictionary<AnimationStep, (GenericAnimationUtility.CachedOffset, GenericAnimationUtility.CachedOffset)>();

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (animator == null) return;
            animator.OnTextChanged -= UpdateSegmentData;
            mocked = null;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            animator = playerData as TMPAnimator;

            if (animator == null) return;

            active ??= new List<ScriptPlayable<TMPMeshModifierBehaviour>>();
            active.Clear();

            animator.OnTextChanged -= UpdateSegmentData;
            animator.OnTextChanged += UpdateSegmentData;
            animator.UnregisterPostAnimationHook(OnAnimatedCallback);

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

            if (active.Count > 0)
            {
                animator.UnregisterPostAnimationHook(OnAnimatedCallback);
                animator.RegisterPostAnimationHook(OnAnimatedCallback);
            }
            else if (needsReset)
            {
                needsReset = false;
                animator.QueueCharacterReset();
            }
        }

        private ITMPSegmentData mocked;

        private void OnAnimatedCallback(CharData cData)
        {
            modifiersStorage ??= new CharDataModifiers();
            modifiersStorage2 ??= new CharDataModifiers();
            accModifier ??= new CharDataModifiers();
            current ??= new CharDataModifiers();

            if (mocked == null) UpdateSegmentData();

            for (int i = 0; i < active.Count; i++)
            {
                if (!active[i].IsValid()) continue;
                var behaviour = active[i].GetBehaviour();
                if (behaviour == null) continue;

                float currTime;
                // if (time < behaviour.Clip.start)
                //     currTime = time - (float)behaviour.Clip.start;
                // else
                currTime = (float)active[i].GetTime();

                float duration = (float)behaviour.Clip.duration;

                var step = behaviour.Step.Step;
                if (!cachedOffsets.TryGetValue(step, out var cachedOffset))
                {
                    step.entryCurve.provider.GetMinMaxOffset(out float inMin, out float inMax, mocked,
                        animator.AnimatorContext);
                    step.exitCurve.provider.GetMinMaxOffset(out float outMin, out float outMax, mocked,
                        animator.AnimatorContext);
                    cachedOffset = (
                        new GenericAnimationUtility.CachedOffset()
                            { minOffset = inMin, maxOffset = inMax, offset = new Dictionary<CharData, float>() },
                        new GenericAnimationUtility.CachedOffset()
                            { minOffset = outMin, maxOffset = outMax, offset = new Dictionary<CharData, float>() });

                    cachedOffsets[step] = cachedOffset;
                }

                if (!cachedOffset.inOffset.offset.TryGetValue(cData, out float inOffset))
                {
                    inOffset = step.entryCurve.provider.GetOffset(cData, mocked, animator.AnimatorContext);
                    cachedOffset.inOffset.offset[cData] = inOffset;
                }

                if (!cachedOffset.outOffset.offset.TryGetValue(cData, out float outOffset))
                {
                    outOffset = step.entryCurve.provider.GetOffset(cData, mocked, animator.AnimatorContext);
                    cachedOffset.outOffset.offset[cData] = outOffset;
                }
            
                float weight = AnimationStep.CalcWeight(behaviour.Step.Step, currTime, duration, cData,
                    animator.AnimatorContext, mocked);
            
                AnimationStep.LerpAnimationStepWeighted(behaviour.Step.Step, weight, cData, animator.AnimatorContext,
                    modifiersStorage, modifiersStorage2, current);

                cData.CharacterModifiers.Combine(current.CharacterModifiers);
                cData.MeshModifiers.Combine(current.MeshModifiers);

                needsReset = true;
            }
        }

        private void UpdateSegmentData(bool _ = false)
        {
            mocked = TMPAnimationUtility.GetMockedSegment(animator.TextComponent.GetParsedText().Length, animator.CharData);
            cachedOffsets =
                new Dictionary<AnimationStep, (GenericAnimationUtility.CachedOffset inOffset, GenericAnimationUtility.CachedOffset outOffset
                    )>();
        }
    }
}