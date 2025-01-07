using TMPEffects.AutoParameters.Attributes;
using UnityEngine;

namespace TMPEffects.TMPCommands.Commands
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new ShowCommand", menuName = "TMPEffects/Commands/Built-in/Show")]
    public partial class ShowCommand : TMPCommand
    {
        public override TagType TagType => TagType.Block;
        public override bool ExecuteInstantly => true;
        public override bool ExecuteOnSkip => false;
        public override bool ExecuteRepeatable => true;
#if UNITY_EDITOR 
        public override bool ExecuteInPreview => true;
#endif
        
        private partial void ExecuteCommand(AutoParametersData data, ICommandContext context)
        {
            context.Writer.Show(context.Indices.StartIndex, context.Indices.Length, true);
        }
    }
}
