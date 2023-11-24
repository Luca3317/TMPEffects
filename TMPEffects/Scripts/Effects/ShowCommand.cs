using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[CreateAssetMenu(fileName = "new ShowCommand", menuName = "TMPEffects/Commands/Show")]
[TMPEffect("show")]
public class ShowCommand : TMPCommand
{
    public override void ExecuteCommand(TMPCommandArgs args, TMPWriterFinal writer)
    {
        Debug.Log("Executed!");
    }

    public override void ResetVariables()
    {
    }

    public override void SetParameters(Dictionary<string, string> parameters)
    {
    }

    public override bool ValidateParameters(Dictionary<string, string> parameters)
    {
        if (parameters == null) return true;
        if (parameters.ContainsKey(""))
        {
            if (!float.TryParse(parameters[""], NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                return false;
        }
        return true;
    }
}
