using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPEffects.Tags;
using TMPEffects.Components;

namespace TMPEffects.Commands
{
    [CreateAssetMenu(fileName = "new ShowCommand", menuName = "TMPEffects/Commands/Show")]
    public class ShowCommand : TMPCommand
    {
        public override TagType TagType => TagType.Container;
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

        public override bool ValidateParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null) return true;
            if (parameters.ContainsKey(""))
            {
                if (!float.TryParse(parameters[""], NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                    return false;
            }
            return true;
        }
    }
}
