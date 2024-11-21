using System;

namespace TMPEffects.ParameterUtilityGenerator.Attributes
{
    // TODO Update link once docs updated
    /// <summary>
    /// Turn this type into a valid parameter type for use with AutoParameters.<br/>
    /// To learn more about AutoParameters and creating your own types, read <see href="https://tmpeffects.luca3317.dev/plugins/autoparameters.html">HERE</see>
    /// </summary>
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