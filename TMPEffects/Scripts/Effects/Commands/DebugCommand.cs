using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPEffects.Tags;
using TMPEffects.Components;

namespace TMPEffects.Commands
{
    [CreateAssetMenu(fileName = "new DebugCommand", menuName = "TMPEffects/Commands/Debug")]
    public class DebugCommand : TMPCommand
    {
        public override CommandType CommandType => CommandType.Index;
        public override bool ExecuteInstantly => false;
        public override bool ExecuteOnSkip => true;

        public override void ExecuteCommand(TMPCommandArgs args)
        {
            if (args.tag.parameters != null)
            {
                if (args.tag.parameters.ContainsKey("type"))
                {
                    switch (args.tag.parameters["type"])
                    {
                        case "w":
                        case "warning": Debug.LogWarning(args.tag.parameters[""]); break;
                        case "e":
                        case "error": Debug.LogError(args.tag.parameters[""]); break;
                        case "l":
                        case "log":
                        default: Debug.Log(args.tag.parameters[""]); break;
                    }
                }
                else Debug.Log(args.tag.parameters[""]);
            }
        }

        public override bool ValidateParameters(Dictionary<string, string> parameters)
        {
            return true;
        }
    }
}
