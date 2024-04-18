using TMPEffects.Tags;
using TMPEffects.TMPCommands;

namespace TMPEffects.Components.Writer
{
internal class CachedCommand : ITagWrapper, ICachedInvokable
{
    public TMPEffectTag Tag => args.tag;
    public TMPEffectTagIndices Indices => args.indices;

    public ITMPCommand command { get; private set; }
    public TMPCommandArgs args { get; private set; }
    public bool Triggered { get; private set; }

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
        command.ExecuteCommand(args);
    }

    public void Reset()
    {
        if (!ExecuteRepeatable) return;
        Triggered = false;
    }

    public CachedCommand(TMPCommandArgs args, ITMPCommand command) => Reset(args, command);
    public void Reset(TMPCommandArgs args, ITMPCommand command)
    {
        this.args = args;
        this.command = command;
        this.Triggered = false;
    }
}
}