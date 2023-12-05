using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[CreateAssetMenu(fileName = "new ShowCommand", menuName = "TMPEffects/Commands/Show")]
public class ShowCommand : TMPCommand
{
    public override CommandType CommandType => CommandType.Range;
    public override bool ExecuteInstantly => true;

    public override void ExecuteCommand(TMPCommandTag tag, TMPWriter writer)
    {
        if (tag.IsOpen) Debug.LogError("Show tag was not closed!");
        writer.Show(tag.startIndex, tag.length, true);
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
