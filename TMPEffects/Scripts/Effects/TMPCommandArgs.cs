using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.Tags;

namespace TMPEffects
{
    public class TMPCommandArgs
    {
        public readonly EffectTag tag;
        public readonly EffectTagIndices indices;
        public readonly TMPWriter writer;

        public TMPCommandArgs(EffectTag tag, EffectTagIndices indices, TMPWriter writer)
        {
            this.tag = tag;
            this.indices = indices;
            this.writer = writer;
        }
    }
}
