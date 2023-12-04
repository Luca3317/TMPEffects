using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="new TestCommand", menuName ="TMPEffects/Commands/Test")]
[TMPEffect("testCommand")]
public class TestCommand : TMPCommand
{
    public override void ExecuteCommand(TMPCommandArgs args, TMPWriter writer)
    {
        writer.Wait(float.Parse(args.parameters[""]));
        Debug.Log("Executed test command");
    }

    public override void ResetVariables()
    { }

    public override void SetParameters(Dictionary<string, string> parameters)
    { }

    public override bool ValidateParameters(Dictionary<string, string> parameters)
    {
        if (parameters == null) return false;
        if (!parameters.ContainsKey(""))
            return false;

        return float.TryParse(parameters[""], out _);
    }
}
