using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Parameters;
using TMPEffects.TMPAnimations;
using UnityEngine;

public class ItmpOffsetProviderScene : MonoBehaviour, ParameterTypes.ITMPOffsetProvider
{
    public float GetOffset(CharData cData, IAnimationContext context)
    {
        return 12;
    }
}
