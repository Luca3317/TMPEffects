using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPEffects.Tags;
using TMPEffects.Components;
using TMPEffects.Components.Writer;
using TMPEffects.TextProcessing;

namespace TMPEffects.TMPCommands.Commands
{
    [CreateAssetMenu(fileName = "new ShowCommand", menuName = "TMPEffects/Commands/Built-in/Show")]
    public class ShowCommand : TMPCommand
    {
        public override TagType TagType => TagType.Block;
        public override bool ExecuteInstantly => true;
        public override bool ExecuteOnSkip => false;
        public override bool ExecuteRepeatable => true;
#if UNITY_EDITOR 
        public override bool ExecuteInPreview => true;
#endif

        public override void ExecuteCommand(IDictionary<string, string> parameters, ICommandContext context)
        {
            context.WriterContext.Writer.Show(context.Indices.StartIndex, context.Indices.Length, true);
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters, IWriterContext context)
        {
            return true;
        }
    }
}
