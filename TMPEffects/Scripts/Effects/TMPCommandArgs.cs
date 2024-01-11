using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.Tags;

namespace TMPEffects
{
    public class TMPCommandArgs
    {
        public readonly TMPCommandTag tag;
        public readonly TMPWriter writer;

        public TMPCommandArgs(TMPCommandTag tag, TMPWriter writer)
        {
            this.tag = tag;
            this.writer = writer;
        }
    }
}
