using System.Collections.Generic;
using TMPEffects.Components;
using Unity.VisualScripting;

public class TMPSceneAnimTest : TMPSceneAnimation
{
    public string DefaultString = "The default string";
    public string currentString = "";

    public override void Animate(ref CharData charData, IAnimationContext context)
    {
    }

    public override IAnimationContext GetNewContext()
    {
        return new DefaultSceneAnimationContext();
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
