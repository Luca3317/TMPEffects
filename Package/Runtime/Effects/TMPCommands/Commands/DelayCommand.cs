using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.Components.Writer;
using TMPEffects.Databases;
using UnityEngine;
using static TMPEffects.Parameters.TMPParameterUtility;

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

        public override void ExecuteCommand(ICommandContext context)
        {
            var writer = context.Writer;
            Data data = (Data)context.CustomData;

            if (string.IsNullOrWhiteSpace(data.methodIdentifier))
            {
                if (data.delay == -1)
                    writer.CurrentDelays.SetDelay(writer.DefaultDelays.delay);
                else
                    writer.CurrentDelays.SetDelay(data.delay);

                return;
            }


            switch (data.methodIdentifier)
            {
                case "whitespace":
                case "ws":
                    if (data.delay == -1)
                        writer.CurrentDelays.SetWhitespaceDelay(writer.DefaultDelays.whitespaceDelay,
                            writer.DefaultDelays.whitespaceDelayType);
                    else
                        writer.CurrentDelays.SetWhitespaceDelay(data.delay, data.delayType);
                    return;

                case "linebreak":
                case "linebr":
                case "br":
                    if (data.delay == -1)
                        writer.CurrentDelays.SetLinebreakDelay(writer.DefaultDelays.linebreakDelay,
                            writer.DefaultDelays.linebreakDelayType);
                    else
                        writer.CurrentDelays.SetLinebreakDelay(data.delay, data.delayType);
                    return;

                case "punctuation":
                case "punct":
                    if (data.delay == -1)
                        writer.CurrentDelays.SetPunctuationDelay(writer.DefaultDelays.punctuationDelay,
                            writer.DefaultDelays.punctuationDelayType);
                    else
                        writer.CurrentDelays.SetPunctuationDelay(data.delay, data.delayType);
                    return;

                case "visible":
                case "vis":
                    if (data.delay == -1)
                        writer.CurrentDelays.SetVisibleDelay(writer.DefaultDelays.visibleDelay,
                            writer.DefaultDelays.visibleDelayType);
                    else
                        writer.CurrentDelays.SetVisibleDelay(data.delay, data.delayType);
                    return;

                default: throw new System.InvalidOperationException();
            }
        }


        public override bool ValidateParameters(IDictionary<string, string> parameters,
            ITMPKeywordDatabase keywordDatabase)
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
            return HasFloatParameter(parameters, keywordDatabase, "") || parameters[""] == "" ||
                   parameters[""] == "default";
        }

        public override object GetNewCustomData()
        {
            return new Data();
        }

        public override void SetParameters(object obj, IDictionary<string, string> parameters,
            ITMPKeywordDatabase keywordDatabase)
        {
            Data data = (Data)obj;
            float delay = -1;
            TMPWriter.DelayType type = TMPWriter.DelayType.Raw;

            if (!TryGetFloatParameter(out delay, parameters, keywordDatabase, ""))
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
                if (!TryGetDefinedParameter(out string typestr, parameters, "type"))
                {
                    type = TMPWriter.DelayType.Raw;
                }
                else
                {
                    switch (parameters[typestr])
                    {
                        case "raw":
                            type = TMPWriter.DelayType.Raw;
                            break;
                        case "percentage":
                        case "pct":
                        case "%":
                            type = TMPWriter.DelayType.Percentage;
                            break;

                        default:
                            type = TMPWriter.DelayType.Raw;
                            break;
                    }
                }

                data.methodIdentifier = parameters[str];
            }

            data.delay = delay;
            data.delayType = type;
        }

        private class Data
        {
            public float delay;
            public TMPWriter.DelayType delayType;
            public string methodIdentifier = null;
        }
    }
}