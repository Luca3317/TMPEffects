using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Parameters;
using TMPEffects.TMPAnimations;
using UnityEngine;

[Serializable]
public class OffsetTypePowerEnum : PowerEnum<ParameterTypes.WaveOffsetType, TMPOffsetType>
{
    public float GetOffset(CharData cData, IAnimationContext context)
    {
        if (!useCustom) return AnimationUtility.GetWaveOffset(cData, context, EnumValue);
        
        if (Value == null) return 0;
        return Value.GetOffset(cData, context);
    }
}