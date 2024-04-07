using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Tags;
using TMPEffects.Components;
using TMPEffects.TextProcessing;

namespace TMPEffects.TMPCommands.Commands
{
    [CreateAssetMenu(fileName = "new WaitCommand", menuName = "TMPEffects/Commands/Wait")]
    public class WaitCommand : TMPCommand
    {
        public override TagType TagType => TagType.Index;
        public override bool ExecuteInstantly => false;
        public override bool ExecuteOnSkip => false;
        public override bool ExecuteRepeatable => true;

#if UNITY_EDITOR
        public override bool ExecuteInPreview => true;
#endif

        public override void ExecuteCommand(TMPCommandArgs args)
        {
            ParsingUtility.StringToFloat(args.tag.Parameters[""], out var value);
            args.writer.Wait(value);
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return false;
            if (!parameters.ContainsKey(""))
                return false;

            return ParsingUtility.StringToFloat(parameters[""], out _);
        }
    }
}
