using System.Collections.Generic;
using System.Linq;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using TMPEffects.Components.Animator;
using TMPEffects.Modifiers;
using TMPEffects.Parameters;
using TMPEffects.Tags;
using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Playables;

namespace TMPEffects.Timeline
{
    public class TMPAnimatorTrackMixer : PlayableBehaviour
    {
        private List<ScriptPlayable<TMPAnimationBehaviour>> active;
        private TMPAnimator animator;

        private bool needsReset = false;
        private CharDataModifiers modifiersStorage, modifiersStorage2;
        private CharDataModifiers accModifier, current;
        private TMPCharacterModifiers result;
        private TMPMeshModifiers resultMesh;

        private float time;
        private MockedAnimationContext mocked;
        private ITMPAnimation lastActive;

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (animator == null) return;
            animator.OnTextChanged -= UpdateContext;
            mocked = null;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            animator = playerData as TMPAnimator;

            if (animator == null) return;

            active ??= new List<ScriptPlayable<TMPAnimationBehaviour>>();
            active.Clear();

            animator.OnTextChanged -= UpdateContext;
            animator.OnTextChanged += UpdateContext;
            animator.UnregisterPostAnimationHook(OnAnimatedCallback);

            int inputCount = playable.GetInputCount();
            for (int i = 0; i < inputCount; i++)
            {
                float weight = playable.GetInputWeight(i);
                if (weight <= 0) continue;

                ScriptPlayable<TMPAnimationBehaviour> behaviour =
                    (ScriptPlayable<TMPAnimationBehaviour>)playable.GetInput(i);

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
                if (behaviour.animation == null) continue;

                float currTime;
                if (time < behaviour.Clip.start)
                    currTime = time - (float)behaviour.Clip.start;
                else
                    currTime = (float)active[i].GetTime();

                float duration = (float)behaviour.Clip.duration;

                // var animation = (behaviour.Clip.asset as TMPAnimationClip).Animation;
                // if (animation == null) return;
                if (mocked == null || lastActive != behaviour.animation)
                {
                    var data = behaviour.animation.GetNewCustomData();
                    mocked = new MockedAnimationContext(animator.AnimatorContext, data);
                    behaviour.animation.SetParameters(data, new Dictionary<string, string>(), null);
                    lastActive = behaviour.animation;
                }

                // Animate
                behaviour.animation.Animate(cdata, mocked);

                // Calculate weight
                float weight = CalcWeight(behaviour.entryDuration, behaviour.exitDuration, behaviour.entryCurve,
                    behaviour.exitCurve, currTime, duration, cdata,
                    animator.AnimatorContext, mocked.SegmentData);

                result ??= new TMPCharacterModifiers();
                resultMesh ??= new TMPMeshModifiers();
                result.ClearModifiers();
                resultMesh.ClearModifiers();

                // Lerp using weight
                CharDataModifiers.LerpCharacterModifiersUnclamped(cdata, cdata.CharacterModifiers, weight, result);
                CharDataModifiers.LerpMeshModifiersUnclamped(cdata, cdata.MeshModifiers, weight, resultMesh);

                // Copy over lerped values
                cdata.CharacterModifiers.CopyFrom(result);
                cdata.MeshModifiers.CopyFrom(resultMesh);

                needsReset = true;
            }
        }

        private void UpdateContext(bool _ = false)
        {
            mocked = null;
        }

        public static float CalcWeight(float entryDuration, float exitDuration, TMPBlendCurve inCurve,
            TMPBlendCurve outCurve, float timeValue, float duration,
            CharData cData, IAnimatorDataProvider context, ITMPSegmentData segmentData)
        {
            float weight = 1;

            if (entryDuration > 0)
            {
                weight = inCurve.EvaluateIn(timeValue, entryDuration, cData, context, segmentData);
            }

            if (exitDuration > 0)
            {
                float preTime = duration - exitDuration;
                var multiplier =
                    outCurve.EvaluateOut(timeValue, exitDuration, preTime, cData, context, segmentData);
                weight *= multiplier;
            }

            return weight;
        }

        private class MockedAnimationContext : IAnimationContext
        {
            public bool Finished(int index)
            {
                return finishedDict[index];
            }

            public bool Finished(CharData cData)
            {
                return finishedDict[cData.info.index];
            }

            public void FinishAnimation(CharData cData)
            {
            }

            public IAnimatorContext AnimatorContext { get; set; }

            public SegmentData SegmentData { get; set; }

            public object CustomData { get; set; }

            private Dictionary<int, bool> finishedDict = new Dictionary<int, bool>();

            public MockedAnimationContext(IAnimatorContext context, object customData)
            {
                AnimatorContext = context;
                CustomData = customData;
                UpdateSegmentData();
            }

            private void UpdateSegmentData()
            {
                TMPEffectTagIndices inds =
                    new TMPEffectTagIndices(0, AnimatorContext.Animator.TextComponent.GetParsedText().Length, 0);
                SegmentData = new SegmentData(inds, AnimatorContext.Animator.CharData, (cd) => true);

                for (int i = 0; i < AnimatorContext.Animator.CharData.Count(); i++)
                {
                    finishedDict[i] = false;
                }
            }
        }
    }
}