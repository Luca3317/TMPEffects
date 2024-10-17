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

        public bool Repeat
        {
            get => repeat;
            set => repeat = value;
        }

        public float Duration
        {
            get => duration;
            set => duration = value;
        }

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
            modifiersStorage.Reset();

            // TODO Probably should use an IntervalTree or smth for this
            int count = 0;
            foreach (var step in animationSteps)
            {
                if (step == null) continue;
                if (!step.animate) continue;
                if (step.startTime > timeValue) continue;
                if (step.EndTime < timeValue) continue;

                float weight = 1;
                float entry = timeValue - step.startTime;
                if (entry <= step.entryDuration)
                {
                    weight = step.entryCurve.Evaluate(entry / step.entryDuration);
                }

                float exit = step.EndTime - timeValue;
                if (exit <= step.exitDuration)
                {
                    weight *= step.exitCurve.Evaluate(exit / step.exitDuration);
                }
                
                CharDataModifiers toUse = step.modifiers.ToCharDataModifiers(cData, context.AnimatorContext);
                CharDataModifiers result = new CharDataModifiers(); // TODO cache

                if (step.useWave)
                {
                    var offset = AnimationUtility.GetWaveOffset(cData, context, step.waveOffsetType);
                        CharDataModifiers.LerpUnclamped(cData, toUse,
                            step.wave.Evaluate(timeValue, offset).Value * weight,
                            result);
                }
                else
                {
                    CharDataModifiers.LerpUnclamped(cData, toUse, weight, result);
                }


                accModifier.MeshModifiers.Combine(result.MeshModifiers);
                accModifier.CharacterModifiers.Combine(result.CharacterModifiers);
            }

            cData.MeshModifiers.Combine(accModifier.MeshModifiers);
            cData.CharacterModifiers.Combine(accModifier.CharacterModifiers);
        }
    }
}