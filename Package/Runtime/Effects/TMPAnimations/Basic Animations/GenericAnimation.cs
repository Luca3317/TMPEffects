using System;
using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Parameters;
using UnityEngine;

namespace TMPEffects.TMPAnimations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new GenericAnimation",
        menuName = "TMPEffects/Animations/Basic Animations/Generic Animation")]
    public partial class GenericAnimation : TMPAnimation
    {
        public List<AnimationStep> AnimationSteps => animationSteps;
        public bool Repeat => repeat;
        public float Duration => duration;
        
        [SerializeReference] private List<AnimationStep> animationSteps = new List<AnimationStep>()
        {
            new AnimationStep()
        };
        
        [AutoParameter("repeat", "rp"), SerializeField]
        private bool repeat;

        [AutoParameter("duration", "dur"), SerializeField]
        private float duration;

        private CharDataModifiers modifiersStorage;

        private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
        {
            modifiersStorage ??= new CharDataModifiers();

            float timeValue =
                data.repeat ? context.AnimatorContext.PassedTime % data.duration : context.AnimatorContext.PassedTime;

            CharDataModifiers accModifier = new CharDataModifiers();
            accModifier.Reset();

            // TODO Probably should use an IntervalTree or smth for this
            int count = 0;
            foreach (var step in animationSteps)
            {
                if (step == null) continue;
                if (!step.animate) continue;
                if (step.startTime > timeValue) continue;
                if (step.EndTime < timeValue) continue;

                CharDataModifiers result;

                if (step.useWave)
                {
                    result =
                        CharDataModifiers.LerpUnclamped(cData, step.charModifiers,
                            step.wave.Evaluate(timeValue,
                                AnimationUtility.GetWaveOffset(cData, context, step.waveOffsetType)).Value,
                            modifiersStorage);
                }
                else
                {
                    result = new CharDataModifiers(step.charModifiers);
                }

                float entry = timeValue - step.startTime;
                if (entry <= step.entryDuration)
                {
                    result = CharDataModifiers.LerpUnclamped(cData, result,
                        step.entryCurve.Evaluate(entry / step.entryDuration), modifiersStorage);
                }

                float exit = step.EndTime - timeValue;
                if (exit <= step.exitDuration)
                {
                    result = CharDataModifiers.LerpUnclamped(cData, result,
                        step.exitCurve.Evaluate(exit / step.exitDuration), modifiersStorage);
                }

                accModifier.MeshModifiers.Combine(result.MeshModifiers);
                accModifier.CharacterModifiers.Combine(result.CharacterModifiers);
            }

            cData.MeshModifiers.Combine(accModifier.MeshModifiers);
            cData.CharacterModifiers.Combine(accModifier.CharacterModifiers);
        }

        public struct AnimationStepComparer : IComparer<AnimationStep>
        {
            private List<AnimationStep> unsorted;

            public AnimationStepComparer(List<AnimationStep> list)
            {
                unsorted = list;
            }

            public int Compare(AnimationStep x, AnimationStep y)
            {
                var startTimeComparison = x.startTime.CompareTo(y.startTime);
                if (startTimeComparison != 0) return startTimeComparison;
                return unsorted.IndexOf(x).CompareTo(unsorted.IndexOf(y));
            }
        }
    }
}