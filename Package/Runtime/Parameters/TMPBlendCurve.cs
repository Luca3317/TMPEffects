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
        public bool finishWholeSegmentInTime;

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
        }

        public float EvaluateIn(float timeValue, float totalDuration, float minOffset, float maxOffset,
            float offset)
        {
            offset = offset - minOffset;
            maxOffset = maxOffset - minOffset;
            if (uniformity < 0)
                offset = maxOffset - offset;

            if (finishWholeSegmentInTime)
            {
                float scalar = (maxOffset * Mathf.Abs(uniformity)) / totalDuration + 1f;
                scalar = 1f / scalar;

                timeValue -= offset * scalar * Mathf.Abs(uniformity);

                return curve.Evaluate(timeValue / totalDuration / scalar);
            }

            timeValue -= offset * Mathf.Abs(uniformity);
            return curve.Evaluate(timeValue / totalDuration);
        }

        public float EvaluateIn(float timeValue, float totalDuration, CharData cData,
            IAnimatorDataProvider animatorData, ITMPSegmentData segmentData)
        {
            float offset = provider.GetOffset(cData, segmentData, animatorData, ignoreAnimatorScaling);
            float min, max;
            provider.GetMinMaxOffset(out min, out max, segmentData, animatorData);
            return EvaluateIn(timeValue, totalDuration, min, max, offset);
        }

        public float EvaluateIn(float timeValue, float duration, CharData cData, IAnimationContext context) =>
            EvaluateIn(timeValue, duration, cData, context.AnimatorContext, context.SegmentData);

        public float EvaluateOut(float timeValue, float totalDuration, float preTime, float minOffset, float maxOffset, float offset)
        {
            offset = offset - minOffset;
            maxOffset = maxOffset - minOffset;
            if (uniformity < 0)
                offset = maxOffset - offset;

            if (finishWholeSegmentInTime)
            {
                float scalar =
                    (maxOffset * Mathf.Abs(uniformity)) / totalDuration + 1f;
                scalar = 1f / scalar;

                timeValue -= offset * scalar * Mathf.Abs(uniformity);

                return curve.Evaluate(1f - (timeValue - preTime) / totalDuration / scalar);
            }

            timeValue -= offset * Mathf.Abs(uniformity);
            return curve.Evaluate(1f - (timeValue - preTime) / totalDuration);
        }
        
        public float EvaluateOut(float timeValue, float totalDuration, float preTime, CharData cData,
            IAnimatorDataProvider animatorData, ITMPSegmentData segmentData)
        {
            float offset = provider.GetOffset(cData, segmentData, animatorData, ignoreAnimatorScaling);
            float min, max;
            provider.GetMinMaxOffset(out min, out max, segmentData, animatorData);
            return EvaluateOut(timeValue, totalDuration, preTime, min, max, offset);
        }

        public float EvaluateOut(float timeValue, float duration, float preTime, CharData cData, IAnimationContext context) =>
            EvaluateOut(timeValue, duration, preTime, cData, context.AnimatorContext, context.SegmentData);
    }
}