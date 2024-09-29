using System;
using System.Collections.Generic;
using System.Text;

namespace TMPEffects.AutoParameters.Attributes
{
    public class AutoParameterBundleAttribute : AutoParameterAttribute
    {
        public AutoParameterBundleAttribute(string prefix) : base(false, prefix)
        { }
    }
}
