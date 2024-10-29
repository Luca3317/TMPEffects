using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.TMPAnimations;
using UnityEngine;


public interface ITMPOffsetType
{
    public float GetOffset(CharData cData, IAnimationContext context);
}

public abstract class TMPOffsetType : ScriptableObject, ITMPOffsetType
{
    // TODO What other params might make sense?
    public abstract float GetOffset(CharData cData, IAnimationContext context);
}

 