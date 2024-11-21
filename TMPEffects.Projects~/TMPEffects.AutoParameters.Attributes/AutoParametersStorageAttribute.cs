using System;
using System.Collections.Generic;
using System.Text;

namespace TMPEffects.AutoParameters.Attributes
{
    // TODO Update link once docs updated
    /// <summary>
    /// Automatically use this type as storage for the parsed parameters.<br/>
    /// To learn more about AutoParameters, read <see href="https://tmpeffects.luca3317.dev/plugins/autoparameters.html">HERE</see>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class AutoParametersStorageAttribute : Attribute
    {
    }
}