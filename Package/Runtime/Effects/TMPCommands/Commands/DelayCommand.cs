using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.Components.Writer;
using UnityEngine;
using static TMPEffects.Parameters.ParameterUtility;

namespace TMPEffects.TMPCommands.Commands
{
    [CreateAssetMenu(fileName = "new DelayCommand", menuName = "TMPEffects/Commands/Built-in/Delay")]
    public class DelayCommand : TMPCommand
    {
        public override TagType TagType => TagType.Index;
        public override bool ExecuteInstantly => false;
        public override bool ExecuteOnSkip => true;
        public override bool ExecuteRepeatable => true;

#if UNITY_EDITOR
        public override bool ExecuteInPreview => true;
#endif

        public override void ExecuteCommand(IDictionary<string, string> parameters, ICommandContext context)
        {
            var writer = context.WriterContext.Writer;
            
            if (!TryGetFloatParameter(out float delay, parameters, context.WriterContext.KeywordDatabase, ""))
            {
                if (parameters[""] == "" || parameters[""] == "default")
                    delay = -1;
                else
                {
                    // Since validate parameters ensures the parameter is in one of the three valid states,
                    // this state should be impossible to reach
                    throw new System.InvalidOperationException();
                }
            }

            if (TryGetDefinedParameter(out string str, parameters, "for"))
            {
                TMPWriter.DelayType type;
                if (!TryGetDefinedParameter(out string typestr, parameters, "type"))
                {
                    type = TMPWriter.DelayType.Raw;
                }
                else
                {
                    switch (parameters[typestr])
                    {
                        case "raw": type = TMPWriter.DelayType.Raw; break;
                        case "percentage":
                        case "pct":
                        case "%": type = TMPWriter.DelayType.Percentage; break;

                        default: type = TMPWriter.DelayType.Raw; break;
                    }
                }

                switch (parameters[str])
                {
                    case "whitespace":
                    case "ws":
                        if (delay == -1)
                            writer.CurrentDelays.SetWhitespaceDelay(writer.DefaultDelays.whitespaceDelay, writer.DefaultDelays.whitespaceDelayType);
                        else
                            writer.CurrentDelays.SetWhitespaceDelay(delay, type);
                        return;

                    case "linebreak":
                    case "linebr":
                    case "br":
                        if (delay == -1)
                            writer.CurrentDelays.SetLinebreakDelay(writer.DefaultDelays.linebreakDelay, writer.DefaultDelays.linebreakDelayType);
                        else
                            writer.CurrentDelays.SetLinebreakDelay(delay, type);
                        return;

                    case "punctuation":
                    case "punct":
                        if (delay == -1)
                            writer.CurrentDelays.SetPunctuationDelay(writer.DefaultDelays.punctuationDelay, writer.DefaultDelays.punctuationDelayType);
                        else
                            writer.CurrentDelays.SetPunctuationDelay(delay, type);
                        return;

                    case "visible":
                    case "vis":
                        if (delay == -1)
                            writer.CurrentDelays.SetVisibleDelay(writer.DefaultDelays.visibleDelay, writer.DefaultDelays.visibleDelayType);
                        else
                            writer.CurrentDelays.SetVisibleDelay(delay, type);
                        return;

                    default: throw new System.InvalidOperationException();
                }
            }
            else
            {
                if (delay == -1)
                    writer.CurrentDelays.SetDelay(writer.DefaultDelays.delay);
                else
                    writer.CurrentDelays.SetDelay(delay);
            }
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters, IWriterContext context)
        {
            if (parameters == null) return false;
            if (!parameters.ContainsKey(""))
            {
                return false;
            }
            
            // If defines a "for", validate its value
            if (TryGetDefinedParameter(out string str, parameters, "for"))
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

                // If defines a "type", validate its value
                if (TryGetDefinedParameter(out str, parameters, "type"))
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

            // Check whether value is float
            return HasFloatParameter(parameters, context.KeywordDatabase, "") || parameters[""] == "" || parameters[""] == "default";
        }
    }
}

