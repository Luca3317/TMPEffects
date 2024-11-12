using System;

namespace TMPEffects.ParameterUtilityGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct,
        AllowMultiple = false, Inherited = false)]
    public class TMPParameterTypeAttribute : System.Attribute
    {
        private bool generateKeywordDatabase;
        private string displayName;
        private Type sceneType;
        private Type diskType;
        private Type sharedBaseType;

        public TMPParameterTypeAttribute(string displayName, bool generateKeywordDatabase = false)
        {
            this.displayName = displayName;
            this.generateKeywordDatabase = generateKeywordDatabase;
        }

        public TMPParameterTypeAttribute(string displayName, Type diskType, Type sceneType,
            bool generateKeywordDatabase = false)
        {
            this.displayName = displayName;
            this.diskType = diskType;
            this.sceneType = sceneType;
            this.generateKeywordDatabase = generateKeywordDatabase;
        }
    }
}