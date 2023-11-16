using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="new TestCommand", menuName ="TMPEffects/Commands/Test")]
[TMPEffect("testCommand")]
public class TestCommand : TMPCommand
{
    public override void ExecuteCommand(TMPCommandArgs args)
    {
        Debug.Log("Executed test command");
    }

    public override void ResetVariables()
    {

    }

    public override void SetParameters(Dictionary<string, string> parameters)
    {
        
    }

    public override bool ValidateParameters(Dictionary<string, string> parameters)
    {
        return true;
    }
}
