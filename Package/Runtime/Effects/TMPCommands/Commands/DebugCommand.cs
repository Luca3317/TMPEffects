using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.TMPCommands.Commands
{
    [CreateAssetMenu(fileName = "new DebugCommand", menuName = "TMPEffects/Commands/Debug")]
    public class DebugCommand : TMPCommand
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
            if (args.tag.Parameters != null)
            {
                if (args.tag.Parameters.ContainsKey("type"))
                {
                    switch (args.tag.Parameters["type"])
                    {
                        case "w":
                        case "warning": Debug.LogWarning(args.tag.Parameters[""]); break;
                        case "e":
                        case "error": Debug.LogError(args.tag.Parameters[""]); break;
                        case "l":
                        case "log":
                        default: Debug.Log(args.tag.Parameters[""]); break;
                    }
                }
                else
                {
                    Debug.Log(args.tag.Parameters[""]);
                }
            }
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            return true;
        }
    }
}
