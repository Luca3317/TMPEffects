using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TMPCommand : ScriptableObject
{
    public abstract void ExecuteCommand(TMPCommandArgs args);
    public abstract bool ValidateParameters(Dictionary<string, string> parameters);
    public abstract void SetParameters(Dictionary<string, string> parameters);
    public abstract void ResetVariables();
}