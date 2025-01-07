using System;

namespace TMPEffects.Parameters.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class TMPParameterBundleAttribute : Attribute
    {
        public TMPParameterBundleAttribute(string displayName)
        {
        }
    }
}