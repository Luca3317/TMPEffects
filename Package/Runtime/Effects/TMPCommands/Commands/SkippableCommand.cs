using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Parameters;

namespace TMPEffects.TMPCommands.Commands
{
    [CreateAssetMenu(fileName ="new SkippableCommand", menuName = "TMPEffects/Commands/Built-in/Skippable")]
    public class SkippableCommand : TMPCommand
    {
        public override TagType TagType => TagType.Index;

        public override bool ExecuteInstantly => false;

        public override bool ExecuteOnSkip => true;

        public override bool ExecuteRepeatable => true;

#if UNITY_EDITOR
        public override bool ExecuteInPreview => true;
#endif

        public override void ExecuteCommand(TMPCommandArgs args)
        {
            bool val;
            ParameterParsing.StringToBool(args.tag.Parameters[""], out val);
            args.writer.SetSkippable(val);
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return false;
            return ParameterParsing.StringToBool(parameters[""], out _);
        }
    }
}
