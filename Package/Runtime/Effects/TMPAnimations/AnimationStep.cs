using System;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.Modifiers;
using TMPEffects.Parameters;
using UnityEngine;

namespace TMPEffects.TMPAnimations
{
    [Serializable]
    public class AnimationStep
    {
        public string name;
        public bool animate = true;

        // HideInInspector so it doesnt show "Add from TMPOffsetProvider" in context menu of timeline
        [HideInInspector] public TMPBlendCurve entryCurve;
        public float entryDuration;

        [HideInInspector] public TMPBlendCurve exitCurve;
        public float exitDuration;

        public ExtrapolationMode preExtrapolation;
        public ExtrapolationMode postExtrapolation;
    
        public bool useWave;
        public Wave wave;
        public OffsetBundle waveOffset;

        public float startTime;
        public float duration;

        // Get end time of this step; if loops forever (repetitions == 0), float.MaxValue
        public float EndTime => startTime + duration;

        public bool useInitialModifiers = false;
        public EditorFriendlyCharDataModifiers initModifiers;
        public EditorFriendlyCharDataModifiers modifiers;

        public AnimationStep()
        {
        }

        public AnimationStep(AnimationStep original)
        {
            name = original.name;
            animate = original.animate;

            entryCurve = new TMPBlendCurve(original.entryCurve);
            entryDuration = original.entryDuration;
            exitCurve = new TMPBlendCurve(original.exitCurve);
            exitDuration = original.exitDuration;

            preExtrapolation = original.preExtrapolation;
            postExtrapolation = original.postExtrapolation;

            useWave = original.useWave;
            waveOffset = original.waveOffset;
            wave = new Wave(original.wave);

            startTime = original.startTime;
            duration = original.duration;

            useInitialModifiers = original.useInitialModifiers;
            initModifiers = new EditorFriendlyCharDataModifiers(original.initModifiers);
            modifiers = new EditorFriendlyCharDataModifiers(original.modifiers);
        }

        public enum ExtrapolationMode
        {
            None,
            Hold,
            Loop,
            PingPong,
            Continue
        }

        /// <summary>
        /// Calculate the weight of the step using the time value.
        /// </summary>
        /// <param name="timeValue"></param>
        /// <param name="duration"></param>
        /// <param name="cData"></param>
        /// <param name="context"></param>
        /// <param name="segmentData"></param>
        /// <returns></returns>
        public float CalcWeight(float timeValue, float duration,
            CharData cData, IAnimatorDataProvider context, ITMPSegmentData segmentData)
        {
            return CalcWeight(this, timeValue, duration, cData, context, segmentData);
        }

        /// <summary>
        /// Calculate the weight of the step using the time value.
        /// </summary>
        /// <param name="timeValue"></param>
        /// <param name="duration"></param>
        /// <param name="cData"></param>
        /// <param name="context"></param>
        /// <param name="segmentData"></param>
        /// <returns></returns>
        public static float CalcWeight(AnimationStep step, float timeValue, float duration,
            CharData cData, IAnimatorDataProvider context, ITMPSegmentData segmentData)
        {
            float weight = 1;

            if (step.entryDuration > 0)
            {
                weight = step.entryCurve.EvaluateIn(timeValue, step.entryDuration, cData, context, segmentData);
            }

            if (step.exitDuration > 0)
            {
                float preTime = duration - step.exitDuration;
                var multiplier =
                    step.exitCurve.EvaluateOut(timeValue, step.exitDuration, preTime, cData, context, segmentData);
                weight *= multiplier;
            }

            if (step.useWave)
            {
                var waveOffset = step.waveOffset.GetOffset(cData, context, segmentData);
                weight *= step.wave.Evaluate(timeValue, waveOffset).Value;
            }

            return weight;
        }

        public static float CalcWeight(AnimationStep step, float timeValue, float duration,
            CharData cData, IAnimatorDataProvider context, ITMPSegmentData segmentData, GenericAnimationUtility.CachedOffset inOffset,
            GenericAnimationUtility.CachedOffset outOffset, float waveOffset = 0)
        {
            float weight = 1;

            if (step.entryDuration > 0)
            {
                weight = step.entryCurve.EvaluateIn(timeValue, step.entryDuration, inOffset.minOffset, inOffset.maxOffset,
                    inOffset.offset[cData]);
            }

            if (step.exitDuration > 0)
            {
                float preTime = duration - step.exitDuration;
                var multiplier =
                    step.exitCurve.EvaluateOut(timeValue, step.exitDuration, preTime, outOffset.minOffset,
                        outOffset.maxOffset, outOffset.offset[cData]);
                weight *= multiplier;
            }

            if (step.useWave)
            {
                // var waveOffset = step.waveOffset.GetOffset(cData, context, segmentData);
                weight *= step.wave.Evaluate(timeValue, waveOffset).Value;
            }

            return weight;
        }

        public void LerpAnimationStepWeighted(float weight, CharData cData,
            IAnimatorContext context,
            CharDataModifiers storage, CharDataModifiers storage2, CharDataModifiers result)
        {
            LerpAnimationStepWeighted(this, weight, cData, context, storage, storage2, result);
        }

        /// <summary>
        /// Lerp the step using the weight
        /// </summary>
        /// <param name="timeValue"></param>
        /// <param name="duration"></param>
        /// <param name="cData"></param>
        /// <param name="context"></param>
        /// <param name="segmentData"></param>
        /// <returns></returns>
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
    }
}