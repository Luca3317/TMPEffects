using System;

namespace TMPEffects.ParameterUtilityGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GenerateParameterTypeAttribute : System.Attribute
    {
        private Type _type;
        
        public GenerateParameterTypeAttribute(Type type)
        {
            _type = type;
        }
    }
}