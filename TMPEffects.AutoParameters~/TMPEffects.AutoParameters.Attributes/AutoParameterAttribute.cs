using System;

namespace TMPEffects.AutoParameters.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AutoParameterAttribute : Attribute
    {
        private bool required;
        private string name;
        private string[] aliases;

        public AutoParameterAttribute(string name, params string[] aliases)
        {
            required = false;
            this.name = name;
            this.aliases = aliases;
        }
        public AutoParameterAttribute(bool required, string name, params string[] aliases)
        {
            this.required = required;
            this.name = name;
            this.aliases = aliases;
        }
    }
}
