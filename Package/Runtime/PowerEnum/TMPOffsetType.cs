using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.TMPAnimations;
using UnityEngine;

public abstract class TMPOffsetType : MonoBehaviour
{
    // TODO What other params might make sense?
    public abstract float GetOffset(CharData cData, IAnimationContext context);
}

 