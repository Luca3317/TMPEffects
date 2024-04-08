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
        [Tooltip("How long the animation will take to fully hide the character.\nAliases: duration, dur, d")]
        [SerializeField] float duration = 1f;

        [Tooltip("The maximum X amplitude of each shake.\nAliases: maxxamplitude, maxxamp, maxxa, maxx")]
        [SerializeField] float maxXAmplitude = 5;
        [Tooltip("The minimum X amplitude of each shake.\nAliases: minxamplitude, minxamp, minxa, minx")]
        [SerializeField] float minXAmplitude = 5;
        [Tooltip("The maximum Y amplitude of each shake.\nAliases: maxyamplitude, maxyamp, maxya, maxy")]
        [SerializeField] float maxYAmplitude = 5;
        [Tooltip("The minimum Y amplitude of each shake.\nAliases: minyamplitude, minyamp, minya, miny")]
        [SerializeField] float minYAmplitude = 5;

        [Tooltip("The minimum amount of time to wait after each shake.\nAliases: minwait, minw")]
        [SerializeField] float minWait = 0.1f;
        [Tooltip("The maximum amount of time to wait after each shake.\nAliases: maxwait, maxw")]
        [SerializeField] float maxWait = 0.1f;

        [Tooltip("The curve that defines the falloff of the wait between each shake.\nAliases: waitcurve, waitcrv, waitc")]
        [SerializeField] AnimationCurve waitCurve = AnimationCurveUtility.Linear();
        [Tooltip("The curve that defines the falloff of the amplitude of each shake.\nAliases: amplitudecurve, amplitudecrv, amplitudec, ampcurve, ampcrv, ampc")]
        [SerializeField] AnimationCurve amplitudeCurve = AnimationCurveUtility.Invert(AnimationCurveUtility.Linear());

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.CustomData as Data;

            if (!d.init)
            {
                d.init = true;

                InitRNGDict(context);
                InitLastUpdatedDict(context);
                InitDelayDict(context);
                InitOffsetDict(context);
            }

            float t = Mathf.Lerp(0, 1, (context.AnimatorContext.PassedTime - context.AnimatorContext.StateTime(cData)) / d.duration);

            float delayMult = d.waitCurve.Evaluate(1 - t);
            float ampMult = d.ampCurve.Evaluate(1 - t);

            int segmentIndex = context.SegmentData.SegmentIndexOf(cData);
            float remaining = d.duration - (context.AnimatorContext.PassedTime - context.AnimatorContext.StateTime(cData));

            if (t == 1)
            {
                d.delayDict[segmentIndex] = 0f;
                d.lastUpdatedDict[segmentIndex] = 0f;
                d.offsetDict[segmentIndex] = Vector2.zero;
                context.FinishAnimation(cData);
                return;
            }

            Vector3 offset;


            if (context.AnimatorContext.PassedTime - d.lastUpdatedDict[segmentIndex] >= d.delayDict[segmentIndex] && remaining >= d.minWait * delayMult)
            {
                float xAmp = d.maxXAmplitude == d.minXAmplitude ? d.maxXAmplitude : Mathf.Lerp(d.minXAmplitude, d.maxXAmplitude, (float)d.rngDict[segmentIndex].NextDouble());
                float yAmp = d.maxYAmplitude == d.minYAmplitude ? d.maxYAmplitude : Mathf.Lerp(d.minYAmplitude, d.maxYAmplitude, (float)d.rngDict[segmentIndex].NextDouble());

                float delay = d.maxWait == d.minWait ? d.maxWait : Mathf.Lerp(d.minWait, d.maxWait, (float)d.rngDict[segmentIndex].NextDouble());
                delay *= delayMult;
                delay = Mathf.Clamp(delay, 0f, remaining);
                d.delayDict[segmentIndex] = delay;

                d.lastUpdatedDict[segmentIndex] = context.AnimatorContext.PassedTime;

                float xOffset = ((float)d.rngDict[segmentIndex].NextDouble() * 2f - 1f) * xAmp * ampMult;
                float yOffset = ((float)d.rngDict[segmentIndex].NextDouble() * 2f - 1f) * yAmp * ampMult;

                d.offsetDict[segmentIndex] = new Vector3(xOffset, yOffset, 0f);
            }

            offset = d.offsetDict[segmentIndex];
            cData.SetPosition(cData.InitialPosition + offset);
        }

        private void InitRNGDict(IAnimationContext context)
        {
            Data d = context.CustomData as Data;
            int seed = (int)(context.AnimatorContext.PassedTime * 1000);
            d.rngDict = new Dictionary<int, System.Random>(context.SegmentData.length);
            for (int i = 0; i < context.SegmentData.length; i++)
            {
                d.rngDict.Add(i, new System.Random(seed + i));
            }
        }
         
        private void InitLastUpdatedDict(IAnimationContext context)
        {
            Data d = context.CustomData as Data;
            d.lastUpdatedDict = new Dictionary<int, float>(context.SegmentData.length);
            for (int i = 0; i < context.SegmentData.length; i++)
            {
                d.lastUpdatedDict.Add(i, context.AnimatorContext.PassedTime);
            }
        }

        private void InitDelayDict(IAnimationContext context)
        {
            Data d = context.CustomData as Data;
            d.delayDict = new Dictionary<int, float>(context.SegmentData.length);

            for (int i = 0; i < context.SegmentData.length; i++)
            {
                d.delayDict.Add(i, 0);
            }
        }

        private void InitOffsetDict(IAnimationContext context)
        {
            Data d = context.CustomData as Data;
            d.offsetDict = new Dictionary<int, Vector2>(context.SegmentData.length);

            for (int i = 0; i < context.SegmentData.length; i++)
            {
                d.offsetDict.Add(i, Vector2.zero);
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float f, parameters, "maxxamplitude", "maxxamp", "maxxa", "maxx")) d.maxXAmplitude = f;
            if (TryGetFloatParameter(out f, parameters, "duration", "dur", "d")) d.duration = f;
            if (TryGetFloatParameter(out f, parameters, "maxyamplitude", "maxyamp", "maxya", "maxy")) d.maxYAmplitude = f;
            if (TryGetFloatParameter(out f, parameters, "minxamplitude", "minxamp", "minxa", "minx")) d.minXAmplitude = f;
            if (TryGetFloatParameter(out f, parameters, "minyamplitude", "minyamp", "minya", "miny")) d.minYAmplitude = f;
            if (TryGetFloatParameter(out f, parameters, "minwait", "minw")) d.minWait = f;
            if (TryGetFloatParameter(out f, parameters, "maxwait", "maxw")) d.maxWait = f;
            if (TryGetAnimCurveParameter(out var crv, parameters, "waitcurve", "waitcrv", "waitc")) d.waitCurve = crv;
            if (TryGetAnimCurveParameter(out crv, parameters, "amplitudecurve", "amplitudecrv", "amplitudec", "ampcurve", "ampcrv", "ampc")) d.ampCurve = crv;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "maxxamplitude", "maxxamp", "maxxa", "maxx")) return false;
            if (HasNonFloatParameter(parameters, "duration", "dur", "d")) return false;
            if (HasNonFloatParameter(parameters, "maxyamplitude", "maxyamp", "maxya", "maxy")) return false;
            if (HasNonFloatParameter(parameters, "minxamplitude", "minxamp", "minxa", "minx")) return false;
            if (HasNonFloatParameter(parameters, "minyamplitude", "minyamp", "minya", "miny")) return false;
            if (HasNonFloatParameter(parameters, "minwait", "minw")) return false;
            if (HasNonFloatParameter(parameters, "maxwait", "maxw")) return false;
            if (HasNonAnimCurveParameter(parameters, "waitcurve", "waitcrv", "waitc")) return false;
            if (HasNonAnimCurveParameter(parameters, "amplitudecurve", "amplitudecrv", "amplitudec", "ampcurve", "ampcrv", "ampc")) return false;
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
                minWait = this.minWait,
                maxWait = this.maxWait,
                waitCurve = this.waitCurve,
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
            public float minWait;
            public float maxWait;
            public AnimationCurve waitCurve;
            public AnimationCurve ampCurve;

            public Dictionary<int, Vector2> offsetDict = null;
            public Dictionary<int, float> lastUpdatedDict = null;
            public Dictionary<int, float> delayDict = null;
            public Dictionary<int, System.Random> rngDict = null;
        }
    }
}