using System.Collections.Generic;
using UnityEngine;

public abstract class TMPAnimation : ScriptableObject, ITMPAnimation
{
    public abstract void Animate(ref CharData charData, AnimationContext context);
    public abstract void SetParameters(Dictionary<string, string> parameters);
    public abstract bool ValidateParameters(Dictionary<string, string> parameters);
    public abstract void ResetVariables();

    public abstract void SetParameter<T>(string name, T value);
}

public abstract class TMPAnimationParameterless : TMPAnimation
{
    public override void SetParameters(Dictionary<string, string> parameters) { }
    public override bool ValidateParameters(Dictionary<string, string> parameters) => true;
    public override void SetParameter<T>(string name, T value) { }
}
