using System;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using TMPEffects.Components.Animator;
using TMPEffects.ParameterUtilityGenerator.Attributes;
using TMPEffects.TMPAnimations;
using UnityEngine;

namespace TMPEffects.Parameters
{
    // TODO Update to be in sync w / offsetbundle
    // Scene bundle for AutoParameters to easily get offsets
    [Serializable]
    [TMPParameterBundle("SceneOffsetBundle")]
    public partial class SceneOffsetBundle
    {
        [TMPParameterBundleField("offset", "off")]
        private ITMPOffsetProvider _provider;

        [SerializeField] private SceneOffsetTypePowerEnum offsetProvider;

        [TMPParameterBundleField("uniformity", "uni")] [SerializeField]
        private float uniformity;

        [SerializeField] [TMPParameterBundleField("ignoreanimatorscaling", "ignorescaling", "ignorescl", "ignscl")]
        private bool ignoreAnimatorScaling;

        [SerializeField]
        [TMPParameterBundleField("ignoresegmentlength", "ignoreseglen", "ignorelen", "ignseglen", "ignlen")]
        private bool ignoreSegmentLength;

        [SerializeField] [TMPParameterBundleField("zerooffset", "zerooff", "zoff", "zoffset", "ignlen")]
        private bool zeroBaseOffset = true;

        public float GetOffset(CharData cData, IAnimationContext context)
        {
            float offset = _provider.GetOffset(cData, context.SegmentData, context.AnimatorContext,
                ignoreAnimatorScaling);

            if (zeroBaseOffset)
            {
                _provider.GetMinMaxOffset(out var min, out var max, context.SegmentData, context.AnimatorContext);
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
            }

            return offset * uniformity;
        }

        public float GetOffset(CharData cData, IAnimatorContext context)
        {
            var segmentData = AnimationUtility.GetMockedSegment(context.Animator.TextComponent.GetParsedText().Length,
                context.Animator.CharData);
            float offset = _provider.GetOffset(cData, segmentData, context, ignoreAnimatorScaling);

            if (zeroBaseOffset)
            {
                _provider.GetMinMaxOffset(out var min, out var max, segmentData, context);
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
            }

            return offset * uniformity;
        }

        private static void Create_Hook(ref SceneOffsetBundle newInstance, SceneOffsetBundle originalInstance,
            SceneOffsetBundleParameters parameters)
        {
            newInstance._provider = parameters._provider ?? originalInstance.offsetProvider;
        }
    }
}