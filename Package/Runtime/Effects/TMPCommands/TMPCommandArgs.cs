using TMPEffects.Components;
using TMPEffects.Tags;

namespace TMPEffects.TMPCommands
{
    /// <summary>
    /// The argument object for <see cref="ITMPCommand"/>.
    /// </summary>
    public class TMPCommandArgs
    {
        /// <summary>
        /// The tag of the command.
        /// </summary>
        public readonly TMPEffectTag tag;
        /// <summary>
        /// The indices of the tag.
        /// </summary>
        public readonly TMPEffectTagIndices indices;
        /// <summary>
        /// The <see cref="TMPWriter"/> that executed this command.
        /// </summary>
        public readonly TMPWriter writer;

        public TMPCommandArgs(TMPEffectTag tag, TMPEffectTagIndices indices, TMPWriter writer)
        {
            this.tag = tag;
            this.indices = indices;
            this.writer = writer;
        }
    }
}
