using System;

namespace TMPEffects.Parameters.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class TMPParameterBundleFieldAttribute : Attribute
    {
        public TMPParameterBundleFieldAttribute(string displayName, params string[] aliases)
        {
        }
    }
}