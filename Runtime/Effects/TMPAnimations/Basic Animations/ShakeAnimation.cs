using System.Collections.Generic;
using TMPEffects.CharacterData;
using UnityEngine;
using static TMPEffects.ParameterUtility;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new ShakeAnimation", menuName = "TMPEffects/Animations/Shake")]
    public class ShakeAnimation : TMPAnimation
    {
        [Tooltip("Whether to apply the shake uniformly across the text.\nAliases: uniform, uni")]
        [SerializeField] bool uniform = false;

        [Tooltip("The maximum X amplitude of each shake.\nAliases: maxxamplitude, maxxamp, maxxa, maxx")]
        [SerializeField] float maxXAmplitude = 5;
        [Tooltip("The minimum X amplitude of each shake.\nAliases: minxamplitude, minxamp, minxa, minx")]
        [SerializeField] float minXAmplitude = 5;
        [Tooltip("The maximum Y amplitude of each shake.\nAliases: maxyamplitude, maxyamp, maxya, maxy")]
        [SerializeField] float maxYAmplitude = 5;
        [Tooltip("The minimum Y amplitude of each shake.\nAliases: minyamplitude, minyamp, minya, miny")]
        [SerializeField] float minYAmplitude = 5;

        [Tooltip("Whether to use uniform wait time across the text. Ignored if uniform is true.\nAliases: uniformdelay, unidelay, unid")]
        [SerializeField] bool uniformWait = true;
        [Tooltip("The minimum amount of time to wait after each shake.\nAliases: minwait, minw")]
        [SerializeField] float minWait = 0.1f;
        [Tooltip("The maximum amount of time to wait after each shake.\nAliases: maxwait, maxw")]
        [SerializeField] float maxWait = 0.1f;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.CustomData as Data;

            // Initialize if not yet initialized
            if (!d.init)
            {
                d.init = true;

                if (!d.uniform)
                {
                    InitRNGDict(context);
                    InitLastUpdatedDict(context);
                    InitDelayDict(context);
                    InitOffsetDict(context);

                    if (d.uniformDelay) InitAutoUpdateDict(context);
                }
                else
                {
                    d.rng = new System.Random((int)(Time.time * 1000));
                }
            }

            Vector3 offset;

            // If the shake is uniform (all characters shake the same way, at the same time)
            if (d.uniform)
            {
                // if the delay time is exceeded, shake the character by calculating new x/yoffset, x/yamplitude and delay
                if (context.AnimatorContext.PassedTime - d.lastUpdated >= d.delay)
                {
                    float xAmp = d.maxXAmplitude == d.minXAmplitude ? d.maxXAmplitude : Mathf.Lerp(d.minXAmplitude, d.maxXAmplitude, (float)d.rng.NextDouble());
                    float yAmp = d.maxYAmplitude == d.minYAmplitude ? d.maxYAmplitude : Mathf.Lerp(d.minYAmplitude, d.maxYAmplitude, (float)d.rng.NextDouble());

                    d.delay = d.maxDelay == d.minDelay ? d.maxDelay : Mathf.Lerp(d.minDelay, d.maxDelay, (float)d.rng.NextDouble());
                    d.lastUpdated = context.AnimatorContext.PassedTime;

                    d.xOffset = ((float)d.rng.NextDouble() * 2f - 1f) * xAmp;
                    d.yOffset = ((float)d.rng.NextDouble() * 2f - 1f) * yAmp;
                }

                offset = new Vector3(d.xOffset, d.yOffset, 0f);
            }

            // else if the shake uses uniform delay (all character shake at the same time)
            else if (d.uniformDelay)
            {
                int segmentIndex = context.SegmentData.SegmentIndexOf(cData);

                // if the delay time is exceeded, shake the character by calculating new x/yoffset, x/yamplitude for each character a new delay
                if (d.autoUpdateDict[segmentIndex] || context.AnimatorContext.PassedTime - d.sharedLastUpdated >= d.sharedDelay)
                {
                    float xAmp = d.maxXAmplitude == d.minXAmplitude ? d.maxXAmplitude : Mathf.Lerp(d.minXAmplitude, d.maxXAmplitude, (float)d.rngDict[segmentIndex].NextDouble());
                    float yAmp = d.maxYAmplitude == d.minYAmplitude ? d.maxYAmplitude : Mathf.Lerp(d.minYAmplitude, d.maxYAmplitude, (float)d.rngDict[segmentIndex].NextDouble());

                    if (d.autoUpdateDict[segmentIndex]) d.autoUpdateDict[segmentIndex] = false;
                    else
                    {
                        d.sharedDelay = d.maxDelay == d.minDelay ? d.maxDelay : Mathf.Lerp(d.minDelay, d.maxDelay, (float)d.rngDict[segmentIndex].NextDouble());
                        d.sharedLastUpdated = context.AnimatorContext.PassedTime;

                        for (int i = 0; i < context.SegmentData.length; i++)
                        {
                            if (i == segmentIndex) continue;
                            d.autoUpdateDict[i] = true;
                        }
                    }

                    float xOffset = ((float)d.rngDict[segmentIndex].NextDouble() * 2f - 1f) * xAmp;
                    float yOffset = ((float)d.rngDict[segmentIndex].NextDouble() * 2f - 1f) * yAmp;
                    d.offsetDict[segmentIndex] = new Vector3(xOffset, yOffset, 0f);
                }

                offset = d.offsetDict[segmentIndex];
            }

            // else if the characters shake completely independently
            else
            {
                int segmentIndex = context.SegmentData.SegmentIndexOf(cData);

                // if the delay time of the current character is exceeded, shake the character by calculating new x/yoffset, x/yamplitude and delays for each character
                if (context.AnimatorContext.PassedTime - d.lastUpdatedDict[segmentIndex] >= d.delayDict[segmentIndex])
                {
                    float xAmp = d.maxXAmplitude == d.minXAmplitude ? d.maxXAmplitude : Mathf.Lerp(d.minXAmplitude, d.maxXAmplitude, (float)d.rngDict[segmentIndex].NextDouble());
                    float yAmp = d.maxYAmplitude == d.minYAmplitude ? d.maxYAmplitude : Mathf.Lerp(d.minYAmplitude, d.maxYAmplitude, (float)d.rngDict[segmentIndex].NextDouble());

                    d.delayDict[segmentIndex] = d.maxDelay == d.minDelay ? d.maxDelay : Mathf.Lerp(d.minDelay, d.maxDelay, (float)d.rngDict[segmentIndex].NextDouble());
                    d.lastUpdatedDict[segmentIndex] = context.AnimatorContext.PassedTime;

                    float xOffset = ((float)d.rngDict[segmentIndex].NextDouble() * 2f - 1f) * xAmp;
                    float yOffset = ((float)d.rngDict[segmentIndex].NextDouble() * 2f - 1f) * yAmp;
                    d.offsetDict[segmentIndex] = new Vector3(xOffset, yOffset, 0f);
                }

                offset = d.offsetDict[segmentIndex];
            }

            // Set the position of the character using the calculated offset
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

        private void InitAutoUpdateDict(IAnimationContext context)
        {
            Data d = context.CustomData as Data;
            d.autoUpdateDict = new Dictionary<int, bool>(context.SegmentData.length);

            for (int i = 0; i < context.SegmentData.length; i++)
            {
                d.autoUpdateDict.Add(i, false);
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float f, parameters, "maxxamplitude", "maxxamp", "maxxa", "maxx")) d.maxXAmplitude = f;
            if (TryGetFloatParameter(out f, parameters, "maxyamplitude", "maxyamp", "maxya", "maxy")) d.maxYAmplitude = f;
            if (TryGetFloatParameter(out f, parameters, "minxamplitude", "minxamp", "minxa", "minx")) d.minXAmplitude = f;
            if (TryGetFloatParameter(out f, parameters, "minyamplitude", "minyamp", "minya", "miny")) d.minYAmplitude = f;
            if (TryGetBoolParameter(out bool b, parameters, "uniform", "uni")) d.uniform = b;
            if (TryGetBoolParameter(out b, parameters, "uniformwait", "uniwait", "uniw")) d.uniformDelay = b;
            if (TryGetFloatParameter(out f, parameters, "minwait", "minw")) d.minDelay = f;
            if (TryGetFloatParameter(out f, parameters, "maxwait", "maxw")) d.maxDelay = f;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "maxxamplitude", "maxxamp", "maxxa", "maxx")) return false;
            if (HasNonFloatParameter(parameters, "maxyamplitude", "maxyamp", "maxya", "maxy")) return false;
            if (HasNonFloatParameter(parameters, "minxamplitude", "minxamp", "minxa", "minx")) return false;
            if (HasNonFloatParameter(parameters, "minyamplitude", "minyamp", "minya", "miny")) return false;
            if (HasNonBoolParameter(parameters, "uniform", "uni")) return false;
            if (HasNonBoolParameter(parameters, "uniformwait", "uniwait", "uniw")) return false;
            if (HasNonFloatParameter(parameters, "minwait", "minw")) return false;
            if (HasNonFloatParameter(parameters, "maxwait", "maxw")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                uniform = this.uniform,
                maxXAmplitude = this.maxXAmplitude,
                minXAmplitude = this.minXAmplitude,
                minYAmplitude = this.minYAmplitude,
                maxYAmplitude = this.maxYAmplitude,
                uniformDelay = this.uniformWait,
                minDelay = this.minWait,
                maxDelay = this.maxWait,
            };
        }

        private class Data
        {
            public bool init = false;

            public bool uniform;
            public float maxXAmplitude;
            public float minXAmplitude;
            public float maxYAmplitude;
            public float minYAmplitude;
            public bool uniformDelay;
            public float minDelay;
            public float maxDelay;

            // uniform
            public System.Random rng = null;
            public float yOffset = 0;
            public float xOffset = 0;
            public float lastUpdated = 0;
            public float delay = 0;

            // uniform, non uniform delay
            public Dictionary<int, bool> autoUpdateDict = null;
            public int updatingIndex = -1;
            public float sharedDelay = 0;
            public float sharedLastUpdated = 0;

            // non uniform
            public Dictionary<int, Vector2> offsetDict = null;
            public Dictionary<int, float> lastUpdatedDict = null;
            public Dictionary<int, float> delayDict = null;
            public Dictionary<int, System.Random> rngDict = null;
        }
    }
}