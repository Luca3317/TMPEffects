using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPEffects.Tags;
using TMPEffects.Components;

namespace TMPEffects.Commands
{
    [CreateAssetMenu(fileName = "new ShowCommand", menuName = "TMPEffects/Commands/Show")]
    public class ShowCommand : TMPCommand
    {
        public override CommandType CommandType => CommandType.Range;
        public override bool ExecuteInstantly => true;
        public override bool ExecuteOnSkip => false;

        public override void ExecuteCommand(TMPCommandArgs args)
        {
            if (args.tag.IsOpen) Debug.LogError("Show tag was not closed!");
            args.writer.Show(args.tag.startIndex, args.tag.length, true);
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
}
