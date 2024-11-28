using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.ParameterUtilityGenerator.Attributes;
using TMPEffects.TMPAnimations;
using UnityEngine;

namespace TMPEffects.Parameters
{
    [System.Serializable]
    [TMPParameterBundle("BlendCurve")]
    public partial class TMPBlendCurve
    {
        [TMPParameterBundleField("curve", "crv")]
        public AnimationCurve curve;

        public ITMPOffsetProvider provider
        {
            get => _provider ?? offsetProvider;
            set => _provider = value;
        }

        [TMPParameterBundleField("offset", "off")]
        private ITMPOffsetProvider _provider;

        [SerializeField] private OffsetTypePowerEnum offsetProvider = new OffsetTypePowerEnum();

        [TMPParameterBundleField("uniformity", "uni")]
        public float uniformity = 1;

        [TMPParameterBundleField("ignorescaling", "ignorescl", "ignscl", "ignscaling")]
        public bool ignoreAnimatorScaling = false;

        [TMPParameterBundleField("ignoresegmentlength", "ignoresegmentlen", "ignoreseglen", "ignseglen",
            "ignsegmentlength", "ignsegmentlen")]
        public bool finishWholeSegmentInTime; // TODO Wtf to call this; what is the core of the concept

        [TMPParameterBundleField("")]
        public bool zeroBasedOffset;

        public TMPBlendCurve()
        {
        }

        public TMPBlendCurve(TMPBlendCurve crv)
        {
            this.curve = crv.curve;
            this.uniformity = crv.uniformity;
            this._provider = crv._provider;
            this.offsetProvider = crv.offsetProvider;
            this.ignoreAnimatorScaling = crv.ignoreAnimatorScaling;
            this.finishWholeSegmentInTime = crv.finishWholeSegmentInTime;
            this.zeroBasedOffset = crv.zeroBasedOffset;
        }

        public float EvaluateIn(CharData cData, ITMPSegmentData segmentData, IAnimatorDataProvider animatorData, float timeValue, float totalDuration)
        {
            float offset = provider.GetOffset(cData, segmentData, animatorData, ignoreAnimatorScaling);

            // TODO
            // if (zeroBasedOffsets)
            // {
            provider.GetMinMaxOffset(out var min, out var max, segmentData, animatorData);
            float zeroedOffset = offset - min;
            float zeroedMax = max - min;
            if (uniformity >= 0)
            {
                offset = zeroedOffset;
            }
            else
            {
                offset = zeroedMax - zeroedOffset;
            }
            // }

            if (finishWholeSegmentInTime)
            {
                float scalar = (zeroedMax * Mathf.Abs(uniformity)) / totalDuration + 1f;
                scalar = 1f / scalar;

                timeValue -= offset * scalar * Mathf.Abs(uniformity);

                return curve.Evaluate(timeValue / totalDuration / scalar);
            }

            timeValue -= offset * Mathf.Abs(uniformity);
            return curve.Evaluate(timeValue / totalDuration);
        }
        
        public float EvaluateIn(CharData cData, IAnimatorDataProvider context, float timeValue, float totalDuration)
        {
            var segmentData = AnimationUtility.GetMockedSegment(context.Animator.TextComponent.GetParsedText().Length,
                context.Animator.CharData);
            return EvaluateIn(cData, segmentData, context, timeValue, totalDuration);
        }

        public float EvaluateOut(CharData cData, ITMPSegmentData segmentData, IAnimatorDataProvider animatorData, float timeValue, float totalDuration,
            float preTime)
        {
            float offset = provider.GetOffset(cData, segmentData, animatorData, ignoreAnimatorScaling);

            // if (zeroBasedOffsets)
            // {
            provider.GetMinMaxOffset(out var min, out var max, segmentData, animatorData);
            float zeroedOffset = offset - min;
            float zeroedMax = max - min;
            if (uniformity >= 0)
            {
                offset = zeroedOffset;
            }
            else
            {
                offset = zeroedMax - zeroedOffset;
            }
            // }

            if (finishWholeSegmentInTime)
            {
                float scalar = (zeroedMax * Mathf.Abs(uniformity)) / totalDuration + 1f;
                scalar = 1f / scalar;

                timeValue -= offset * scalar * Mathf.Abs(uniformity);

                return curve.Evaluate(1f - (timeValue - preTime) / totalDuration / scalar);
            }

            timeValue -= offset * Mathf.Abs(uniformity);
            return curve.Evaluate(1f - (timeValue - preTime) / totalDuration);
        }
        
        public float EvaluateOut(CharData cData, IAnimatorContext context, float timeValue, float totalDuration,
            float preTime)
        {
            var segmentData = AnimationUtility.GetMockedSegment(context.Animator.TextComponent.GetParsedText().Length,
                context.Animator.CharData);
            return EvaluateOut(cData, segmentData, context, timeValue, totalDuration, preTime);
        }
    }
}