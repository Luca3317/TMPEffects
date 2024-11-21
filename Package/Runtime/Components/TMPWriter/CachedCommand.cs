using System.Collections.Generic;
using TMPEffects.Tags;
using TMPEffects.TMPCommands;

namespace TMPEffects.Components.Writer
{
    internal class CachedCommand : ITagWrapper, ICachedInvokable
    {
        public TMPEffectTag Tag { get; private set; }
        public TMPEffectTagIndices Indices { get; private set; }

        public ITMPCommand command { get; private set; }
        public TMPCommandArgs args { get; private set; }
        public bool Triggered { get; private set; }

        public ReadOnlyCommandContext roContext { get; private set; }

        public bool ExecuteInstantly => command.ExecuteInstantly;
        public bool ExecuteOnSkip => command.ExecuteOnSkip;
        public bool ExecuteRepeatable => command.ExecuteRepeatable;
#if UNITY_EDITOR
        public bool ExecuteInPreview => command.ExecuteInPreview;
#endif

        public void Trigger()
        {
            if (Triggered) return;

            Triggered = true;
            command.ExecuteCommand(roContext);
        }

        public void Reset()
        {
            if (!ExecuteRepeatable) return;
            Triggered = false;
        }

        public CachedCommand(TMPEffectTag tag, TMPEffectTagIndices indices, CommandContext context, ITMPCommand command)
            => Reset(tag, indices, context, command);

        public void Reset(TMPEffectTag tag, TMPEffectTagIndices indices, CommandContext context, ITMPCommand command)
        {
            this.Tag = tag;
            this.Indices = indices;
            this.command = command;
            this.Triggered = false;
            roContext = new ReadOnlyCommandContext(context);
        }
    }
}