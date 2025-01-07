using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using UnityEngine;

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
            // Initialize if not yet initialized
            if (!data.init)
            {
                data.init = true;

                if (!data.uniform)
                {
                    InitRNGDict(context);
                    InitLastUpdatedDict(context);
                    InitDelayDict(context);
                    InitOffsetDict(context);

                    if (data.uniformWait) InitAutoUpdateDict(context);
                }
                else
                {
                    data.rng = new System.Random((int)(Time.time * 1000));
                }
            }

            Vector3 offset;

            // If the shake is uniform (all characters shake the same way, at the same time)
            if (data.uniform)
            {
                // if the delay time is exceeded, shake the character by calculating new x/yoffset, x/yamplitude and delay
                if (context.AnimatorContext.PassedTime - data.lastUpdated >= data.delay)
                {
                    float xAmp = data.maxXAmplitude == data.minXAmplitude
                        ? data.maxXAmplitude
                        : Mathf.Lerp(data.minXAmplitude, data.maxXAmplitude, (float)data.rng.NextDouble());
                    float yAmp = data.maxYAmplitude == data.minYAmplitude
                        ? data.maxYAmplitude
                        : Mathf.Lerp(data.minYAmplitude, data.maxYAmplitude, (float)data.rng.NextDouble());

                    data.delay = data.maxWait == data.minWait
                        ? data.maxWait
                        : Mathf.Lerp(data.minWait, data.maxWait, (float)data.rng.NextDouble());
                    data.lastUpdated = context.AnimatorContext.PassedTime;

                    data.xOffset = (((float)data.rng.NextDouble() * 2f) - 1f) * xAmp;
                    data.yOffset = (((float)data.rng.NextDouble() * 2f) - 1f) * yAmp;
                }

                offset = new Vector3(data.xOffset, data.yOffset, 0f);
            }

            // else if the shake uses uniform delay (all character shake at the same time)
            else if (data.uniformWait)
            {
                int segmentIndex = context.SegmentData.SegmentIndexOf(cData);

                // if the delay time is exceeded, shake the character by calculating new x/yoffset, x/yamplitude for each character a new delay
                if (data.autoUpdateDict[segmentIndex] ||
                    context.AnimatorContext.PassedTime - data.sharedLastUpdated >= data.sharedDelay)
                {
                    float xAmp = data.maxXAmplitude == data.minXAmplitude
                        ? data.maxXAmplitude
                        : Mathf.Lerp(data.minXAmplitude, data.maxXAmplitude, (float)data.rngDict[segmentIndex].NextDouble());
                    float yAmp = data.maxYAmplitude == data.minYAmplitude
                        ? data.maxYAmplitude
                        : Mathf.Lerp(data.minYAmplitude, data.maxYAmplitude, (float)data.rngDict[segmentIndex].NextDouble());

                    if (data.autoUpdateDict[segmentIndex]) data.autoUpdateDict[segmentIndex] = false;
                    else
                    {
                        data.sharedDelay = data.maxWait == data.minWait
                            ? data.maxWait
                            : Mathf.Lerp(data.minWait, data.maxWait, (float)data.rngDict[segmentIndex].NextDouble());
                        data.sharedLastUpdated = context.AnimatorContext.PassedTime;

                        for (int i = 0; i < context.SegmentData.Length; i++)
                        {
                            if (i == segmentIndex) continue;
                            data.autoUpdateDict[i] = true;
                        }
                    }

                    float xOffset = (((float)data.rngDict[segmentIndex].NextDouble() * 2f) - 1f) * xAmp;
                    float yOffset = (((float)data.rngDict[segmentIndex].NextDouble() * 2f) - 1f) * yAmp;
                    data.offsetDict[segmentIndex] = new Vector3(xOffset, yOffset, 0f);
                }

                offset = data.offsetDict[segmentIndex];
            }

            // else if the characters shake completely independently
            else 
            {
                int segmentIndex = context.SegmentData.SegmentIndexOf(cData);

                // if the delay time of the current character is exceeded, shake the character by calculating new x/yoffset, x/yamplitude and delays for each character
                if (context.AnimatorContext.PassedTime - data.lastUpdatedDict[segmentIndex] >= data.delayDict[segmentIndex])
                {
                    float xAmp = data.maxXAmplitude == data.minXAmplitude
                        ? data.maxXAmplitude
                        : Mathf.Lerp(data.minXAmplitude, data.maxXAmplitude, (float)data.rngDict[segmentIndex].NextDouble());
                    float yAmp = data.maxYAmplitude == data.minYAmplitude
                        ? data.maxYAmplitude
                        : Mathf.Lerp(data.minYAmplitude, data.maxYAmplitude, (float)data.rngDict[segmentIndex].NextDouble());

                    data.delayDict[segmentIndex] = data.maxWait == data.minWait
                        ? data.maxWait
                        : Mathf.Lerp(data.minWait, data.maxWait, (float)data.rngDict[segmentIndex].NextDouble());
                    data.lastUpdatedDict[segmentIndex] = context.AnimatorContext.PassedTime;

                    float xOffset = (((float)data.rngDict[segmentIndex].NextDouble() * 2f) - 1f) * xAmp;
                    float yOffset = (((float)data.rngDict[segmentIndex].NextDouble() * 2f) - 1f) * yAmp;
                    data.offsetDict[segmentIndex] = new Vector3(xOffset, yOffset, 0f);
                }

                offset = data.offsetDict[segmentIndex];
            }

            // Set the position of the character using the calculated offset
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

        private void InitAutoUpdateDict(IAnimationContext context)
        {
            Data d = context.CustomData as Data;
            d.autoUpdateDict = new Dictionary<int, bool>(context.SegmentData.Length);

            for (int i = 0; i < context.SegmentData.Length; i++)
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