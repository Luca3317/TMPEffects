using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.Components.Writer;
using UnityEngine;

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
        
        [AutoParameter("type")]
        private string type;

        [AutoParameter(true, "")]
        private string message;

        private partial void ExecuteCommand(IDictionary<string, string> parameters, AutoParametersData data,
            ICommandContext context)
        {
            if (data.type == "")
            {
                Debug.Log(data.message);
                return;
            }

            switch (data.type)
            {
                case "w":
                case "warning": Debug.LogWarning(parameters[""]); break;
                case "e":
                case "error": Debug.LogError(parameters[""]); break;
                default: Debug.Log(parameters[""]); break;
            }
        }
    }
}
