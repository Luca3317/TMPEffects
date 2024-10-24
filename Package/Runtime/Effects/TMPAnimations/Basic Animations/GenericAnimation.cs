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
    public sealed partial class GenericAnimation : TMPAnimation
    {
        [Serializable]
        public class Track
        {
            public List<AnimationStep> Clips => clips;
            [SerializeReference] private List<AnimationStep> clips = new List<AnimationStep>();
        }

        [Serializable]
        public class TrackList
        {
            public List<Track> Tracks = new List<Track>();

            public Track this[int index]
            {
                get => Tracks[index];
                set => Tracks[index] = value;
            }
        }

        public TrackList Tracks = new TrackList();

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

        [AutoParameter("repeat", "rp"), SerializeField]
        private bool repeat;

        [AutoParameter("duration", "dur"), SerializeField]
        private float duration;

        private CharDataModifiers modifiersStorage, modifiersStorage2;
        private CharDataModifiers accModifier, current;

        public static bool ApplyAnimationStep(AnimationStep step, float timeValue, CharData cData,
            IAnimatorContext context,
            CharDataModifiers storage, CharDataModifiers storage2, CharDataModifiers result)
        {
            if (step == null) return false;
            if (!step.animate) return false;

            if (timeValue < step.startTime)
            {
                switch (step.preExtrapolation)
                {
                    case AnimationStep.ExtrapolationMode.Continue:
                    case AnimationStep.ExtrapolationMode.Hold:
                        timeValue = step.startTime;
                        break;
                    case AnimationStep.ExtrapolationMode.Loop:
                        float diff = (step.startTime - timeValue) % step.duration;
                        timeValue = step.EndTime - diff;
                        break;
                    case AnimationStep.ExtrapolationMode.PingPong:
                        diff = (step.startTime - timeValue) % step.duration;
                        timeValue = step.startTime + diff;
                        break;
                }
            }

            if (timeValue > step.EndTime)
            {
                switch (step.postExtrapolation)
                {
                    case AnimationStep.ExtrapolationMode.Continue:
                    case AnimationStep.ExtrapolationMode.Hold:
                        timeValue = step.EndTime;
                        break;
                    case AnimationStep.ExtrapolationMode.Loop:
                        float diff = (timeValue - step.EndTime) % step.duration;
                        timeValue = step.startTime + diff;
                        break;
                    case AnimationStep.ExtrapolationMode.PingPong:
                        diff = (timeValue - step.EndTime) % step.duration;
                        timeValue = step.EndTime - diff;
                        break;
                }
            }
            
            // If time before clip starts (after adjusting for extrapolation)
            if (step.startTime > timeValue) return false;

            // If time after clip ends (after adjusting for extrapolation)
            if (step.EndTime < timeValue) return false;

            float weight = 1;
            float entry = timeValue - step.startTime;
            if (step.entryDuration > 0 && entry <= step.entryDuration)
            {
                weight = step.entryCurve.Evaluate(entry / step.entryDuration);
            }

            float exit = step.EndTime - timeValue;
            if (step.exitDuration > 0 && exit <= step.exitDuration)
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

        public static void ApplyAnimationStepWeighted(AnimationStep step, float weight, CharData cData,
            IAnimatorContext context,
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
                Debug.LogWarning("inbetween is now br color " + result.MeshModifiers.BR_Color);
                CharDataModifiers.LerpUnclamped(cData, storage, storage2, weight, result);
                Debug.LogWarning("inbetween 2 is now br color " + result.MeshModifiers.BR_Color);
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
            Animate(cData, context.AnimatorContext, data.repeat, data.duration);
        }

        public void Animate(CharData cData, IAnimatorContext context, bool repeat, float duration)
        {
            // Create modifiers if needed
            modifiersStorage ??= new CharDataModifiers();
            modifiersStorage2 ??= new CharDataModifiers();
            accModifier ??= new CharDataModifiers();
            current ??= new CharDataModifiers();

            // Calculate timeValue
            float timeValue =
                repeat ? context.PassedTime % duration : context.PassedTime;

            // Reset accModifier
            accModifier.Reset();

            foreach (var track in Tracks.Tracks)
            {
                foreach (var step in track.Clips)
                {
                    if (!ApplyAnimationStep(step, timeValue, cData, context, modifiersStorage,
                            modifiersStorage2, current)) continue;

                    accModifier.MeshModifiers.Combine(current.MeshModifiers);
                    accModifier.CharacterModifiers.Combine(current.CharacterModifiers);
                }
            }

            cData.MeshModifiers.Combine(accModifier.MeshModifiers);
            cData.CharacterModifiers.Combine(accModifier.CharacterModifiers);
        }
    }
}