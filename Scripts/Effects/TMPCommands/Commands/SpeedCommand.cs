using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using TMPEffects.Tags;
using TMPEffects.Components;
using TMPEffects.TextProcessing;

namespace TMPEffects.TMPCommands.Commands
{
    [CreateAssetMenu(fileName = "new SpeedCommand", menuName = "TMPEffects/Commands/Speed")]
    public class SpeedCommand : TMPCommand
    {
        public override TagType TagType => TagType.Either;
        public override bool ExecuteInstantly => false;
        public override bool ExecuteOnSkip => false;
        public override bool ExecuteRepeatable => true;

#if UNITY_EDITOR
        public override bool ExecuteInPreview => true;
#endif

        public override void ExecuteCommand(TMPCommandArgs args)
        {
            ParsingUtility.StringToFloat(args.tag.Parameters[""], out float speed);
            args.writer.SetSpeed(speed);
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

