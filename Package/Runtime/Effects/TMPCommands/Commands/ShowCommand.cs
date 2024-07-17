using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPEffects.Tags;
using TMPEffects.Components;
using TMPEffects.TextProcessing;

namespace TMPEffects.TMPCommands.Commands
{
    [CreateAssetMenu(fileName = "new ShowCommand", menuName = "TMPEffects/Commands/Show")]
    public class ShowCommand : TMPCommand
    {
        public override TagType TagType => TagType.Block;
        public override bool ExecuteInstantly => true;
        public override bool ExecuteOnSkip => false;
        public override bool ExecuteRepeatable => true;
#if UNITY_EDITOR 
        public override bool ExecuteInPreview => true;
#endif

        public override void ExecuteCommand(TMPCommandArgs args)
        {
            args.writer.Show(args.indices.StartIndex, args.indices.Length, true);
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            return true;
        }
    }
}
