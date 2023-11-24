using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[CreateAssetMenu(fileName = "new SpeedCommand", menuName = "TMPEffects/Commands/Speed")]
[TMPEffect("speed")]
public class SpeedCommand : TMPCommand
{
    public override void ExecuteCommand(TMPCommandArgs args, TMPWriterFinal writer)
    {
        writer.SetSpeed(float.Parse(args.parameters[""], CultureInfo.InvariantCulture));
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
        
        return float.TryParse(parameters[""], NumberStyles.Float, CultureInfo.InvariantCulture, out _);
    }
}
