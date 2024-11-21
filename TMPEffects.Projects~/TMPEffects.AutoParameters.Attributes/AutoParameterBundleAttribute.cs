using System;
using System.Collections.Generic;
using System.Text;

namespace TMPEffects.AutoParameters.Attributes
{
    // TODO Update link once docs updated
    /// <summary>
    /// Automatically use this field as parameter bundle.<br/>
    /// To learn more about AutoParameters, read <see href="https://tmpeffects.luca3317.dev/plugins/autoparameters.html">HERE</see>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class AutoParameterBundleAttribute : AutoParameterAttribute
    {
        public AutoParameterBundleAttribute(string prefix) : base(false, prefix)
        { }
    }
}