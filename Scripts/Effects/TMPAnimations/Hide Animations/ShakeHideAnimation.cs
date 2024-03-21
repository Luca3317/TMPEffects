using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.ParameterUtility;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    [CreateAssetMenu(fileName = "new ShakeHideAnimation", menuName = "TMPEffects/Hide Animations/Shake")]
    public class ShakeHideAnimation : TMPHideAnimation
    {
        [SerializeField] float duration = 1f;

        [SerializeField] float maxXAmplitude = 5;
        [SerializeField] float minXAmplitude = 5;
        [SerializeField] float maxYAmplitude = 5;
        [SerializeField] float minYAmplitude = 5;

        [SerializeField] float minDelay = 0.1f;
        [SerializeField] float maxDelay = 0.1f;

        [SerializeField] AnimationCurve delayCurve = AnimationCurveUtility.Invert(AnimationCurveUtility.Linear());
        [SerializeField] AnimationCurve amplitudeCurve = AnimationCurveUtility.Linear();

        public override void Animate(CharData cData, IAnimationContext context)
        {

            Data d = context.customData as Data;

            if (!d.init)
            {
                d.init = true;

                InitRNGDict(context);
                InitLastUpdatedDict(context);
                InitDelayDict(context);
                InitOffsetDict(context);
            }

            float t = Mathf.Lerp(0, 1, (context.animatorContext.PassedTime - context.animatorContext.StateTime(cData)) / d.duration);

            float delayMult = d.delayCurve.Evaluate(t);
            float ampMult = d.ampCurve.Evaluate(t);

            int segmentIndex = context.segmentData.SegmentIndexOf(cData);
            float remaining = d.duration - (context.animatorContext.PassedTime - context.animatorContext.StateTime(cData));

            if (t == 1 || remaining < d.minDelay)
            {
                d.delayDict[segmentIndex] = 0f;
                d.lastUpdatedDict[segmentIndex] = 0f;
                d.offsetDict[segmentIndex] = Vector2.zero;
                context.FinishAnimation(cData);
                return;
            }

            Vector3 offset;

            if (context.animatorContext.PassedTime - d.lastUpdatedDict[segmentIndex] >= d.delayDict[segmentIndex] && remaining >= d.minDelay * delayMult)
            {
                float xAmp = d.maxXAmplitude == d.minXAmplitude ? d.maxXAmplitude : Mathf.Lerp(d.minXAmplitude, d.maxXAmplitude, (float)d.rngDict[segmentIndex].NextDouble());
                float yAmp = d.maxYAmplitude == d.minYAmplitude ? d.maxYAmplitude : Mathf.Lerp(d.minYAmplitude, d.maxYAmplitude, (float)d.rngDict[segmentIndex].NextDouble());

                float delay = d.maxDelay == d.minDelay ? d.maxDelay : Mathf.Lerp(d.minDelay, d.maxDelay, (float)d.rngDict[segmentIndex].NextDouble());
                delay *= delayMult;
                delay = Mathf.Clamp(delay, d.delayDict[segmentIndex], remaining);
                d.delayDict[segmentIndex] = delay;

                d.lastUpdatedDict[segmentIndex] = context.animatorContext.PassedTime;

                float xOffset = ((float)d.rngDict[segmentIndex].NextDouble() * 2f - 1f) * xAmp * ampMult;
                float yOffset = ((float)d.rngDict[segmentIndex].NextDouble() * 2f - 1f) * yAmp * ampMult;

                d.offsetDict[segmentIndex] = new Vector3(xOffset, yOffset, 0f);
            }

            offset = d.offsetDict[segmentIndex];
            cData.SetPosition(cData.info.initialPosition + offset);
        }

        private void InitRNGDict(IAnimationContext context)
        {
            Data d = context.customData as Data;
            int seed = (int)(context.animatorContext.PassedTime * 1000);
            d.rngDict = new Dictionary<int, System.Random>(context.segmentData.length);
            for (int i = 0; i < context.segmentData.length; i++)
            {
                d.rngDict.Add(i, new System.Random(seed + i));
            }
        }

        private void InitLastUpdatedDict(IAnimationContext context)
        {
            Data d = context.customData as Data;
            d.lastUpdatedDict = new Dictionary<int, float>(context.segmentData.length);
            for (int i = 0; i < context.segmentData.length; i++)
            {
                d.lastUpdatedDict.Add(i, context.animatorContext.PassedTime);
            }
        }

        private void InitDelayDict(IAnimationContext context)
        {
            Data d = context.customData as Data;
            d.delayDict = new Dictionary<int, float>(context.segmentData.length);

            for (int i = 0; i < context.segmentData.length; i++)
            {
                d.delayDict.Add(i, 0);
            }
        }

        private void InitOffsetDict(IAnimationContext context)
        {
            Data d = context.customData as Data;
            d.offsetDict = new Dictionary<int, Vector2>(context.segmentData.length);

            for (int i = 0; i < context.segmentData.length; i++)
            {
                d.offsetDict.Add(i, Vector2.zero);
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float f, parameters, "maxXAmplitude", "maxXAmp", "maxXA", "maxX")) d.maxXAmplitude = f;
            if (TryGetFloatParameter(out f, parameters, "duration", "dur", "d")) d.duration = f;
            if (TryGetFloatParameter(out f, parameters, "maxYAmplitude", "maxYAmp", "maxYA", "maxY")) d.maxYAmplitude = f;
            if (TryGetFloatParameter(out f, parameters, "minXAmplitude", "minXAmp", "minXA", "minX")) d.minXAmplitude = f;
            if (TryGetFloatParameter(out f, parameters, "minYAmplitude", "minYAmp", "minYA", "minY")) d.minYAmplitude = f;
            if (TryGetFloatParameter(out f, parameters, "minDelay", "minD")) d.minDelay = f;
            if (TryGetFloatParameter(out f, parameters, "maxDelay", "maxD")) d.maxDelay = f;
            if (TryGetAnimCurveParameter(out var crv, parameters, "delayCurve", "delayCrv", "delayC")) d.delayCurve = crv;
            if (TryGetAnimCurveParameter(out crv, parameters, "amplitudeCurve", "amplitudeCrv", "amplitudeC", "ampCurve", "ampCrv", "ampC")) d.ampCurve = crv;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "maxXAmplitude", "maxXAmp", "maxXA", "maxX")) return false;
            if (HasNonFloatParameter(parameters, "duration", "dur", "d")) return false;
            if (HasNonFloatParameter(parameters, "maxYAmplitude", "maxYAmp", "maxYA", "maxY")) return false;
            if (HasNonFloatParameter(parameters, "minXAmplitude", "minXAmp", "minXA", "minX")) return false;
            if (HasNonFloatParameter(parameters, "minYAmplitude", "minYAmp", "minYA", "minY")) return false;
            if (HasNonFloatParameter(parameters, "minDelay", "minD")) return false;
            if (HasNonFloatParameter(parameters, "maxDelay", "maxD")) return false;
            if (HasNonAnimCurveParameter(parameters, "delayCurve", "delayCrv", "delayC")) return false;
            if (HasNonAnimCurveParameter(parameters, "amplitudeCurve", "amplitudeCrv", "amplitudeC", "ampCurve", "ampCrv", "ampC")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                init = false,
                duration = this.duration,
                maxXAmplitude = this.maxXAmplitude,
                minXAmplitude = this.minXAmplitude,
                minYAmplitude = this.minYAmplitude,
                maxYAmplitude = this.maxYAmplitude,
                minDelay = this.minDelay,
                maxDelay = this.maxDelay,
                delayCurve = this.delayCurve,
                ampCurve = this.amplitudeCurve,

                offsetDict = null,
                lastUpdatedDict = null,
                delayDict = null,
                rngDict = null,
            };
        }

        private class Data
        {
            public float duration;
            public bool init;

            public float maxXAmplitude;
            public float minXAmplitude;
            public float maxYAmplitude;
            public float minYAmplitude;
            public float minDelay;
            public float maxDelay;
            public AnimationCurve delayCurve;
            public AnimationCurve ampCurve;

            public Dictionary<int, Vector2> offsetDict = null;
            public Dictionary<int, float> lastUpdatedDict = null;
            public Dictionary<int, float> delayDict = null;
            public Dictionary<int, System.Random> rngDict = null;
        }
    }
}