using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.Modifiers;
using UnityEngine;

namespace TMPEffects.TMPAnimations
{
    public interface IGenericAnimation
    {
        public GenericAnimationUtility.TrackList Tracks { get; }
    }

    /// <summary>
    /// A generic <see cref="ITMPAnimation"/>, allowing you to create animations in the inspector.
    /// </summary>
    [AutoParameters]
    [CreateAssetMenu(fileName = "new GenericAnimation",
        menuName = "TMPEffects/Animations/Basic Animations/Generic Animation")]
    public sealed partial class GenericAnimation : TMPAnimation, IGenericAnimation
    {
        #region Editor stuff

        protected override void OnValidate()
        {
            base.OnValidate();
            GenericAnimationUtility.EnsureNonOverlappingTimings_Editor(Tracks);
        }

        #endregion

        #region Fields + Properties

        [field: SerializeField]
        public GenericAnimationUtility.TrackList Tracks { get; set; } = new GenericAnimationUtility.TrackList();

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
            GenericAnimationUtility.Animate(cData, Tracks, ref data.Steps, data.CachedOffsets, data.repeat,
                data.duration, context.AnimatorContext.PassedTime, context, ref modifiersStorage, ref modifiersStorage2,
                ref accModifier, ref current);
        }

        [AutoParametersStorage]
        private partial class AutoParametersData
        {
            public List<List<AnimationStep>> Steps = null;

