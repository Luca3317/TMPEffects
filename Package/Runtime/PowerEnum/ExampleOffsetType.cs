using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.TMPAnimations;
using UnityEngine;

public class ExampleOffsetType : TMPOffsetType
{
    public override float GetOffset(CharData cData, IAnimationContext context)
    {
        if (cData.info.character == 'A') return 0.5f;
        return 0f;
    }
}