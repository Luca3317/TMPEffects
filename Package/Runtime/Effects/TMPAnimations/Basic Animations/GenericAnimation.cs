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
        #region Editor stuff

        // TODO Theres a new weird issue "A previewrenderutility was not cleaned up"
        // TODO Potentially might have smth to do with this. Idk why but it started happening around the time i implemented this.
        protected override void OnValidate()
        {
            base.OnValidate();
            EnsureNonOverlappingTimings_Editor();
        }

        private void EnsureNonOverlappingTimings_Editor()
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

        private void EnsureNonOverlappingTimings(List<List<AnimationStep>> steps)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                var track = steps[i];

                // Ensure no negative values
                for (int j = 0; j < track.Count; j++)
                {
                    var step = track[j];
                    if (step == null) continue;

                    if (step.duration < 0)
                        step.duration = 0;
                    if (step.startTime < 0)
                        step.startTime = 0;
                }

                var copy = new List<AnimationStep>(track);
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

        #endregion

        #region Custom types

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

        #endregion

        #region Fields + Properties

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

        #endregion

        private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
        {
            CreateStepsSorted(ref data.Steps);

            IAnimatorContext ac = context.AnimatorContext;
            var steps = data.Steps;

            // Create modifiers if needed
            modifiersStorage ??= new CharDataModifiers();
            modifiersStorage2 ??= new CharDataModifiers();
            accModifier ??= new CharDataModifiers();
            current ??= new CharDataModifiers();

            // Calculate timeValue
            float timeValue =
                data.repeat ? ac.PassedTime % data.duration : ac.PassedTime;

            // Reset accModifier
            accModifier.Reset();

            // For every track
            foreach (var track in steps)
            {
                // Find the currently active clip (max 1)
                var active = FindCurrentlyActive(timeValue, track);
                if (active == -1) continue;

                // If that clip is disabled continue
                var step = track[active];
                if (!step.animate) continue;

                // Adjust the timeValue for extrapolation setting
                float t = timeValue - step.startTime;
                t = AdjustTimeForExtrapolation(step, t, cData, ac);

                // Calculate the weight of the current clip
                float weight = CalcWeight(step, t, step.duration, cData, ac);

                // Lerp the animation step using the weight
                LerpAnimationStepWeighted(step, weight, cData, ac,
                    modifiersStorage, modifiersStorage2, current);

                // Combine the lerped modifiers with the previously calculated ones
                accModifier.MeshModifiers.Combine(current.MeshModifiers);
                accModifier.CharacterModifiers.Combine(current.CharacterModifiers);
            }

            // Combine the calculated modifiers into the CharData's one
            cData.MeshModifiers.Combine(accModifier.MeshModifiers);
            cData.CharacterModifiers.Combine(accModifier.CharacterModifiers);
        }

        // Ensure steps are sorted
        private void CreateStepsSorted(ref List<List<AnimationStep>> steps)
        {
            if (steps != null) return;

            steps = new List<List<AnimationStep>>();
            for (int i = 0; i < Tracks.Tracks.Count; i++)
            {
                var track = Tracks.Tracks[i];
                var copy = new List<AnimationStep>(track.Clips);
                copy.Sort(new StepComparer());
                steps.Add(copy);
            }
        }

        // Get the currently active step
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

                // If before step
                if (step.startTime > timeValue)
                {
                    if (i == 0)
                    {
                        if (step.preExtrapolation != AnimationStep.ExtrapolationMode.None)
                            return i;

                        return -1;
                    }

                    if (steps[i - 1].postExtrapolation != AnimationStep.ExtrapolationMode.None)
                    {
                        return i - 1;
                    }

                    if (step.preExtrapolation != AnimationStep.ExtrapolationMode.None)
                        return i;

                    return -1;
                }
            }

            // Wasnt before or on any clip => must be after the last one
            if (steps[^1].postExtrapolation != AnimationStep.ExtrapolationMode.None)
                return steps.Count - 1;

            return -1;
        }

        // Adjust the raw time value according to the extrapolation
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

        // Calculate the weight of the step using the time value
        // TODO This should have an overload that takes an IANimationContext
        // so the fancyanimationcurve evaluates can use the segmentData
        // Dont do this before deciding how / whether to split the anim context
        // interfaces into smaller ones
        public static float CalcWeight(AnimationStep step, float timeValue, float duration,
            CharData cData, IAnimatorContext context)
        {
            float weight = 1;

            if (step.entryDuration > 0)
            {
                weight = step.entryCurve.EvaluateIn(cData, context, timeValue, step.entryDuration);
            }

            if (step.exitDuration > 0)
            {
                float preTime = duration - step.exitDuration;
                var multiplier =
                    step.exitCurve.EvaluateOut(cData, context, timeValue, step.exitDuration, preTime);
                weight *= multiplier;
            }

            if (step.useWave)
            {
                var waveOffset = step.waveOffset.GetOffset(cData, context);
                weight *= step.wave.Evaluate(timeValue, waveOffset).Value;
            }

            return weight;
        }

        // Backup from before moving logic into fancyanimationcurve
        // public static float CalcWeightOLD(AnimationStep step, float timeValue, float duration,
        //     CharData cData, IAnimatorContext context)
        // {
        //     float weight = 1;
        //
        //     // var entryTimeValue = timeValue - Mathf.Abs(step.entryCurve.GetOffset(cData, context)) /** step.entryCurve.uniformity*/ / context.Animator.TextComponent.GetParsedText().Length;
        //     var seglen = context.Animator.TextComponent.GetParsedText().Length;
        //
        //
        //     // We know biggest offset, lets call it x; it means, after x + duration seconds the last one is done
        //     // we know how long we want everything to take (duration)
        //     // ergo we need to get the scalar for whcih (x + duration) * scalar = duration
        //     // x * scalar + duration * scalar = duration
        //     // x * scalar / duration + scalar = 1
        //     // x * scalara / duration = 1 -scalra
        //     // x / duration = 1 / scalar - 1
        //     // scalar = 1 / ((x / duration) + 1)
        //
        //     var mocked = AnimationUtility.GetMockedSegment(seglen, context.Animator.CharData);
        //     var minmax = AnimationUtility.GetMinMaxOffset(ParameterTypes.OffsetType.Index, mocked, context);
        //
        //     float offset = step.entryCurve.GetOffset(cData, context);
        //     float zeroedOffset = offset - minmax.min;
        //     float zeroedMin = 0;
        //     float zeroedMax = minmax.max - minmax.min;
        //
        //     float newOffset;
        //     if (step.entryCurve.uniformity >= 0)
        //     {
        //         newOffset = zeroedOffset;
        //     }
        //     else
        //     {
        //         newOffset = zeroedMax - offset;
        //     }
        //
        //     float scalar = 1f;
        //     if (!step.entryCurve.ignoreSegmentLength)
        //     {
        //         scalar = (zeroedMax * Mathf.Abs(step.entryCurve.uniformity)) / step.entryDuration + 1f;
        //         scalar = 1f / scalar;
        //     }
        //
        //     var entryTimeValue = timeValue -
        //                          newOffset * scalar * Mathf.Abs(step.entryCurve.uniformity);
        //
        //     if (step.entryDuration > 0)
        //     {
        //         weight = step.entryCurve.curve.Evaluate(entryTimeValue / step.entryDuration / scalar);
        //     }
        //
        //
        //     offset = step.exitCurve.GetOffset(cData, context);
        //     zeroedOffset = offset - minmax.min;
        //     zeroedMax = minmax.max - minmax.min;
        //
        //     if (step.exitCurve.uniformity >= 0)
        //     {
        //         newOffset = zeroedOffset;
        //     }
        //     else
        //     {
        //         newOffset = zeroedMax - offset;
        //
        //         // if (zeroedMax == 0)
        //         // {
        //         //     newOffset = 0;
        //         // }
        //         // else
        //         // {
        //         //     newOffset = Mathf.Lerp(zeroedMax, zeroedMin, zeroedOffset / zeroedMax);
        //         // }
        //     }
        //
        //     scalar = 1f;
        //     if (!step.exitCurve.ignoreSegmentLength)
        //     {
        //         scalar = (zeroedMax * Mathf.Abs(step.exitCurve.uniformity)) / step.exitDuration + 1f;
        //         scalar = 1f / scalar;
        //     }
        //
        //     var exitTimeValue = timeValue -
        //                         newOffset * scalar * Mathf.Abs(step.exitCurve.uniformity);
        //
        //     if (step.exitDuration > 0)
        //     {
        //         float preTime = duration - step.exitDuration;
        //         var multiplier =
        //             (step.exitCurve.curve.Evaluate(1f - (exitTimeValue - preTime) / step.exitDuration / scalar));
        //         weight *= multiplier;
        //     }
        //
        //     if (step.useWave)
        //     {
        //         var waveOffset = step.waveOffset.GetOffset(cData, context);
        //         weight *= step.wave.Evaluate(timeValue, waveOffset, step.waveOffset.GetUniformity(context)).Value;
        //     }
        //
        //     return weight;
        // }

        // Lerp the step using the weight
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

        [AutoParametersStorage]
        private partial class AutoParametersData
        {
            public List<List<AnimationStep>> Steps = null;
        }
    }
}