            [CanBeNull] public Dictionary<AnimationStep,
                (GenericAnimationUtility.CachedOffset inOffset, GenericAnimationUtility.CachedOffset outOffset)> CachedOffsets =
                new Dictionary<AnimationStep, (GenericAnimationUtility.CachedOffset inOffset, GenericAnimationUtility.CachedOffset outOffset
                    )>();
        }
    }

    public static class GenericAnimationUtility
    {
        public static void EnsureNonOverlappingTimings_Editor(TrackList trackList)
        {
            for (int i = 0; i < trackList.Tracks.Count; i++)
            {
                var track = trackList.Tracks[i];

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
                copy.Sort(new GenericAnimationUtility.StepComparer());
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

                // Uncommenting this will auto force sort clips
                // (which makes the reorderable part of them sort of pointless unless same start and 0 duration)
                // Maybe make an editor only bool for whether to do this
                // Tracks.Tracks[i].Clips = copy;
            }
        }

        public static void EnsureNonOverlappingTimings(List<List<AnimationStep>> steps)
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
                copy.Sort(new GenericAnimationUtility.StepComparer());
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

                // Uncommenting this will auto force sort clips
                // (which makes the reorderable part of them sort of pointless unless same start and 0 duration)
                // Tracks.Tracks[i].Clips = copy;
            }
        }


        #region Types

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

        public struct StepComparer : IComparer<AnimationStep>
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

        // Ensure steps are sorted
        public static void CreateStepsSorted(TrackList tracks, ref List<List<AnimationStep>> steps)
        {
            if (steps != null) return;

            steps = new List<List<AnimationStep>>();
            for (int i = 0; i < tracks.Tracks.Count; i++)
            {
                var track = tracks.Tracks[i];
                var copy = new List<AnimationStep>(track.Clips);
                copy.Sort(new StepComparer());
                steps.Add(copy);
            }
        }

        // Adjust the raw time value according to the extrapolation
        public static float AdjustTimeForExtrapolation(AnimationStep step, float timeValue)
        {
            if (timeValue < 0f)
            {
                switch (step.preExtrapolation)
                {
                    case AnimationStep.ExtrapolationMode.Continue:
                        break; // Continue only different in how the value should be handled
                    case AnimationStep.ExtrapolationMode.Hold:
                        timeValue = 0;
                        break;
                    case AnimationStep.ExtrapolationMode.Loop:
                        float diff = (-timeValue) % step.duration;
                        timeValue = step.duration - diff;
                        break;
                    case AnimationStep.ExtrapolationMode.PingPong:
                        diff = (-timeValue) % step.duration;
                        timeValue = diff;
                        break;
                }
            }

            if (timeValue > step.duration)
            {
                switch (step.postExtrapolation)
                {
                    case AnimationStep.ExtrapolationMode.Continue:
                        break; // Continue only different in how the value should be handled
                    case AnimationStep.ExtrapolationMode.Hold:
                        timeValue = step.duration;
                        break;
                    case AnimationStep.ExtrapolationMode.Loop:
                        float diff = (timeValue - step.duration) % step.duration;
                        timeValue = diff;
                        break;
                    case AnimationStep.ExtrapolationMode.PingPong:
                        diff = (timeValue - step.duration) % step.duration;
                        timeValue = step.duration - diff;
                        break;
                }
            }

            return timeValue;
        }

        // Get the currently active step
        public static int FindCurrentlyActive(float timeValue, List<AnimationStep> steps)
        {
            if (steps == null || steps.Count == 0) return -1;

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
        
        public struct CachedOffset
        {
            public Dictionary<CharData, float> offset;
            public float minOffset;
            public float maxOffset;
        }

        public static void Animate(CharData cData, TrackList tracks, ref List<List<AnimationStep>> dataSteps,
            Dictionary<AnimationStep, (CachedOffset inOffset, CachedOffset outOffset)>
                cachedOffsets,
            bool repeat, float duration, float passedTime, IAnimationContext context,
            ref CharDataModifiers modifiersStorage,
            ref CharDataModifiers modifiersStorage2, ref CharDataModifiers accModifier, ref CharDataModifiers current)
        {
            if (dataSteps == null)
                GenericAnimationUtility.CreateStepsSorted(tracks, ref dataSteps);

            IAnimatorContext ac = context.AnimatorContext;
            var steps = dataSteps;

            // Create modifiers if needed
            modifiersStorage ??= new CharDataModifiers();
            modifiersStorage2 ??= new CharDataModifiers();
            accModifier ??= new CharDataModifiers();
            current ??= new CharDataModifiers();

            // Calculate timeValue
            float timeValue =
                repeat ? passedTime % duration : passedTime;

            // Reset accModifier
            accModifier.Reset();

            // For every track
            foreach (var track in steps)
            {
                // Find the currently active clip (max 1)
                var active = GenericAnimationUtility.FindCurrentlyActive(timeValue, track);
                if (active == -1) continue;

                // If that clip is disabled continue
                var step = track[active];
                if (!step.animate) continue;

                // Adjust the timeValue for extrapolation setting
                float t = timeValue - step.startTime;
                t = GenericAnimationUtility.AdjustTimeForExtrapolation(step, t);

                // Get (and cache) the min / max / current offset
                if (!cachedOffsets.TryGetValue(step, out var cachedOffset))
                {
                    step.entryCurve.provider.GetMinMaxOffset(out float inMin, out float inMax, context.SegmentData,
                        context.AnimatorContext);
                    step.exitCurve.provider.GetMinMaxOffset(out float outMin, out float outMax, context.SegmentData,
                        context.AnimatorContext);
                    cachedOffset = (
                        new CachedOffset()
                            { minOffset = inMin, maxOffset = inMax, offset = new Dictionary<CharData, float>() },
                        new CachedOffset()
                            { minOffset = outMin, maxOffset = outMax, offset = new Dictionary<CharData, float>() });

                    cachedOffsets[step] = cachedOffset;
                }

                if (!cachedOffset.inOffset.offset.TryGetValue(cData, out float inOffset))
                {
                    inOffset = step.entryCurve.provider.GetOffset(cData, context.SegmentData, context.AnimatorContext);
                    cachedOffset.inOffset.offset[cData] = inOffset;
                }

                if (!cachedOffset.outOffset.offset.TryGetValue(cData, out float outOffset))
                {
                    outOffset = step.entryCurve.provider.GetOffset(cData, context.SegmentData, context.AnimatorContext);
                    cachedOffset.outOffset.offset[cData] = outOffset;
                }

                // Calculate the weight of the current clip
                float weight = AnimationStep.CalcWeight(step, t, step.duration, cData, ac, context.SegmentData,
                    cachedOffset.inOffset, cachedOffset.outOffset);

                // Lerp the animation step using the weight
                AnimationStep.LerpAnimationStepWeighted(step, weight, cData, ac,
                    modifiersStorage, modifiersStorage2, current);

                // Combine the lerped modifiers with the previously calculated ones
                accModifier.MeshModifiers.Combine(current.MeshModifiers);
                accModifier.CharacterModifiers.Combine(current.CharacterModifiers);
            }

            // Combine the calculated modifiers into the CharData's one
            cData.MeshModifiers.Combine(accModifier.MeshModifiers);
            cData.CharacterModifiers.Combine(accModifier.CharacterModifiers);
        }
    }
}