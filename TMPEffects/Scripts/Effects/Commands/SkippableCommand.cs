using System.Collections;
using System.Collections.Generic;
using TMPEffects;
using TMPEffects.TextProcessing;
using UnityEngine;

namespace TMPEffects.Commands
{
    [CreateAssetMenu(fileName ="new SkippableCommand", menuName ="TMPEffects/Commands/Skippable")]
    public class SkippableCommand : TMPCommand
    {
        public override CommandType CommandType => CommandType.Both;

        public override bool ExecuteInstantly => false;

        public override bool ExecuteOnSkip => true;

        public override void ExecuteCommand(TMPCommandArgs args)
        {
            bool val;
            ParsingUtility.StringToBool(args.tag.parameters[""], out val);
            args.writer.SetSkippable(val);
        }

        public override bool ValidateParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null) return false;
            return ParsingUtility.StringToBool(parameters[""], out _);
        }
    }
}
