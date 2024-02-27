using System.Collections;
using System.Collections.Generic;
using TMPEffects;
using TMPEffects.Components;
using TMPEffects.TMPAnimations;
using UnityEngine;
using TMPEffects.Components.CharacterData;

public class TMPSceneShowANimaTest : TMPSceneShowAnimation
{
    public override void Animate(ref CharData charData, IAnimationContext context)
    {
        charData.SetVisibilityState(VisibilityState.Shown, context.animatorContext.passedTime); 
    }

    public override IAnimationContext GetNewContext()
    {
        return new DefaultSceneAnimationContext();
    }

    public override void ResetParameters()
    {
    }

    public override void SetParameters(IDictionary<string, string> parameters)
    {
    }

    public override bool ValidateParameters(IDictionary<string, string> parameters)
    {
        return true;
    }
}
