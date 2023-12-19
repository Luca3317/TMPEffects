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

        public override void ExecuteCommand(TMPCommandTag args, TMPWriter writer)
        {
            ParsingUtility.StringToFloat(args.parameters[""], out var value);
            writer.Wait(value);

            writer.WaitUntil(() => true);
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
