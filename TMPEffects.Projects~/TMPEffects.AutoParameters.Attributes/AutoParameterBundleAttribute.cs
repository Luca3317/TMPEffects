using System;
using System.Collections.Generic;
using System.Text;

namespace TMPEffects.AutoParameters.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AutoParameterBundleAttribute : AutoParameterAttribute
    {
        public AutoParameterBundleAttribute(string prefix) : base(false, prefix)
        { }
    }
}