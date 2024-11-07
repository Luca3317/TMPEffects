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
        // TODO Theres a new weird issue "A previewrenderutility was not cleaned up"
        // TODO Potentially might have smth to do with this. Idk why but it started happening around the time i implemented this.
        protected override void OnValidate()
        {
            base.OnValidate();
            EnsureNonOverlappingTimings();
        }

        //TODO delete
        public void SetParameters_Hook(object customData,
            System.Collections.Generic.IDictionary<string, string> parameters,
            TMPEffects.TMPAnimations.IAnimationContext context)
        {
        }

        //TODO delete
        public bool ValidateParameters_Hook(System.Collections.Generic.IDictionary<string, string> parameters,
            IAnimatorContext context)
        {
            return true;
        }

        private void EnsureNonOverlappingTimings()
        {
            for (int i = 0; i < Tracks.Tracks.Count; i++)
            {
                var track = Tracks.Tracks[i];

                // Ensure no negative values
                for (int j = 0; j < track.Clips.Count; j++)
                {
                    var step = track.Clips[j];
                    if (step == null) continue;

                    if (step.duration < 0)
                        step.duration = 0;
                    if (step.startTime < 0)
                        step.startTime = 0;
                }

                var copy = new List<AnimationStep>(track.Clips);
                copy.Sort(new StepComparer());
                float lastStartTime = -1, lastDuration = 0;
                for (int j = 0; j < copy.Count; j++)
                {
                    var step = copy[j];
                    if (step == null || step.duration == 0) continue;

                    // If starts while previous step still running
                    if (step.startTime < lastStartTime + lastDuration)
                    {
                        float prev = step.startTime;
                        step.startTime = lastStartTime + lastDuration;
                        step.duration = Mathf.Max(0f, step.duration - (step.startTime - prev));
                    }

                    lastStartTime = step.startTime;
                    lastDuration = step.duration;
                }

                // TODO Uncommenting this will auto force sort clips
                // (which makes the reorderable part of them sort of pointless unless same start and 0 duration)
                // TODO Maybe make an editor only bool for whether to do this
                // Tracks.Tracks[i].Clips = copy;
            }
        }


        [Serializable]
        public class Track
        {
            public List<AnimationStep> Clips
            {
                get => clips;
                set => clips = value;
            }

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


        private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
        {
            if (data.Steps == null)
            {
                data.Steps = new List<List<AnimationStep>>();
                for (int i = 0; i < Tracks.Tracks.Count; i++)
                {
                    var track = Tracks.Tracks[i];
                    var copy = new List<AnimationStep>(track.Clips);
                    copy.Sort(new StepComparer());
                    data.Steps.Add(copy);
                }
            }

            Animate(cData, data.Steps, context.AnimatorContext, data.repeat, data.duration);
        }

        public static float AdjustTimeForExtrapolation(AnimationStep step, float timeValue, CharData cData,
            IAnimatorContext context)
        {
            if (timeValue < step.startTime)
            {
                switch (step.preExtrapolation)
                {
                    case AnimationStep.ExtrapolationMode.Continue:
                        break; // Continue only different in how the value should be handled
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
                        break; // Continue only different in how the value should be handled
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

            return timeValue;
        }

        public static float CalcWeight(AnimationStep step, float timeValue, float duration,
            CharData cData, IAnimatorContext context)
        {
            float weight = 1;
            if (step.entryDuration > 0 /*&& currTime <= behaviour.Step.Step.entryDuration*/)
            {
                weight = step.entryCurve.Evaluate(cData, context,
                    timeValue / step.entryDuration);
            }

            if (step.exitDuration > 0 /*&& currTime >= duration - behaviour.Step.Step.exitDuration*/)
            {
                float preTime = duration - step.exitDuration;
                var multiplier = (step.exitCurve.Evaluate(cData, context,
                    1f - (timeValue - preTime) /
                    step.exitDuration));
                
                weight *= multiplier;
            }

            if (step.useWave)
            {
                var offset = AnimationUtility.GetOffset(cData, context,
                    step.offsetType);
                weight *= step.wave.Evaluate(timeValue, offset).Value;
            }

            return weight;
        }

        public static void LerpAnimationStepWeighted(AnimationStep step, float weight, CharData cData,
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
                CharDataModifiers.LerpUnclamped(cData, context, storage, storage2, weight, result);
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

        private void Animate(CharData cData, List<List<AnimationStep>> steps, IAnimatorContext context, bool repeat,
            float duration)
        {
            // Create modifiers if needed
            modifiersStorage ??= new CharDataModifiers();
            modifiersStorage2 ??= new CharDataModifiers();
            accModifier ??= new CharDataModifiers();
            current ??= new CharDataModifiers();

            // Init 

            // Calculate timeValue
            float timeValue =
                repeat ? context.PassedTime % duration : context.PassedTime;

            // Reset accModifier
            accModifier.Reset();

            foreach (var track in steps)
            {
                var active = FindCurrentlyActive(timeValue, track);
                if (active == -1) continue;
                var step = track[active];
                if (!step.animate) continue;

                float t = AdjustTimeForExtrapolation(step, timeValue, cData, context);
                t = t - step.startTime;
                float weight = CalcWeight(step, t, step.duration, cData, context);
                if (step.name == "sief" && cData.info.index == 0)
                    Debug.LogWarning("animating sief with " + t + " w " + weight);

                LerpAnimationStepWeighted(step, weight, cData, context,
                    modifiersStorage, modifiersStorage2, current);

                accModifier.MeshModifiers.Combine(current.MeshModifiers);
                accModifier.CharacterModifiers.Combine(current.CharacterModifiers);
            }

            cData.MeshModifiers.Combine(accModifier.MeshModifiers);
            cData.CharacterModifiers.Combine(accModifier.CharacterModifiers);
        }

        private int FindCurrentlyActive(float timeValue, List<AnimationStep> steps)
        {
            if (steps == null || steps.Count == 0) return -1;

            // TODO This asssumes each track is sorted
            for (int i = 0; i < steps.Count; i++)
            {
                var step = steps[i];

                if (step == null) continue;

                // If on step
                if (step.startTime < timeValue && step.EndTime > timeValue)
                    return i;

                // // If after step
                // if (step.EndTime < timeValue)
                // {
                //     if (i == (steps.Count - 1) || step.postExtrapolation != AnimationStep.ExtrapolationMode.None)
                //         return i;
                //     if (steps[i + 1].preExtrapolation != AnimationStep.ExtrapolationMode.None)
                //         return i + 1;
                //
                //     Debug.LogWarning(i + " is after");
                //     return -1;
                // }

                // If before step
                if (step.startTime > timeValue)
                {
                    if (i == 0)
                    {
                        if (step.preExtrapolation != AnimationStep.ExtrapolationMode.None) return i;
                        return -1;
                    }

                    if (steps[i - 1].postExtrapolation != AnimationStep.ExtrapolationMode.None)
                        return i - 1;

                    if (step.preExtrapolation != AnimationStep.ExtrapolationMode.None)
                        return i;

                    return -1;
                }
            }

            return steps.Count - 1;
        }

        [AutoParametersStorage]
        private partial class AutoParametersData
        {
            public List<List<AnimationStep>> Steps = null;
        }

        private struct StepComparer : IComparer<AnimationStep>
        {
            public int Compare(AnimationStep x, AnimationStep y)
            {
                if (x == null || y == null) return 0;
                if (x.startTime < y.startTime) return -1;
                if (x.startTime > y.startTime) return 1;
                if (x.EndTime < y.EndTime) return -1;
                if (x.EndTime > y.EndTime) return 1;
                return 0;
            }
        }
    }
}