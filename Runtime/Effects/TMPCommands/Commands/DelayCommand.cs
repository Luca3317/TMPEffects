using System.Collections.Generic;
using TMPEffects.Components;
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
            if (!ParameterUtility.TryGetFloatParameter(out float delay, args.tag.Parameters, ""))
            {
                // Since validate parameters ensures the parameter is present and float,
                // this state should be impossible to reach
                throw new System.InvalidOperationException();
            }
               
            if (ParameterUtility.TryGetDefinedParameter(out string str, args.tag.Parameters, "for"))
            {
                if (!ParameterUtility.TryGetDefinedParameter(out string typestr, args.tag.Parameters, "type"))
                    throw new System.InvalidOperationException();

                TMPWriter.DelayType type;
                switch (args.tag.Parameters[typestr])
                {
                    case "raw": type = TMPWriter.DelayType.Raw; break;
                    case "percentage":
                    case "pct":
                    case "%": type = TMPWriter.DelayType.Percentage; break;

                    default: type = TMPWriter.DelayType.Raw; break;
                }

                switch (args.tag.Parameters[str])
                {
                    case "whitespace":
                    case "ws": args.writer.SetWhitespaceDelay(delay, type); return;

                    case "linebreak":
                    case "linebr":
                    case "br": args.writer.SetLinebreakDelay(delay, type); return;

                    case "punctuation":
                    case "punct": args.writer.SetPunctuationDelay(delay, type); return;

                    case "visible":
                    case "vis": args.writer.SetVisibleDelay(delay, type); ; return;

                    default: throw new System.InvalidOperationException();
                }
            }
            else
            {
                args.writer.SetDelay(delay);
            }
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return false;
            if (!parameters.ContainsKey(""))
                return false;

            if (ParameterUtility.TryGetDefinedParameter(out string str, parameters, "for"))
            {
                switch (parameters[str])
                {
                    case "whitespace":
                    case "ws":
                    case "linebreak":
                    case "linebr":
                    case "br":
                    case "punctuation":
                    case "punct":
                    case "visible":
                    case "vis": break;

                    default: return false;
                }

                if (ParameterUtility.TryGetDefinedParameter(out str, parameters, "type"))
                {
                    switch (parameters[str])
                    {
                        case "raw":
                        case "percentage":
                        case "pct":
                        case "%": break;

                        default: return false;
                    }
                }
            }

            return ParameterUtility.HasFloatParameter(parameters, "");
        }
    }
}

