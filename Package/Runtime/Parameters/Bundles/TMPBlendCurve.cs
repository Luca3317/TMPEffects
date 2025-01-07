using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.Parameters.Attributes;
using TMPEffects.TMPAnimations;
using UnityEngine;

namespace TMPEffects.Parameters
{
    /// <summary>
    /// Blend curve with built-in offsets.<br/>
    /// This uses a different, internal way to calculate offsets (different from e.g. <see cref="OffsetBundle"/>)
    /// because it was created to be used for TMPMeshModifier clips in the timeline, which requires the offset to be
    /// adjustable in certain ways, in some cases.
    /// </summary>
    [System.Serializable]
    [TMPParameterBundle("BlendCurve")]
    public partial class TMPBlendCurve
    {
        /// <summary>
        /// The curve used for evaluation.
        /// </summary>
        [TMPParameterBundleField("curve", "crv")]
        public AnimationCurve curve;

        /// <summary>
        /// The <see cref="ITMPOffsetProvider"/> used to calculate offsets.
        /// </summary>
        public ITMPOffsetProvider provider
        {
            get => _provider ?? offsetProvider;
            set => _provider = value;
        }

        [TMPParameterBundleField("offset", "off")]
        private ITMPOffsetProvider _provider;

        [SerializeField] private OffsetTypePowerEnum offsetProvider = new OffsetTypePowerEnum();

        /// <summary>
        /// The uniformity that should be applied to the offset.
        /// </summary>
        [TMPParameterBundleField("uniformity", "uni")]
        public float uniformity = 0;

        /// <summary>
        /// Whether to ignore animator scaling (for relevant offset types).
        /// </summary>
        [TMPParameterBundleField("ignorescaling", "ignorescl", "ignscl", "ignscaling")]
        public bool ignoreAnimatorScaling = false;

        /// <summary>
        /// Whether to finish blending the entire segment in the specified duration, or the first character.
        /// </summary>
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

        /// <summary>
        /// Evaluate as an "in" blending curve.
        /// </summary>
        /// <param name="timeValue"></param>
        /// <param name="totalDuration"></param>
        /// <param name="minOffset"></param>
        /// <param name="maxOffset"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public float EvaluateIn(float timeValue, float totalDuration, float minOffset, float maxOffset,
            float offset)
        {
            offset = offset - minOffset;
            maxOffset = maxOffset - minOffset;
            if (uniformity < 0)
                offset = maxOffset - offset;

            if (finishWholeSegmentInTime)
            {
                float scalar = ((maxOffset * Mathf.Abs(uniformity)) / totalDuration) + 1f;
                scalar = 1f / scalar;

                timeValue -= offset * scalar * Mathf.Abs(uniformity);

                return curve.Evaluate(timeValue / totalDuration / scalar);
            }

            timeValue -= offset * Mathf.Abs(uniformity);
            return curve.Evaluate(timeValue / totalDuration);
        }

        /// <summary>
        /// Evaluate as an "in" blending curve.
        /// </summary>
        /// <param name="timeValue"></param>
        /// <param name="totalDuration"></param>
        /// <param name="cData"></param>
        /// <param name="animatorData"></param>
        /// <param name="segmentData"></param>
        /// <returns></returns>
        public float EvaluateIn(float timeValue, float totalDuration, CharData cData,
            IAnimatorDataProvider animatorData, ITMPSegmentData segmentData)
        {
            float offset = provider.GetOffset(cData, segmentData, animatorData, ignoreAnimatorScaling);
            float min, max;
            provider.GetMinMaxOffset(out min, out max, segmentData, animatorData);
            return EvaluateIn(timeValue, totalDuration, min, max, offset);
        }

        /// <summary>
        /// Evaluate as an "in" blending curve.
        /// </summary>
        /// <param name="timeValue"></param>
        /// <param name="duration"></param>
        /// <param name="cData"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public float EvaluateIn(float timeValue, float duration, CharData cData, IAnimationContext context) =>
            EvaluateIn(timeValue, duration, cData, context.AnimatorContext, context.SegmentData);

        /// <summary>
        /// Evaluate as an "out" blending curve.
        /// </summary>
        /// <param name="timeValue"></param>
        /// <param name="totalDuration"></param>
        /// <param name="minOffset"></param>
        /// <param name="maxOffset"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public float EvaluateOut(float timeValue, float totalDuration, float preTime, float minOffset, float maxOffset, float offset)
        {
            offset = offset - minOffset;
            maxOffset = maxOffset - minOffset;
            if (uniformity < 0)
                offset = maxOffset - offset;

            if (finishWholeSegmentInTime)
            {
                float scalar =
                    ((maxOffset * Mathf.Abs(uniformity)) / totalDuration) + 1f;
                scalar = 1f / scalar;

                timeValue -= offset * scalar * Mathf.Abs(uniformity);

                return curve.Evaluate(1f - ((timeValue - preTime) / totalDuration / scalar));
            }

            timeValue -= offset * Mathf.Abs(uniformity);
            return curve.Evaluate(1f - ((timeValue - preTime) / totalDuration));
        }
        
        /// <summary>
        /// Evaluate as an "out" blending curve.
        /// </summary>
        /// <param name="timeValue"></param>
        /// <param name="totalDuration"></param>
        /// <param name="cData"></param>
        /// <param name="animatorData"></param>
        /// <param name="segmentData"></param>
        /// <returns></returns>
        public float EvaluateOut(float timeValue, float totalDuration, float preTime, CharData cData,
            IAnimatorDataProvider animatorData, ITMPSegmentData segmentData)
        {
            float offset = provider.GetOffset(cData, segmentData, animatorData, ignoreAnimatorScaling);
            float min, max;
            provider.GetMinMaxOffset(out min, out max, segmentData, animatorData);
            return EvaluateOut(timeValue, totalDuration, preTime, min, max, offset);
        }

        /// <summary>
        /// Evaluate as an "out" blending curve.
        /// </summary>
        /// <param name="timeValue"></param>
        /// <param name="duration"></param>
        /// <param name="cData"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public float EvaluateOut(float timeValue, float duration, float preTime, CharData cData, IAnimationContext context) =>
            EvaluateOut(timeValue, duration, preTime, cData, context.AnimatorContext, context.SegmentData);
    }
}