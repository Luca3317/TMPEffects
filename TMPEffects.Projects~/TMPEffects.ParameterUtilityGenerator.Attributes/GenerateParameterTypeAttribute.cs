using System;

namespace TMPEffects.ParameterUtilityGenerator.Attributes
{
    // TODO Maybe move this into tmpeffects as an internal class, definitely shouldnt be exposed to users
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