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
        // Serailize Reference to be able to cast managedReferenceValue back to AnimationStep in Generator
        [SerializeReference] private List<AnimationStep> animationSteps = new List<AnimationStep>()
        {
            new AnimationStep()
        };

        [AutoParameter("repeat", "rp"), SerializeField]
        private bool repeat;

        [AutoParameter("duration", "dur"), SerializeField]
        private float duration;


        private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
        {
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
                                AnimationUtility.GetWaveOffset(cData, context, step.waveOffsetType)).Value);
                }
                else
                {
                    result = new CharDataModifiers(step.charModifiers); 
                }

                float entry = timeValue - step.startTime;
                if (entry <= step.entryDuration)
                {
                    result = CharDataModifiers.LerpUnclamped(cData, result,
                        step.entryCurve.Evaluate(entry / step.entryDuration));
                }

                float exit = step.EndTime - timeValue;
                if (exit <= step.exitDuration)
                {
                    result = CharDataModifiers.LerpUnclamped(cData, result,
                        step.exitCurve.Evaluate(exit / step.exitDuration));
                }

                accModifier.CharacterModifiers.Combine(result.CharacterModifiers);
                accModifier.MeshModifiers.Combine(result.MeshModifiers);
            }
            
            // Have to reassign cause struct
            var meshMods = cData.MeshModifiers;
            meshMods.Combine(accModifier.MeshModifiers);
            cData.MeshModifiers = meshMods;
            cData.CharacterModifiers.Combine(accModifier.CharacterModifiers);
        }
        
        [Serializable]
        public class AnimationStep
        {
            public string name;
            public bool animate = true;

            public AnimationCurve entryCurve;
            public float entryDuration;

            public AnimationCurve exitCurve;
            public float exitDuration;

            public bool loops;
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

            public CharDataModifiers charModifiers;
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