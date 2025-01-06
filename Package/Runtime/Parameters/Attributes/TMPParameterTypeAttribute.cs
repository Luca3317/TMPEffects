using System;

namespace TMPEffects.Parameters.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class TMPParameterTypeAttribute : Attribute
    {
        private bool generateKeywordDatabase;
        private string displayName;
        private Type sceneType;
        private Type diskType;

        public TMPParameterTypeAttribute(string displayName)
        {
            this.displayName = displayName;
            // this.generateKeywordDatabase = generateKeywordDatabase;
        }

        public TMPParameterTypeAttribute(
            string displayName,
            Type diskType,
            Type sceneType)
        {
            this.displayName = displayName;
            this.diskType = diskType;
            this.sceneType = sceneType;
            // this.generateKeywordDatabase = generateKeywordDatabase;
        }
        
        internal TMPParameterTypeAttribute(string displayName, bool generateKeywordDatabase)
        {
            this.displayName = displayName;
            this.generateKeywordDatabase = generateKeywordDatabase;
        }

        internal TMPParameterTypeAttribute(
            string displayName,
            Type diskType,
            Type sceneType,
            bool generateKeywordDatabase)
        {
            this.displayName = displayName;
            this.diskType = diskType;
            this.sceneType = sceneType;
            this.generateKeywordDatabase = generateKeywordDatabase;
        }
    }
}