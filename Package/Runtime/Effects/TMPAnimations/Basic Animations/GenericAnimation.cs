using System;
using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
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

        private CharDataModifiers modifiersStorage, modifiersStorage2;
        private CharDataModifiers accModifier, current;

        public static bool ApplyAnimationStep(AnimationStep step, float timeValue, CharData cData, IAnimatorContext context,
            CharDataModifiers storage, CharDataModifiers storage2, CharDataModifiers result)
        {
            if (step == null) return false;
            if (!step.animate) return false;
            if (step.startTime > timeValue) return false;
            if (step.EndTime < timeValue) return false;

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

            if (step.useWave)
            {
                var offset = AnimationUtility.GetWaveOffset(cData, context, step.waveOffsetType);
                weight *= step.wave.Evaluate(timeValue, offset).Value;
            }
            
            result.Reset();
            
            if (step.useInitialModifiers)
            {
                // Reset storage
                storage.Reset();
                storage2.Reset();

                // Set modifiers
                step.initModifiers.ToCharDataModifiers(cData, context, storage);
                step.modifiers.ToCharDataModifiers(cData, context, storage2);

                // Lerp modifiers and store into current
                CharDataModifiers.LerpUnclamped(cData, storage, storage2, weight, result);
            }
            else
            {
                // Reset storage
                storage.Reset();

                // Set modifier
                step.modifiers.ToCharDataModifiers(cData, context, storage);

                // Lerp modifier and store into current
                CharDataModifiers.LerpUnclamped(cData, storage, weight, result);
            }

            return true;
        }

        public static void ApplyAnimationStepWeighted(AnimationStep step, float weight, CharData cData, IAnimatorContext context,
            CharDataModifiers storage, CharDataModifiers storage2, CharDataModifiers result)
        {
            result.Reset();
            
            if (step.useInitialModifiers)
            {
                // Reset storage
                storage.Reset();
                storage2.Reset();

                // Set modifiers
                step.initModifiers.ToCharDataModifiers(cData, context, storage);
                step.modifiers.ToCharDataModifiers(cData, context, storage2);

                // Lerp modifiers and store into current
                CharDataModifiers.LerpUnclamped(cData, storage, storage2, weight, result);
            }
            else
            {
                // Reset storage
                storage.Reset();

                // Set modifier
                step.modifiers.ToCharDataModifiers(cData, context, storage);

                // Lerp modifier and store into current
                CharDataModifiers.LerpUnclamped(cData, storage, weight, result);
            }
        }

        private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
        {
            // Create modifiers if needed
            modifiersStorage ??= new CharDataModifiers();
            modifiersStorage2 ??= new CharDataModifiers();
            accModifier ??= new CharDataModifiers();
            current ??= new CharDataModifiers();

            // Calculate timeValue
            float timeValue =
                data.repeat ? context.AnimatorContext.PassedTime % data.duration : context.AnimatorContext.PassedTime;

            // Reset accModifier
            accModifier.Reset();

            foreach (var step in animationSteps)
            {
                if (!ApplyAnimationStep(step, timeValue, cData, context.AnimatorContext, modifiersStorage,
                        modifiersStorage2, current)) continue;
                
                accModifier.MeshModifiers.Combine(current.MeshModifiers);
                accModifier.CharacterModifiers.Combine(current.CharacterModifiers);
                //
                // // Continue checks
                // if (step == null) continue;
                // if (!step.animate) continue;
                // if (step.startTime > timeValue) continue;
                // if (step.EndTime < timeValue) continue;
                //
                // // Calculate weight / t value for current step
                // float weight = 1;
                // float entry = timeValue - step.startTime;
                // if (entry <= step.entryDuration)
                // {
                //     weight = step.entryCurve.Evaluate(entry / step.entryDuration);
                // }
                //
                // float exit = step.EndTime - timeValue;
                // if (exit <= step.exitDuration)
                // {
                //     weight *= step.exitCurve.Evaluate(exit / step.exitDuration);
                // }
                //
                // if (step.useWave)
                // {
                //     var offset = AnimationUtility.GetWaveOffset(cData, context, step.waveOffsetType);
                //     weight *= step.wave.Evaluate(timeValue, offset).Value;
                // }
                //
                // // Reset storage
                // modifiersStorage.Reset();
                //
                // if (step.useInitialModifiers)
                // {
                //     // Set modifiers
                //     step.initModifiers.ToCharDataModifiers(cData, context, modifiersStorage);
                //     step.modifiers.ToCharDataModifiers(cData, context, modifiersStorage2);
                //
                //     // Lerp modifiers and store into current
                //     CharDataModifiers.LerpUnclamped(cData, modifiersStorage, modifiersStorage2, weight, current);
                // }
                // else
                // {
                //     // Set modifier
                //     step.modifiers.ToCharDataModifiers(cData, context, modifiersStorage);
                //
                //     // Lerp modifier and store into current
                //     CharDataModifiers.LerpUnclamped(cData, modifiersStorage, weight, current);
                // }
                //
                // accModifier.MeshModifiers.Combine(current.MeshModifiers);
                // accModifier.CharacterModifiers.Combine(current.CharacterModifiers);
            }

            cData.MeshModifiers.Combine(accModifier.MeshModifiers);
            cData.CharacterModifiers.Combine(accModifier.CharacterModifiers);
        }
    }
}