using System;

namespace TMPEffects.ParameterUtilityGenerator.Attributes
{
    // TODO Update link once docs updated
    /// <summary>
    /// Use this field as parameter for the bundle.<br/>
    /// To learn more about AutoParameters and creating your own bundles, read <see href="https://tmpeffects.luca3317.dev/plugins/autoparameters.html">HERE</see>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class TMPParameterBundleFieldAttribute : System.Attribute
    {
        public TMPParameterBundleFieldAttribute(string displayName, params string[] aliases)
        {
        }
    }
}