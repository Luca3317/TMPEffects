using System;

namespace TMPEffects.ParameterUtilityGenerator.Attributes
{
    public class TMPParameterTypeAttribute : System.Attribute
    {
        private bool generateKeywordDatabase;
        private string displayName;
        private Type sceneType;
        private Type diskType;
        private Type sharedBaseType;

        public TMPParameterTypeAttribute(string displayName, Type type, bool generateKeywordDatabase = false)
        {
            this.displayName = displayName;
            this.sceneType = type;
            this.diskType = type;
            this.sharedBaseType = type;
            this.generateKeywordDatabase = generateKeywordDatabase;
        }

        public TMPParameterTypeAttribute(string displayName, Type sharedBaseType, Type diskType, Type sceneType, bool generateKeywordDatabase = false)
        {
            this.displayName = displayName;
            this.diskType = diskType;
            this.sceneType = sceneType;
            this.sharedBaseType = sharedBaseType;
            this.generateKeywordDatabase = generateKeywordDatabase;
        }
    }
}