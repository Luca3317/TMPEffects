using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using UnityEngine;
using static TMPEffects.Parameters.ParameterUtility;

namespace TMPEffects.TMPAnimations.Animations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new ShakeAnimation",
        menuName = "TMPEffects/Animations/Basic Animations/Built-in/Shake")]
    public partial class ShakeAnimation : TMPAnimation
    {
        [SerializeField, AutoParameter("uniform", "uni")]
        [Tooltip("Whether to apply the shake uniformly across the text.\nAliases: uniform, uni")]
        bool uniform = false;

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

        [SerializeField, AutoParameter("uniformwait", "uniwait", "uniw")]
        [Tooltip(
            "Whether to use uniform wait time across the text. Ignored if uniform is true.\nAliases: uniformwait, uniwait, uniw")]
        bool uniformWait = true;

        [SerializeField, AutoParameter("minwait", "minw")]
        [Tooltip("The minimum amount of time to wait after each shake.\nAliases: minwait, minw")] 
        float minWait = 0.1f;

        [SerializeField, AutoParameter("maxwait", "maxw")]
        [Tooltip("The maximum amount of time to wait after each shake.\nAliases: maxwait, maxw")] 
        float maxWait = 0.1f;
        
        private partial void Animate(CharData cData, Data data, IAnimationContext context)
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

                    if (d.uniformWait) InitAutoUpdateDict(context);
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
                    float xAmp = d.maxXAmplitude == d.minXAmplitude
                        ? d.maxXAmplitude
                        : Mathf.Lerp(d.minXAmplitude, d.maxXAmplitude, (float)d.rng.NextDouble());
                    float yAmp = d.maxYAmplitude == d.minYAmplitude
                        ? d.maxYAmplitude
                        : Mathf.Lerp(d.minYAmplitude, d.maxYAmplitude, (float)d.rng.NextDouble());

                    d.delay = d.maxWait == d.minWait
                        ? d.maxWait
                        : Mathf.Lerp(d.minWait, d.maxWait, (float)d.rng.NextDouble());
                    d.lastUpdated = context.AnimatorContext.PassedTime;

                    d.xOffset = ((float)d.rng.NextDouble() * 2f - 1f) * xAmp;
                    d.yOffset = ((float)d.rng.NextDouble() * 2f - 1f) * yAmp;
                }

                offset = new Vector3(d.xOffset, d.yOffset, 0f);
            }

            // else if the shake uses uniform delay (all character shake at the same time)
            else if (d.uniformWait)
            {
                int segmentIndex = context.SegmentData.SegmentIndexOf(cData);

                // if the delay time is exceeded, shake the character by calculating new x/yoffset, x/yamplitude for each character a new delay
                if (d.autoUpdateDict[segmentIndex] ||
                    context.AnimatorContext.PassedTime - d.sharedLastUpdated >= d.sharedDelay)
                {
                    float xAmp = d.maxXAmplitude == d.minXAmplitude
                        ? d.maxXAmplitude
                        : Mathf.Lerp(d.minXAmplitude, d.maxXAmplitude, (float)d.rngDict[segmentIndex].NextDouble());
                    float yAmp = d.maxYAmplitude == d.minYAmplitude
                        ? d.maxYAmplitude
                        : Mathf.Lerp(d.minYAmplitude, d.maxYAmplitude, (float)d.rngDict[segmentIndex].NextDouble());

                    if (d.autoUpdateDict[segmentIndex]) d.autoUpdateDict[segmentIndex] = false;
                    else
                    {
                        d.sharedDelay = d.maxWait == d.minWait
                            ? d.maxWait
                            : Mathf.Lerp(d.minWait, d.maxWait, (float)d.rngDict[segmentIndex].NextDouble());
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
                    float xAmp = d.maxXAmplitude == d.minXAmplitude
                        ? d.maxXAmplitude
                        : Mathf.Lerp(d.minXAmplitude, d.maxXAmplitude, (float)d.rngDict[segmentIndex].NextDouble());
                    float yAmp = d.maxYAmplitude == d.minYAmplitude
                        ? d.maxYAmplitude
                        : Mathf.Lerp(d.minYAmplitude, d.maxYAmplitude, (float)d.rngDict[segmentIndex].NextDouble());

                    d.delayDict[segmentIndex] = d.maxWait == d.minWait
                        ? d.maxWait
                        : Mathf.Lerp(d.minWait, d.maxWait, (float)d.rngDict[segmentIndex].NextDouble());
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
        
        [AutoParametersStorage]
        private partial class Data
        {
            public bool init = false;

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