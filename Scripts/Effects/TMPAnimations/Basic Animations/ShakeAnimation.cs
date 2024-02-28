using System.Collections.Generic;
using TMPEffects.TextProcessing;
using UnityEngine;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new ShakeAnimation", menuName = "TMPEffects/Animations/Shake")]
    public class ShakeAnimation : TMPAnimation
    {
        [SerializeField] bool uniform;
        [SerializeField] float xAmplitude;
        [SerializeField] float yAmplitude;
        [SerializeField] float delay;

        [System.NonSerialized] bool currentUniform;
        [System.NonSerialized] float currentXAmplitude;
        [System.NonSerialized] float currentYAmplitude;
        [System.NonSerialized] float currentDelay;

        ShakeAnimationContext shakeContext;
        System.Random random;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            shakeContext = (ShakeAnimationContext)context;

            float xOffset;
            float yOffset;
            Vector3 offset;
            int seed;

            if (context.animatorContext.PassedTime - shakeContext.lastUpdated < currentDelay)
            {
                seed = shakeContext.lastSeed;
            }
            else
            {
                seed = (int)(shakeContext.animatorContext.PassedTime * 1000);
                shakeContext.lastUpdated = shakeContext.animatorContext.PassedTime;
                shakeContext.lastSeed = seed;
            }

            // TODO Probably implement some basic prng to avoid all these allocations
            random = new System.Random(seed + cData.segmentIndex);

            if (currentUniform)
            {
                if (cData.segmentIndex == 0)
                {
                    shakeContext.RandomX = currentXAmplitude == 0f ? 0 : ((float)random.NextDouble() * 2f - 1f) * currentXAmplitude;
                    shakeContext.RandomY = currentYAmplitude == 0f ? 0 : ((float)random.NextDouble() * 2f - 1f) * currentYAmplitude;
                }

                offset = new Vector2(shakeContext.RandomX, shakeContext.RandomY);
            }
            else
            {
                xOffset = 0;
                if (currentXAmplitude != 0)
                {
                    xOffset = ((float)random.NextDouble() * 2f - 1f) * currentXAmplitude;
                }
                yOffset = 0;
                if (currentYAmplitude != 0)
                {
                    yOffset = ((float)random.NextDouble() * 2f - 1f) * currentYAmplitude;
                }

                offset = new Vector3(xOffset, yOffset, 0);
            }

            cData.SetPosition(cData.info.initialPosition + offset);
        }

        public override void ResetParameters()
        {
            currentUniform = uniform;
            currentXAmplitude = xAmplitude;
            currentYAmplitude = yAmplitude;
            currentDelay = delay;
        }

        public override void SetParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            foreach (KeyValuePair<string, string> kvp in parameters)
            {
                switch (kvp.Key.ToLower())
                {
                    case "u":
                    case "uniform":
                        ParsingUtility.StringToBool(kvp.Value, out currentUniform);
                        break;

                    case "x":
                    case "xa":
                    case "xamp":
                    case "xamplitude":
                        ParsingUtility.StringToFloat(kvp.Value, out currentXAmplitude);
                        break;

                    case "y":
                    case "ya":
                    case "yamp":
                    case "yamplitude":
                        ParsingUtility.StringToFloat(kvp.Value, out currentYAmplitude);
                        break;

                    case "d":
                    case "dl":
                    case "delay":
                        ParsingUtility.StringToFloat(kvp.Value, out currentDelay);
                        break;

                    case "a":
                    case "amp":
                    case "amplitude":
                        ParsingUtility.StringToFloat(kvp.Value, out currentXAmplitude);
                        currentYAmplitude = currentXAmplitude;
                        break;
                }
            }
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            foreach (KeyValuePair<string, string> kvp in parameters)
            {
                switch (kvp.Key.ToLower())
                {
                    case "u":
                    case "uniform":
                        if (!ParsingUtility.StringToBool(kvp.Value, out _)) return false;
                        break;

                    case "x":
                    case "xa":
                    case "xamp":
                    case "xamplitude":
                    case "y":
                    case "ya":
                    case "yamp":
                    case "yamplitude":
                    case "a":
                    case "amp":
                    case "amplitude":
                    case "d":
                    case "dl":
                    case "delay":
                        if (!ParsingUtility.StringToFloat(kvp.Value, out _)) return false;
                        break;
                }
            }

            return true;
        }

        public override IAnimationContext GetNewContext()
        {
            return new ShakeAnimationContext();
        }

        private class ShakeAnimationContext : IAnimationContext
        {
            public SegmentData segmentData { get; set; }

            public ReadOnlyAnimatorContext animatorContext { get => settings; set => settings = value; }
            private ReadOnlyAnimatorContext settings;

            public float RandomX;
            public float RandomY;

            public float lastUpdated;
            public int lastSeed;
        }
    }
}
