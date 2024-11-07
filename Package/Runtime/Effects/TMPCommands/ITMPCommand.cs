using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.Components.Writer;

namespace TMPEffects.TMPCommands
{
    /// <summary>
    /// Base interface for all TMPEffects commands.
    /// </summary>
    public interface ITMPCommand
    {
        /// <summary>
        /// The type of command this is.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Index: This type of command is executed when the <see cref="TMPWriter"/> shows the character at the corresponding index. It does not need to be closed.<br/> Example: This is &lt;!delay=0.01&gt; my text
        /// </para>
        /// <para>
        /// Block: This type of command is executed when the <see cref="TMPWriter"/> shows the character at the first corresponding index. It needs to be closed, and will operate on the enclosed text.<br/> Example: This &lt;!show&gt;is my&lt;/!show&gt; text
        /// </para>
        /// <para>
        /// Either: Both applications are valid.
        /// </para>
        /// </remarks>
        public TagType TagType { get; }

        /// <summary>
        /// Whether the command is executed the moment the <see cref="TMPWriter"/> begin writing.<br/>
        /// Otherwise, it is executed when the <see cref="TMPWriter"/> shows the character at the corresponding index.
        /// </summary>
        public bool ExecuteInstantly { get; }
        /// <summary>
        /// Whether the command should be executed by the <see cref="TMPWriter"/> if its text position is skipped over.
        /// </summary>
        public bool ExecuteOnSkip { get; }
        /// <summary>
        /// Whether the command may be executed multiple times if, for example, the <see cref="TMPWriter"/> is reset to an earlier text position after the command has been executed.<br/>
        /// An example for a command that should not be repeatable is one that triggers a quest, or adds an item to the player's inventory.
        /// </summary>
        public bool ExecuteRepeatable { get; }

#if UNITY_EDITOR
        /// <summary>
        /// Whether the command may be executed in preview mode.<br/>
        /// ! ONLY AVAILABLE IN THE EDITOR !<br/>
        /// You must wrap usages and implementations of this in a #if UNITY_EDITOR directive.<br/>
        /// This should be false for any command that makes changes, for example manipulating assets.<br/>
        /// It's recommended to set this to false by default.
        /// </summary>
        public bool ExecuteInPreview { get; }
#endif

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="context"></param>
        public void ExecuteCommand(IDictionary<string, string> parameters, ICommandContext context);

        /// <summary>
        /// Validate the parameters.<br/>
        /// Used to validate tags.
        /// </summary>
        /// <param name="parameters">The parameters to validate.</param>
        /// <param name="context"></param>
        /// <returns>true if the parameters were successfully validated; false otherwise.</returns>
        public bool ValidateParameters(IDictionary<string, string> parameters, IWriterContext context);
    }
}
