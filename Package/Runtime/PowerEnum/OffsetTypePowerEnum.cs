using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Parameters;
using TMPEffects.TMPAnimations;
using UnityEngine;


[Serializable]
public class SceneOffsetTypePowerEnum : PowerEnum<ParameterTypes.OffsetType, ParameterTypes.TMPSceneOffsetProvider>,
    IEquatable<SceneOffsetTypePowerEnum>, ParameterTypes.ITMPOffsetProvider
{
    public SceneOffsetTypePowerEnum()
    {
    }

    public SceneOffsetTypePowerEnum(ParameterTypes.OffsetType offsetType, ParameterTypes.TMPSceneOffsetProvider customOffsetProvider)
        : base(offsetType, customOffsetProvider)
    {
    }

    public SceneOffsetTypePowerEnum(ParameterTypes.OffsetType offsetType, ParameterTypes.TMPSceneOffsetProvider customOffsetProvider, bool useCustom)
        : base(offsetType, customOffsetProvider, useCustom)
    {
    }

    public float GetOffset(CharData cData, IAnimationContext context)
    {
        if (!useCustom) return AnimationUtility.GetOffset(cData, context, EnumValue);

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
    public OffsetTypePowerEnum() : base()
    {
    }
    
    
    public OffsetTypePowerEnum(ParameterTypes.OffsetType enumValue) : base(enumValue)
    {
    }

    public OffsetTypePowerEnum(ParameterTypes.OffsetType offsetType, ParameterTypes.TMPOffsetProvider customOffsetProvider)
        : base(offsetType, customOffsetProvider)
    {
    }

    public OffsetTypePowerEnum(ParameterTypes.OffsetType offsetType, ParameterTypes.TMPOffsetProvider customOffsetProvider, bool useCustom)
        : base(offsetType, customOffsetProvider, useCustom)
    {
    }

    public float GetOffset(CharData cData, IAnimationContext context)
    {
        if (!useCustom) return AnimationUtility.GetOffset(cData, context, EnumValue);

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

    public static implicit operator ParameterTypes.OffsetType(OffsetTypePowerEnum value)
    {
        return value.EnumValue;
    }

    public static implicit operator OffsetTypePowerEnum(ParameterTypes.OffsetType value)
    {
        return new OffsetTypePowerEnum(value);
    }
}