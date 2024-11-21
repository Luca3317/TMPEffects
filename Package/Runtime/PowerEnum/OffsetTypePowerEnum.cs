using System;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.Parameters;
using TMPEffects.ParameterUtilityGenerator.Attributes;
using UnityEngine;
using System.Collections.Generic;
using TMPEffects.Databases;

namespace TMPEffects.TMPAnimations
{
    // TODO I dont really know whether ignoresegmentlength works at all
    // What was the case i wanted it for again? related to genericanimation
    // TODO Scene offsetbundle
    [Serializable]
    [TMPParameterBundle("OffsetBundle")]
    public partial class OffsetBundle
    {
        [TMPParameterBundleField("offset", "off")]
        private ITMPOffsetProvider _provider;

        [SerializeField] private OffsetTypePowerEnum offsetProvider;

        [TMPParameterBundleField("uniformity", "uni")] [SerializeField]
        private float uniformity = 1;

        [SerializeField] [TMPParameterBundleField("ignoreanimatorscaling", "ignorescaling", "ignorescl", "ignscl")]
        private bool ignoreAnimatorScaling = false;

        [SerializeField]
        [TMPParameterBundleField("ignoresegmentlength", "ignoreseglen", "ignorelen", "ignseglen", "ignlen")]
        private bool ignoreSegmentLength = true;

        
        // TODO
        // For both getuniformity methods:
        // Am still not sure whether segemntlenght considered in
        // offset or uniformity. Therefore for now leave context as part
        // of signature to make changes later easier
        public float GetUniformity(IAnimatorContext context)
        {
            return uniformity;
        }
        
        public float GetUniformity(IAnimationContext context)
        {
            var uni = uniformity;
            return uni;
        }
        
        public float GetOffset(CharData cData, IAnimationContext context)
        {
            float offset = _provider.GetOffset(cData, context, ignoreAnimatorScaling);

            if (!ignoreSegmentLength)
                offset /= context.SegmentData.length == 0 ? 0.001f : context.SegmentData.length;

            return offset;
        }

        public float GetOffset(CharData cData, IAnimatorContext context)
        {
            float offset = _provider?.GetOffset(cData, context, ignoreAnimatorScaling) ?? 0;

            if (!ignoreSegmentLength)
            {
                var segmentLength = context.Animator.TextComponent.text.Length;
                offset /= segmentLength == 0 ? 0.001f : segmentLength;
            }

            return offset;
        }

        private static void Create_Hook(ref OffsetBundle newInstance, OffsetBundle originalInstance,
            OffsetBundleParameters parameters)
        {
            newInstance._provider = parameters._provider ?? originalInstance.offsetProvider;
        }
    }

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

        public float GetOffset(CharData cData, IAnimationContext context)
        {
            float offset = _provider.GetOffset(cData, context, ignoreAnimatorScaling);

            if (!ignoreSegmentLength)
                offset /= context.SegmentData.length == 0 ? 0.001f : context.SegmentData.length;

            return offset * uniformity;
        }

        public float GetOffset(CharData cData, IAnimatorContext context)
        {
            float offset = _provider.GetOffset(cData, context, ignoreAnimatorScaling);

            if (!ignoreSegmentLength)
            {
                var segmentLength = context.Animator.TextComponent.text.Length;
                offset /= segmentLength == 0 ? 0.001f : segmentLength;
            }

            return offset * uniformity;
        }

        private static void Create_Hook(ref SceneOffsetBundle newInstance, SceneOffsetBundle originalInstance,
            SceneOffsetBundleParameters parameters)
        {
            newInstance._provider = parameters._provider ?? originalInstance.offsetProvider;
        }
    }


    [Serializable]
    public class SceneOffsetTypePowerEnum : PowerEnum<ParameterTypes.OffsetType, TMPSceneOffsetProvider>,
        IEquatable<SceneOffsetTypePowerEnum>, ITMPOffsetProvider
    {
        public SceneOffsetTypePowerEnum() : base(ParameterTypes.OffsetType.Index)
        {
        }

        public SceneOffsetTypePowerEnum(ParameterTypes.OffsetType offsetType) : base(offsetType)
        {
        }

        public SceneOffsetTypePowerEnum(ParameterTypes.OffsetType offsetType,
            TMPSceneOffsetProvider customOffsetProvider)
            : base(offsetType, customOffsetProvider)
        {
        }

        public SceneOffsetTypePowerEnum(ParameterTypes.OffsetType offsetType,
            TMPSceneOffsetProvider customOffsetProvider, bool useCustom)
            : base(offsetType, customOffsetProvider, useCustom)
        {
        }

        public float GetOffset(CharData cData, IAnimationContext context, bool ignoreAnimatorScaling = false)
        {
            if (!useCustom)
                return AnimationUtility.GetOffset(cData, context, EnumValue, ignoreScaling: ignoreAnimatorScaling);

            if (Value == null) return 0;

            return Value.GetOffset(cData, context);
        }

        public float GetOffset(CharData cData, IAnimatorContext context, bool ignoreAnimatorScaling = false)
        {
            if (!useCustom)
                return AnimationUtility.GetOffset(cData, context, EnumValue, ignoreScaling: ignoreAnimatorScaling);

            if (Value == null) return 0;

            return Value.GetOffset(cData, context);
        }

        public bool Equals(SceneOffsetTypePowerEnum other)
        {
            return other != null &&
                   other.EnumValue == EnumValue &&
                   other.UseCustom == UseCustom &&
                   other.Value == Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is SceneOffsetTypePowerEnum e) return Equals(e);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Serializable]
    public class OffsetTypePowerEnum : PowerEnum<ParameterTypes.OffsetType, TMPOffsetProvider>,
        IEquatable<OffsetTypePowerEnum>, ITMPOffsetProvider
    {
        public OffsetTypePowerEnum() : base(ParameterTypes.OffsetType.Index)
        {
        }

        public OffsetTypePowerEnum(ParameterTypes.OffsetType offsetType) : base(offsetType)
        {
        }

        public OffsetTypePowerEnum(ParameterTypes.OffsetType offsetType,
            TMPOffsetProvider customOffsetProvider)
            : base(offsetType, customOffsetProvider)
        {
        }

        public OffsetTypePowerEnum(ParameterTypes.OffsetType offsetType,
            TMPOffsetProvider customOffsetProvider, bool useCustom)
            : base(offsetType, customOffsetProvider, useCustom)
        {
        }

        public float GetOffset(CharData cData, IAnimationContext context, bool ignoreAnimatorScaling = false)
        {
            if (!useCustom)
            {
                return AnimationUtility.GetOffset(cData, context, EnumValue, ignoreScaling: ignoreAnimatorScaling);
            }
            
            if (Value == null) return 0;

            return Value.GetOffset(cData, context);
        }

        public float GetOffset(CharData cData, IAnimatorContext context, bool ignoreAnimatorScaling = false)
        {
            if (!useCustom)
                return AnimationUtility.GetOffset(cData, context, EnumValue, ignoreScaling: ignoreAnimatorScaling);

            if (Value == null) return 0;

            return Value.GetOffset(cData, context);
        }

        public bool Equals(OffsetTypePowerEnum other)
        {
            return other != null &&
                   other.EnumValue == EnumValue &&
                   other.UseCustom == UseCustom &&
                   other.Value == Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is OffsetTypePowerEnum e) return Equals(e);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}