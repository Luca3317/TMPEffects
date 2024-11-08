using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.Parameters;
using UnityEngine;

namespace TMPEffects.TMPAnimations
{
    [Serializable]
    public class SceneOffsetTypePowerEnum : PowerEnum<ParameterTypes.OffsetType, ParameterTypes.TMPSceneOffsetProvider>,
        IEquatable<SceneOffsetTypePowerEnum>, ParameterTypes.ITMPOffsetProvider
    {
        // TODO maybe make base class / alternative class that doesnt do this; more basic version essentially
        [SerializeField] bool ignoreAnimatorScaling;
        [SerializeField] bool considerSegmentLength;

        public SceneOffsetTypePowerEnum()
        {
        }

        public SceneOffsetTypePowerEnum(ParameterTypes.OffsetType offsetType,
            ParameterTypes.TMPSceneOffsetProvider customOffsetProvider)
            : base(offsetType, customOffsetProvider)
        {
        }

        public SceneOffsetTypePowerEnum(ParameterTypes.OffsetType offsetType,
            ParameterTypes.TMPSceneOffsetProvider customOffsetProvider, bool useCustom)
            : base(offsetType, customOffsetProvider, useCustom)
        {
        }

        public float GetOffset(CharData cData, IAnimationContext context)
        {
            if (!useCustom)
                return AnimationUtility.GetOffset(cData, context, EnumValue, ignoreScaling: ignoreAnimatorScaling);

            if (Value == null) return 0;

            if (considerSegmentLength)
                return Value.GetOffset(cData, context) / context.SegmentData.length;

            return Value.GetOffset(cData, context);
        }

        public float GetOffset(CharData cData, IAnimatorContext context, int segmentLength = 1)
        {
            if (!useCustom)
                return AnimationUtility.GetOffset(cData, context, EnumValue, ignoreScaling: ignoreAnimatorScaling);

            if (Value == null) return 0;

            if (considerSegmentLength)
                return Value.GetOffset(cData, context) / segmentLength == 0 ? 0.001f : segmentLength;

            return Value.GetOffset(cData, context);
        }

        public float GetOffset(CharData cData, IAnimatorContext context)
        {
            return GetOffset(cData, context, 1);
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
            if (obj is OffsetTypePowerEnum e) return Equals(e);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Serializable]
    public class OffsetTypePowerEnum : PowerEnum<ParameterTypes.OffsetType, ParameterTypes.TMPOffsetProvider>,
        IEquatable<OffsetTypePowerEnum>, ParameterTypes.ITMPOffsetProvider
    {
        // TODO maybe make base class / alternative class that doesnt do this; more basic version essentially
        [SerializeField] bool ignoreAnimatorScaling;
        [SerializeField] bool considerSegmentLength;
        [SerializeField] float uniformity;

        public OffsetTypePowerEnum() : base()
        {
        }

        public OffsetTypePowerEnum(ParameterTypes.OffsetType enumValue) : base(enumValue)
        {
        }

        public OffsetTypePowerEnum(ParameterTypes.OffsetType offsetType,
            ParameterTypes.TMPOffsetProvider customOffsetProvider)
            : base(offsetType, customOffsetProvider)
        {
        }

        public OffsetTypePowerEnum(ParameterTypes.OffsetType offsetType,
            ParameterTypes.TMPOffsetProvider customOffsetProvider, bool useCustom)
            : base(offsetType, customOffsetProvider, useCustom)
        {
        }

        public float GetOffset(CharData cData, IAnimationContext context)
        {
            float offset;
            if (!useCustom)
            {
                offset = AnimationUtility.GetOffset(cData, context, EnumValue, ignoreScaling: ignoreAnimatorScaling);
            }
            else
            {
                if (Value == null) return 0;
                offset = Value.GetOffset(cData, context);
            }

            if (considerSegmentLength)
                offset /= context.SegmentData.length == 0 ? 0.001f : context.SegmentData.length;

            return offset * uniformity;
        }

        public float GetOffset(CharData cData, IAnimatorContext context, int segmentLength)
        {
            float offset;
            if (!useCustom)
            {
                offset = AnimationUtility.GetOffset(cData, context, EnumValue, cData.info.index,
                    ignoreScaling: ignoreAnimatorScaling);
            }
            else
            {
                if (Value == null) return 0;
                offset = Value.GetOffset(cData, context);
            }

            if (considerSegmentLength)
            {
                // if (cData.info.index == context.Animator.TextComponent.GetParsedText().Length - 1)
                // Debug.LogWarning("For character " + cData.info.index + ": " + offset + " / " + segmentLength + " => " +
                //                  (offset / segmentLength));
                offset /= segmentLength == 0 ? 0.001f : segmentLength;
            }

            return offset * uniformity;
        }

        public float GetOffset(CharData cData, IAnimatorContext context)
        {
            // TODO Uses the text length as segmentLength
            // Might be fine, but dont love.
            return GetOffset(cData, context, context.Animator.TextComponent.GetParsedText().Length);
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

        public static implicit operator ParameterTypes.OffsetType(OffsetTypePowerEnum value)
        {
            return value.EnumValue;
        }

        public static implicit operator OffsetTypePowerEnum(ParameterTypes.OffsetType value)
        {
            return new OffsetTypePowerEnum(value);
        }
    }
}