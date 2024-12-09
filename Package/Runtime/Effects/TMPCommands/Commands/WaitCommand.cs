using TMPEffects.AutoParameters.Attributes;
using UnityEngine;

namespace TMPEffects.TMPCommands.Commands
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new WaitCommand", menuName = "TMPEffects/Commands/Built-in/Wait")]
    public partial class WaitCommand : TMPCommand
    {
        public override TagType TagType => TagType.Index;
        public override bool ExecuteInstantly => false;
        public override bool ExecuteOnSkip => false;
        public override bool ExecuteRepeatable => true;

#if UNITY_EDITOR
        public override bool ExecuteInPreview => true;
#endif

        [AutoParameter(true, "")] private float waitTime;

        private partial void ExecuteCommand(AutoParametersData data, ICommandContext context)
        {
            context.Writer.Wait(data.waitTime);
        }
    }
}