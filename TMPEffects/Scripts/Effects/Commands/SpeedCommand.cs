using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[CreateAssetMenu(fileName = "new SpeedCommand", menuName = "TMPEffects/Commands/Speed")]
public class SpeedCommand : TMPCommand
{
    public override CommandType CommandType => CommandType.Index;
    public override bool ExecuteInstantly => false;

    public override void ExecuteCommand(TMPCommandTag args, TMPWriter writer)
    {
        writer.SetSpeed(float.Parse(args.parameters[""], CultureInfo.InvariantCulture));
    }

    public override bool ValidateParameters(Dictionary<string, string> parameters)
    {
        if (parameters == null) return false;
        if (!parameters.ContainsKey(""))
            return false;
        
        return float.TryParse(parameters[""], NumberStyles.Float, CultureInfo.InvariantCulture, out _);
    }
}
