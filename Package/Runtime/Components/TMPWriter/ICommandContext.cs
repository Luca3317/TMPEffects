using TMPEffects.Components;
using TMPEffects.Tags;

namespace TMPEffects.TMPCommands
{
    /// <summary>
    /// Interface for command contexts.
    /// </summary>
    public interface ICommandContext
    {
        /// <summary>
        /// The <see cref="TMPWriter"/> that raised this command.
        /// </summary>
        public TMPWriter Writer { get; }
        
        /// <summary>
        /// The indices of the tag that triggered this command.
        /// </summary>
        public TMPEffectTagIndices Indices { get; }

        /// <summary>
        /// The custom data object.
        /// </summary>
        public object CustomData { get; }
    }
}