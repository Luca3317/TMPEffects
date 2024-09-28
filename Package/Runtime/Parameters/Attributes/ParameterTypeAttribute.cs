using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.Parameters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ParameterTypeAttribute : Attribute
    {
        public string DisplayName => displayName;
        public Type Type => type;

        private string displayName;
        private Type type;

        public ParameterTypeAttribute(Type parameterType, string displayName)
        {
            this.displayName = displayName;
            this.type = parameterType;
        }
    }
}