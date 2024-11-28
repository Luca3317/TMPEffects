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

        public float GetOffset(CharData cData, ITMPSegmentData segmentData, IAnimatorDataProvider animatorData,
            bool ignoreAnimatorScaling = false)
        {
            if (!useCustom)
            {
                return AnimationUtility.GetOffset(EnumValue, cData, segmentData, animatorData, ignoreAnimatorScaling);
            }

            if (Value == null) return 0;

            return Value.GetOffset(cData, segmentData, animatorData, ignoreAnimatorScaling);
        }

        public void GetMinMaxOffset(out float min, out float max, ITMPSegmentData segmentData,
            IAnimatorDataProvider animatorData, bool ignoreAnimatorScaling = false)
        {
            if (!useCustom)
            {
                AnimationUtility.GetMinMaxOffset(out min, out max, EnumValue, segmentData, animatorData, ignoreAnimatorScaling);
                return;
            }

            if (Value == null)
            {
                min = 0;
                max = 0;
                return;
            }

            Value.GetMinMaxOffset(out min, out max, segmentData, animatorData, ignoreAnimatorScaling);
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