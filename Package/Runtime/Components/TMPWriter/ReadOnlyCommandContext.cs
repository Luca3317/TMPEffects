using TMPEffects.Components;
using TMPEffects.Tags;

namespace TMPEffects.TMPCommands
{
    /// <summary>
    /// A read-only wrapper around <see cref="CommandContext"/>.
    /// </summary>
    public class ReadOnlyCommandContext : ICommandContext
    {
        public TMPWriter Writer => context.Writer;
        public TMPEffectTagIndices Indices => context.Indices;
        public object CustomData => context.CustomData;

        public ReadOnlyCommandContext(ICommandContext context)
        {
            this.context = context;
        }

        private ICommandContext context;
    }
}