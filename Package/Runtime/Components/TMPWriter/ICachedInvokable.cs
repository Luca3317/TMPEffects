using TMPEffects.Tags;

namespace TMPEffects.Components.Writer
{
    internal interface ICachedInvokable : ITagWrapper
    {
        public bool Triggered { get; }
        public bool ExecuteInstantly { get; }
        public bool ExecuteOnSkip { get; }
        public bool ExecuteRepeatable { get; }
#if UNITY_EDITOR
        public bool ExecuteInPreview { get; }
#endif
        public void Reset();
        public void Trigger();
    }
}