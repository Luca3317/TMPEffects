using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.Components.Writer;
using UnityEngine;
using TMPEffects.Databases;

namespace TMPEffects.TMPCommands.Commands
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new DebugCommand", menuName = "TMPEffects/Commands/Built-in/Debug")]
    public partial class DebugCommand : TMPCommand
    {
        public override TagType TagType => TagType.Index;
        public override bool ExecuteInstantly => false;
        public override bool ExecuteOnSkip => true;
        public override bool ExecuteRepeatable => true;
#if UNITY_EDITOR
        public override bool ExecuteInPreview => true;
#endif

        [AutoParameter("type")] private string type;
        [AutoParameter(true, "")] private string message;

        private partial void ExecuteCommand(AutoParametersData data, ICommandContext context)
        {
            if (data.type == "")
            {
                Debug.Log(data.message);
                return;
            }

            switch (data.type)
            {
                case "w":
                case "warning":
                    Debug.LogWarning(message);
                    break;
                case "e":
                case "error":
                    Debug.LogError(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
        }
    }
}