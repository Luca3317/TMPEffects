using System;

namespace TMPEffects.ParameterUtilityGenerator.Attributes
{
    // TODO Update link once docs updated
    /// <summary>
    /// Turn this type into a valid parameter bundle for use with AutoParameters.<br/>
    /// To learn more about AutoParameters and creating your own bundles, read <see href="https://tmpeffects.luca3317.dev/plugins/autoparameters.html">HERE</see>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false, Inherited = false)]
    public class TMPParameterBundleAttribute : System.Attribute
    {
        public TMPParameterBundleAttribute(string displayName)
        {
        }
    }
}