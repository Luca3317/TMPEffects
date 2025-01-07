using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.Parameters.TMPParameterUtility;
using static TMPEffects.Parameters.TMPParameterTypes;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new ShakeShowAnimation",
        menuName = "TMPEffects/Animations/Show Animations/Built-in/Shake")]
    public partial class ShakeShowAnimation : TMPShowAnimation
    {
        [SerializeField, AutoParameter("duration", "dur", "d")]
        [Tooltip("How long the animation will take to fully hide the character.\nAliases: duration, dur, d")]
        float duration = 1f;

        [SerializeField, AutoParameter("maxxamplitude", "maxxamp", "maxxa", "maxx")]
        [Tooltip("The maximum X amplitude of each shake.\nAliases: maxxamplitude, maxxamp, maxxa, maxx")]
        float maxXAmplitude = 5;

        [SerializeField, AutoParameter("minxamplitude", "minxamp", "minxa", "minx")]
        [Tooltip("The minimum X amplitude of each shake.\nAliases: minxamplitude, minxamp, minxa, minx")]
        float minXAmplitude = 5;

        [SerializeField, AutoParameter("maxyamplitude", "maxyamp", "maxya", "maxy")]
        [Tooltip("The maximum Y amplitude of each shake.\nAliases: maxyamplitude, maxyamp, maxya, maxy")]
        float maxYAmplitude = 5;

        [SerializeField, AutoParameter("minyamplitude", "minyamp", "minya", "miny")]
        [Tooltip("The minimum Y amplitude of each shake.\nAliases: minyamplitude, minyamp, minya, miny")]
        float minYAmplitude = 5;

        [SerializeField, AutoParameter("minwait", "minw")]
        [Tooltip("The minimum amount of time to wait after each shake.\nAliases: minwait, minw")]
        float minWait = 0.1f;

        [SerializeField, AutoParameter("maxwait", "maxw")]
        [Tooltip("The maximum amount of time to wait after each shake.\nAliases: maxwait, maxw")]
        float maxWait = 0.1f;

        [SerializeField, AutoParameter("waitcurve", "waitcrv", "waitc")]
        [Tooltip(
            "The curve that defines the falloff of the wait between each shake.\nAliases: waitcurve, waitcrv, waitc")]
        AnimationCurve waitCurve = AnimationCurveUtility.Linear();

        [SerializeField,
         AutoParameter("amplitudecurve", "amplitudecrv", "amplitudec", "amplitudec", "ampcurve", "ampcrv", "ampc")]
        [Tooltip(
            "The curve that defines the falloff of the amplitude of each shake.\nAliases: amplitudecurve, amplitudecrv, amplitudec, ampcurve, ampcrv, ampc")]
        AnimationCurve amplitudeCurve = AnimationCurveUtility.Invert(AnimationCurveUtility.Linear());

        private partial void Animate(CharData cData, Data data, IAnimationContext context)
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

            float t = Mathf.Lerp(0, 1,
                (context.AnimatorContext.PassedTime - context.AnimatorContext.StateTime(cData)) / d.duration);

            float delayMult = d.waitCurve.Evaluate(t);
            float ampMult = d.amplitudeCurve.Evaluate(t);

            int segmentIndex = context.SegmentData.SegmentIndexOf(cData);
            float remaining =
                d.duration - (context.AnimatorContext.PassedTime - context.AnimatorContext.StateTime(cData));

            if (t == 1)
            {
                d.delayDict[segmentIndex] = 0f;
                d.lastUpdatedDict[segmentIndex] = 0f;
                d.offsetDict[segmentIndex] = Vector2.zero;
                context.FinishAnimation(cData);
                return;
            }

            Vector3 offset;


            if (context.AnimatorContext.PassedTime - d.lastUpdatedDict[segmentIndex] >= d.delayDict[segmentIndex] &&
                remaining >= d.minWait * delayMult)
            {
                float xAmp = d.maxXAmplitude == d.minXAmplitude
                    ? d.maxXAmplitude
                    : Mathf.Lerp(d.minXAmplitude, d.maxXAmplitude, (float)d.rngDict[segmentIndex].NextDouble());
                float yAmp = d.maxYAmplitude == d.minYAmplitude
                    ? d.maxYAmplitude
                    : Mathf.Lerp(d.minYAmplitude, d.maxYAmplitude, (float)d.rngDict[segmentIndex].NextDouble());

                float delay = d.maxWait == d.minWait
                    ? d.maxWait
                    : Mathf.Lerp(d.minWait, d.maxWait, (float)d.rngDict[segmentIndex].NextDouble());
                delay *= delayMult;
                delay = Mathf.Clamp(delay, d.delayDict[segmentIndex], remaining);
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
            d.rngDict = new Dictionary<int, System.Random>(context.SegmentData.Length);
            for (int i = 0; i < context.SegmentData.Length; i++)
            {
                d.rngDict.Add(i, new System.Random(seed + i));
            }
        }

        private void InitLastUpdatedDict(IAnimationContext context)
        {
            Data d = context.CustomData as Data;
            d.lastUpdatedDict = new Dictionary<int, float>(context.SegmentData.Length);
            for (int i = 0; i < context.SegmentData.Length; i++)
            {
                d.lastUpdatedDict.Add(i, context.AnimatorContext.PassedTime);
            }
        }

        private void InitDelayDict(IAnimationContext context)
        {
            Data d = context.CustomData as Data;
            d.delayDict = new Dictionary<int, float>(context.SegmentData.Length);

            for (int i = 0; i < context.SegmentData.Length; i++)
            {
                d.delayDict.Add(i, 0);
            }
        }

        private void InitOffsetDict(IAnimationContext context)
        {
            Data d = context.CustomData as Data;
            d.offsetDict = new Dictionary<int, Vector2>(context.SegmentData.Length);

            for (int i = 0; i < context.SegmentData.Length; i++)
            {
                d.offsetDict.Add(i, Vector2.zero);
            }
        }

        [AutoParametersStorage]
        private partial class Data
        {
            public bool init;
            
            public Dictionary<int, Vector2> offsetDict = null;
            public Dictionary<int, float> lastUpdatedDict = null;
            public Dictionary<int, float> delayDict = null;
            public Dictionary<int, System.Random> rngDict = null;
        }
    }
}