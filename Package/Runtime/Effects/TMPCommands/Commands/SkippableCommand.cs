using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.Components.Writer;
using UnityEngine;
using TMPEffects.Parameters;

namespace TMPEffects.TMPCommands.Commands
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new SkippableCommand", menuName = "TMPEffects/Commands/Built-in/Skippable")]
    public partial class SkippableCommand : TMPCommand
    {
        public override TagType TagType => TagType.Index;

        public override bool ExecuteInstantly => false;

        public override bool ExecuteOnSkip => true;

        public override bool ExecuteRepeatable => true;

#if UNITY_EDITOR
        public override bool ExecuteInPreview => true;
#endif

        [AutoParameter(true, "")] private bool skippable = false;

        private partial void ExecuteCommand(AutoParametersData data, ICommandContext context)
        {
            context.Writer.SetSkippable(data.skippable);
        }
    }
}