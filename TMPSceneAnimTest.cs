using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects;
using TMPEffects.TMPAnimations;
using TMPEffects.Components.CharacterData;

public class TMPSceneAnimTest : TMPSceneAnimation
{
    public string DefaultString = "The default string";
    public string currentString = "";

    public override void Animate(CharData charData, IAnimationContext context)
    {
    }

    public override object GetNewCustomData()
    {
        return null;
    }

    public override bool ValidateParameters(IDictionary<string, string> parameters)
    {
        if (parameters == null) return false;
        if (parameters.ContainsKey("str")) return true;
        return false;
    }


    public override void ResetParameters()
    {
        currentString = DefaultString;
    }

    public override void SetParameters(IDictionary<string, string> parameters)
    {
        currentString = parameters["str"];
    }
}
