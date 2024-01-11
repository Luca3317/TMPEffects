using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Tags;
using TMPEffects.Components;
using TMPEffects.TextProcessing;

namespace TMPEffects.Commands
{
    [CreateAssetMenu(fileName = "new WaitCommand", menuName = "TMPEffects/Commands/Wait")]
    public class WaitCommand : TMPCommand
    {
        public override CommandType CommandType => CommandType.Index;
        public override bool ExecuteInstantly => false;
        public override bool ExecuteOnSkip => false;

        public override void ExecuteCommand(TMPCommandArgs args)
        {
            ParsingUtility.StringToFloat(args.tag.parameters[""], out var value);
            args.writer.Wait(value);
        }

        public override bool ValidateParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null) return false;
            if (!parameters.ContainsKey(""))
                return false;

            return ParsingUtility.StringToFloat(parameters[""], out _);
        }
    }
}
