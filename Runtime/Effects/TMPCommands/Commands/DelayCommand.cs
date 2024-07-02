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
                if (args.tag.Parameters[""] == "" || args.tag.Parameters[""] == "default")
                    delay = -1;
                else
                {
                    // Since validate parameters ensures the parameter is in one of the three valid states,
                    // this state should be impossible to reach
                    throw new System.InvalidOperationException();
                }
            }

            if (ParameterUtility.TryGetDefinedParameter(out string str, args.tag.Parameters, "for"))
            {
                TMPWriter.DelayType type;
                if (!ParameterUtility.TryGetDefinedParameter(out string typestr, args.tag.Parameters, "type"))
                {
                    type = TMPWriter.DelayType.Raw;
                }
                else
                {
                    switch (args.tag.Parameters[typestr])
                    {
                        case "raw": type = TMPWriter.DelayType.Raw; break;
                        case "percentage":
                        case "pct":
                        case "%": type = TMPWriter.DelayType.Percentage; break;

                        default: type = TMPWriter.DelayType.Raw; break;
                    }
                }

                switch (args.tag.Parameters[str])
                {
                    case "whitespace":
                    case "ws":
                        if (delay == -1)
                            args.writer.CurrentDelays.SetWhitespaceDelay(args.writer.DefaultDelays.whitespaceDelay, args.writer.DefaultDelays.whitespaceDelayType);
                        else
                            args.writer.CurrentDelays.SetWhitespaceDelay(delay, type);
                        return;

                    case "linebreak":
                    case "linebr":
                    case "br":
                        if (delay == -1)
                            args.writer.CurrentDelays.SetLinebreakDelay(args.writer.DefaultDelays.linebreakDelay, args.writer.DefaultDelays.linebreakDelayType);
                        else
                            args.writer.CurrentDelays.SetLinebreakDelay(delay, type);
                        return;

                    case "punctuation":
                    case "punct":
                        if (delay == -1)
                            args.writer.CurrentDelays.SetPunctuationDelay(args.writer.DefaultDelays.punctuationDelay, args.writer.DefaultDelays.punctuationDelayType);
                        else
                            args.writer.CurrentDelays.SetPunctuationDelay(delay, type);
                        return;

                    case "visible":
                    case "vis":
                        if (delay == -1)
                            args.writer.CurrentDelays.SetVisibleDelay(args.writer.DefaultDelays.visibleDelay, args.writer.DefaultDelays.visibleDelayType);
                        else
                            args.writer.CurrentDelays.SetVisibleDelay(delay, type);
                        return;

                    default: throw new System.InvalidOperationException();
                }
            }
            else
            {
                if (delay == -1)
                    args.writer.CurrentDelays.SetDelay(args.writer.DefaultDelays.delay);
                else
                    args.writer.CurrentDelays.SetDelay(delay);
            }
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return false;
            if (!parameters.ContainsKey(""))
            {
                return false;
            }

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

            return ParameterUtility.HasFloatParameter(parameters, "") || parameters[""] == "" || parameters[""] == "default";
        }
    }
}

