using System;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using TMPEffects.Components.Animator;
using TMPEffects.ParameterUtilityGenerator.Attributes;
using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Serialization;

namespace TMPEffects.Parameters
{
    [Serializable]
    [TMPParameterBundle("OffsetBundle")]
    public partial class OffsetBundle
    {
        public ITMPOffsetProvider provider
        {
            get => _provider ?? offsetProvider;
            set => _provider = value;
        }
        
        [TMPParameterBundleField("offset", "off")]
        private ITMPOffsetProvider _provider;

        // HideInInspector so it doesnt show "Add from TMPOffsetProvider" in context menu of timeline
        [SerializeField, HideInInspector] private OffsetTypePowerEnum offsetProvider = new OffsetTypePowerEnum();

        [TMPParameterBundleField("uniformity", "uni")] [SerializeField]
        public float uniformity = 1;

        [SerializeField] [TMPParameterBundleField("ignoreanimatorscaling", "ignorescaling", "ignorescl", "ignscl")]
        public bool ignoreAnimatorScaling = false;

        [SerializeField] [TMPParameterBundleField("zerooffset", "zerooff", "zoff", "zoffset", "ignlen")]
        public bool zeroBasedOffset = false;

        public OffsetBundle(ITMPOffsetProvider provider)
        {
            _provider = provider;
        }

        public OffsetBundle()
        {
            _provider = offsetProvider;
        }
        
        public float GetOffset(CharData cData, IAnimationContext context)
        {
            var segmentData = AnimationUtility.GetMockedSegment(
                context.AnimatorContext.Animator.TextComponent.GetParsedText().Length,
                context.AnimatorContext.Animator.CharData);
            float offset = _provider.GetOffset(cData, segmentData, context.AnimatorContext, ignoreAnimatorScaling);

            if (zeroBasedOffset)
            {
                _provider.GetMinMaxOffset(out var min, out var max, segmentData, context.AnimatorContext);
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

            if (zeroBasedOffset)
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

        // private static void Create_Hook(ref OffsetBundle newInstance, OffsetBundle originalInstance,
        //     OffsetBundleParameters parameters)
        // {
        //     newInstance._provider = parameters._provider ?? originalInstance.offsetProvider;
        // }
    }
}