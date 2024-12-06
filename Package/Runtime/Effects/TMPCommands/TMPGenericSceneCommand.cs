using System;
using System.Collections.Generic;
using System.ComponentModel;
using TMPEffects.Components.Writer;
using TMPEffects.Databases;
using TMPEffects.ObjectChanged;
using TMPEffects.TMPCommands;
using UnityEngine;
using UnityEngine.Events;

namespace TMPEffects.TMPCommands
{
    /// <summary>
    /// Struct defining generic scene commands.
    /// </summary>
    [System.Serializable]
    public struct TMPGenericSceneCommand : ITMPCommand
    {
        /// <inheritdoc/>
        public TagType TagType => commandType;

        /// <inheritdoc/>
        public bool ExecuteInstantly => executeInstantly;

        /// <inheritdoc/>
        public bool ExecuteOnSkip => executeOnSkip;

        /// <inheritdoc/>
        public bool ExecuteRepeatable => executeRepeatable;

#if UNITY_EDITOR
        /// <inheritdoc/>
        public bool ExecuteInPreview => false;
#endif
        [Tooltip("Whether tags of this command operate on a range (and therefore need to be closed) or on an index.")]
        [SerializeField]
        private TagType commandType;

        [Tooltip(
            "Whether the command is executed instantly when the writer begins writing the text, regardless of its position within it.")]
        [SerializeField]
        private bool executeInstantly;

        [Tooltip(
            "Whether the command should be executed when the tag's position is skipped by the writer. Check for essential commands, e.g. triggering a quest.")]
        [SerializeField]
        private bool executeOnSkip;

        [Tooltip(
            "Whether the command should be allowed to be executed multiple times. Relevant for when the writer is reset while writing.")]
        [SerializeField]
        private bool executeRepeatable;

        [Tooltip("The methods to trigger.")] [SerializeField]
        private UnityEvent<IDictionary<string, string>, ICommandContext> command;

        /// <inheritdoc/>
        public void ExecuteCommand(ICommandContext context)
        {
            var parameters = ((ParameterContainer)context.CustomData).parameters;
            command?.Invoke(parameters, context);
        }

        /// <inheritdoc/>
        public bool ValidateParameters(IDictionary<string, string> parameters, ITMPKeywordDatabase keywordDatabase)
        {
            return true;
        }

        /// <inheritdoc/>
        public object GetNewCustomData()
        {
            return new ParameterContainer();
        }

        /// <inheritdoc/>
        public void SetParameters(object obj, IDictionary<string, string> parameters,
            ITMPKeywordDatabase keywordDatabase)
        {
            ((ParameterContainer)obj).parameters = parameters;
        }

        private class ParameterContainer
        {
            public IDictionary<string, string> parameters;
        }
    }
}