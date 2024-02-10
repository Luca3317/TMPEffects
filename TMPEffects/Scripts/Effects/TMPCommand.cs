using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Tags;
using TMPEffects.Components;

namespace TMPEffects
{
    /// <summary>
    /// Base class for commands.
    /// </summary>
    public abstract class TMPCommand : ScriptableObject, ITMPCommand
    {
        /// <summary>
        /// The type of command this is.
        /// </summary>
        /// <remarks>
        /// <list type="table">
        /// <item><see cref="TagType.Empty"/>: This type of command is executed when the <see cref="TMPWriter"/> shows the character at the corresponding index. It does not need to be closed. Example: This is <!speed=10> my text</item>
        /// <item><see cref="TagType.Container"/>: This type of command is executed when the <see cref="TMPWriter"/> shows the character at the first corresponding index. It needs to be closed, and will operate on the enclosed text. Example: This <!show>is my</!show> text</item>
        /// <item><see cref="TagType.Either"/>: Both applications are valid.</item>
        /// </list>
        /// </remarks>
        public abstract TagType TagType { get; }
        /// <summary>
        /// Whether the command is executed the moment the <see cref="TMPWriter"/> begin writing.
        /// Otherwise, it is executed when the <see cref="TMPWriter"/> shows the character at the corresponding index
        /// </summary>
        public abstract bool ExecuteInstantly { get; }

        /// <summary>
        /// Whether the command should be executed by the <see cref="TMPWriter"/> if its text position is skipped over.
        /// </summary>
        public abstract bool ExecuteOnSkip { get; }

        public abstract bool ExecuteRepeatable { get; }

#if UNITY_EDITOR
        public abstract bool ExecuteInPreview { get; }
#endif

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="writer"></param>
        public abstract void ExecuteCommand(TMPCommandArgs args);
        /// <summary>
        /// Validate the parameters.<br/>
        /// Used to validate tags.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public abstract bool ValidateParameters(Dictionary<string, string> parameters);
    }

    public enum TagType
    {
        Empty = 0,
        Container = 1,
        Either = 2
    }
}