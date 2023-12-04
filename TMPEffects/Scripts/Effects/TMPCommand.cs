using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TMPCommand : ScriptableObject
{
    // TODO ExecuteCommand needs a context variable;
    // Can pass in both writer and animator, but would prefer a single dedicated context object later
    public abstract void ExecuteCommand(TMPCommandArgs args, TMPWriter writer/*, TMPAnimator animator*/);
    public abstract bool ValidateParameters(Dictionary<string, string> parameters);
    public abstract void SetParameters(Dictionary<string, string> parameters);
    public abstract void ResetVariables();
}