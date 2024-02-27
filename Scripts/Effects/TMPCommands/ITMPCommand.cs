namespace TMPEffects.TMPCommands
{
    public interface ITMPCommand
    {
        public TagType TagType { get; }

        public bool ExecuteInstantly { get; }
        public bool ExecuteOnSkip { get; }
        public bool ExecuteRepeatable { get; }

#if UNITY_EDITOR
        public bool ExecuteInPreview { get; }
#endif

        public void ExecuteCommand(TMPCommandArgs args);
    }
}
