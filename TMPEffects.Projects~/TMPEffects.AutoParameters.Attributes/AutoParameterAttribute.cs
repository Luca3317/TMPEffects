using System;

namespace TMPEffects.AutoParameters.Attributes
{
    // TODO Update link once docs updated
    /// <summary>
    /// Automatically use this field as parameter.<br/>
    /// To learn more about AutoParameters, read <see href="https://tmpeffects.luca3317.dev/plugins/autoparameters.html">HERE</see>
    /// </summary>
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