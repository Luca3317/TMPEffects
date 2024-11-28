using System;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.Parameters;

namespace TMPEffects.TMPAnimations
{
    // Offset provider wrappers, used in database etc
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
        
        public float GetOffset(CharData cData, ITMPSegmentData segmentData, IAnimatorDataProvider animatorData,
            bool ignoreAnimatorScaling = false)
        {
            if (!useCustom)
                return AnimationUtility.GetOffset(EnumValue, cData, segmentData, animatorData, ignoreAnimatorScaling);

            if (Value == null) return 0;

            return Value.GetOffset(cData, segmentData, animatorData, ignoreAnimatorScaling);;
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
}