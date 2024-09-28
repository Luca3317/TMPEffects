using System;

namespace TMPEffects.Parameters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ParameterTypeAttribute : Attribute
    {
        public string DisplayName => displayName;
        public Type Type => type;

        private string displayName;
        private Type type;

        public ParameterTypeAttribute(Type parameterType, string displayName)
        {
            this.displayName = displayName;
            this.type = parameterType;
        }
    }
}