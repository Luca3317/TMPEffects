using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Parameters;
using TMPEffects.TMPAnimations;
using UnityEngine;

[CreateAssetMenu(fileName = "examplescript", menuName = "sief")]
public class ItmpOffsetProviderScriptable : ScriptableObject, ParameterTypes.ITMPOffsetProvider
{
    public float GetOffset(CharData cData, IAnimationContext context)
    {
        return 0;
    }
}
