using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Parameters;
using TMPEffects.TMPAnimations;
using UnityEngine;

public class ExampleOffset : ParameterTypes.TMPOffsetProvider
{
    public override float GetOffset(CharData cData, IAnimationContext context)
    {
        if (cData.info.character == 'A') return 0.5f;
        return 0f;
    }
}