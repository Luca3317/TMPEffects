using System;

namespace TMPEffects.SerializedCollections.KeysGenerators
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class KeyListGeneratorAttribute : Attribute
    {
        public readonly string Name;
        public readonly Type TargetType;
        public readonly bool NeedsWindow;

        public KeyListGeneratorAttribute(string name, Type targetType, bool needsWindow = true)
        {
            Name = name;
            TargetType = targetType;
            NeedsWindow = needsWindow;
        }
    }
}