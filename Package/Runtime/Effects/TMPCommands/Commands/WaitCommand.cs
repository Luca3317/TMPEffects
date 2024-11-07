using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.Components.Writer;
using UnityEngine;
using TMPEffects.Parameters;

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

        private partial void ExecuteCommand(IDictionary<string, string> parameters, AutoParametersData data,
            ICommandContext context)
        {
            context.WriterContext.Writer.Wait(data.waitTime);
        }

        // public override void ExecuteCommand(IDictionary<string, string> parameters, ICommandContext context)
        // {
        //     ParameterParsing.StringToFloat(parameters[""], out var value, context.WriterContext.KeywordDatabase);
        //     context.WriterContext.Writer.Wait(value);
        // }
        //
        // public override bool ValidateParameters(IDictionary<string, string> parameters, IWriterContext context)
        // {
        //     if (parameters == null) return false;
        //     if (!parameters.ContainsKey(""))
        //         return false;
        //     
        //     return ParameterParsing.StringToFloat(parameters[""], out _, context.KeywordDatabase);
        // }
    }
}