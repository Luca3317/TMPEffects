using TMPEffects.Components;
using TMPEffects.Tags;

namespace TMPEffects.TMPCommands
{
    public class TMPCommandArgs
    {
        public readonly TMPEffectTag tag;
        public readonly TMPEffectTagIndices indices;
        public readonly TMPWriter writer;

        public TMPCommandArgs(TMPEffectTag tag, TMPEffectTagIndices indices, TMPWriter writer)
        {
            this.tag = tag;
            this.indices = indices;
            this.writer = writer;
        }
    }
}
