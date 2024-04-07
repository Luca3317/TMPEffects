using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.TMPCommands.Commands
{
    [CreateAssetMenu(fileName = "new DelayCommand", menuName = "TMPEffects/Commands/Delay")]
    public class DelayCommand : TMPCommand
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
            if (ParameterUtility.TryGetFloatParameter(out float delay, args.tag.Parameters, ""))
            {
                args.writer.SetDelay(delay);
                return;
            }
             
            // Since validate parameters ensures the parameter is present and float,
            // this state should be impossible to reach
            throw new System.InvalidOperationException();
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return false;
            if (!parameters.ContainsKey(""))
                return false;

            return ParameterUtility.HasFloatParameter(parameters, "");
        }
    }
}

