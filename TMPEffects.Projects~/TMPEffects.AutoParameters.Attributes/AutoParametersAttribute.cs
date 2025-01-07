using System;
using System.Collections.Generic;
using System.Text;

namespace TMPEffects.AutoParameters.Attributes
{
    // TODO Update link once docs updated
    /// <summary>
    /// Automatically generate parameter handling for this type.<br/>
    /// Only valid for types that implement ITMPAnimation or ITMPCommand.<br/>
    /// To learn more about AutoParameters, read <see href="https://tmpeffects.luca3317.dev/plugins/autoparameters.html">HERE</see>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class AutoParametersAttribute : Attribute
    {
    }
}