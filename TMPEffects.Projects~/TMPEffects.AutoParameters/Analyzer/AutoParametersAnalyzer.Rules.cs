using System.Collections.Immutable;
using System.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TMPEffects.StringLibrary;

namespace TMPEffects.AutoParameters.Analyzer
{
    public partial class AutoParametersAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create
                (
                    Rule_0,
                    Rule_1,
                    
                    // Animation Specific
                    Rule_1001,
                    Rule_1002,
                    Rule_1003,
                    Rule_1004,
                    Rule_1005,
                
                    // Command Specific
                    Rule_2001,
                    Rule_2002,
                    // Rule_2003,
                    Rule_2004,
                    Rule_2005,
                
                    // General ones
                    Rule_1_1,
                    Rule_1_2,
                    Rule_1_3,
                    Rule_1_4,
                    Rule_1_5,
                    Rule_1_7,
                    Rule_1_8,
                    Rule_2_0,
                    Rule_2_1,
                    Rule_2_2,
                    Rule_2_3,
                    Rule_2_4,
                    Rule___,
                    
                    Rule_900,
                    Rule_901,
                    Rule_902,
                    Rule_903,
                    Rule_904
                );
            }
        }


        #region ITMPCommand specific

        // ReSharper disable InconsistentNaming
        public const string DiagnosticId_2001 = "TMPAP2001";

        private static readonly LocalizableString Title_2001 = "Types decorated with " +
                                                               Strings.AutoParametersAttributeName +
                                                               $" must not implement SetParameters(IDictionary<string, string>, ICommandContext)";

        private static readonly LocalizableString MessageFormat_2001 = "Type {0} decorated with " +
                                                                       Strings.AutoParametersAttributeName +
                                                                       " implements SetParameters(IDictionary<string, string>, ICommandContext)";

        private const string Category_2001 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_2001 = new DiagnosticDescriptor(DiagnosticId_2001, Title_2001,
            MessageFormat_2001, Category_2001, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_2002 = "TMPAP2002";

        private static readonly LocalizableString Title_2002 = "Types decorated with " +
                                                               Strings.AutoParametersAttributeName +
                                                               " must not implement ValidateParameters(IDictionary<string, string>, IWriterContext)";

        private static readonly LocalizableString MessageFormat_2002 = "Type {0} decorated with " +
                                                                       Strings.AutoParametersAttributeName +
                                                                       " implements ValidateParameters(IDictionary<string, string>, IWriterContext)";

        private const string Category_2002 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_2002 = new DiagnosticDescriptor(DiagnosticId_2002, Title_2002,
            MessageFormat_2002, Category_2002, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking


        public const string DiagnosticId_2004 = "TMPAP2004";

        private static readonly LocalizableString Title_2004 =
            "Direct reference to AutoParameter field in ExecuteCommand method";

        private static readonly LocalizableString MessageFormat_2004 =
            "Direct reference to AutoParameter field {0} in ExecuteCommand method; did you mean to use the {1} field of your AutoParameterStorage?";

        private const string Category_2004 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_2004 = new DiagnosticDescriptor(DiagnosticId_2004, Title_2004,
            MessageFormat_2004, Category_2004, DiagnosticSeverity.Warning, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_2005 = "TMPAP2005";

        private static readonly LocalizableString Title_2005 = "Types decorated with " +
                                                               Strings.AutoParameterAttributeName +
                                                               " must implement private partial void ExecuteCommand(IDictionary<string, string>, AutoParametersData, ICommandContext)";

        private static readonly LocalizableString MessageFormat_2005 =
            "Type {0} decorated with " + Strings.AutoParameterAttributeName +
            " does not implement private partial void ExecuteCommand(IDictionary<string, string>, AutoParametersData, ICommandContext)";

        private const string Category_2005 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_2005 = new DiagnosticDescriptor(DiagnosticId_2005, Title_2005,
            MessageFormat_2005, Category_2005, DiagnosticSeverity.Warning, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        #endregion

        #region ITMPAnimation specific

        public const string DiagnosticId_1001 = "TMPAP1001";

        private static readonly LocalizableString Title_1001 = "Types decorated with " +
                                                               Strings.AutoParametersAttributeName +
                                                               " must not implement SetParameters(object, IDictionary<string, string>)";

        private static readonly LocalizableString MessageFormat_1001 = "Type {0} decorated with " +
                                                                       Strings.AutoParametersAttributeName +
                                                                       " implements SetParameters(object, IDictionary<string, string>)";

        private const string Category_1001 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1001 = new DiagnosticDescriptor(DiagnosticId_1001, Title_1001,
            MessageFormat_1001, Category_1001, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_1002 = "TMPAP1002";

        private static readonly LocalizableString Title_1002 = "Types decorated with " +
                                                               Strings.AutoParametersAttributeName +
                                                               " must not implement ValidateParameters(IDictionary<string, string>, IAnimatorContext)";

        private static readonly LocalizableString MessageFormat_1002 = "Type {0} decorated with " +
                                                                       Strings.AutoParametersAttributeName +
                                                                       " implements ValidateParameters(IDictionary<string, string>, IAnimatorContext)";

        private const string Category_1002 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1002 = new DiagnosticDescriptor(DiagnosticId_1002, Title_1002,
            MessageFormat_1002, Category_1002, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_1003 = "TMPAP1003";

        private static readonly LocalizableString Title_1003 = "Types decorated with " +
                                                            Strings.AutoParametersAttributeName +
                                                            " must not implement GetNewCustomData()";

        private static readonly LocalizableString MessageFormat_1003 = "Type {0} decorated with " +
                                                                    Strings.AutoParametersAttributeName +
                                                                    " implements GetNewCustomData()";

        private const string Category_1003 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1003 = new DiagnosticDescriptor(DiagnosticId_1003, Title_1003,
            MessageFormat_1003, Category_1003, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_1004 = "TMPAP1004";
        private static readonly LocalizableString Title_1004 = "Direct reference to AutoParameter field in Animate method";

        private static readonly LocalizableString MessageFormat_1004 =
            "Direct reference to AutoParameter field {0} in Animate method; did you mean to use the {1} field of your AutoParameterStorage?";

        private const string Category_1004 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1004 = new DiagnosticDescriptor(DiagnosticId_1004, Title_1004,
            MessageFormat_1004, Category_1004, DiagnosticSeverity.Warning, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_1005 = "TMPAP1005";

        private static readonly LocalizableString Title_1005 = "Types decorated with " +
                                                               Strings.AutoParameterAttributeName +
                                                               " must implement private partial void Animate()";

        private static readonly LocalizableString MessageFormat_1005 =
            "Type {0} decorated with " + Strings.AutoParameterAttributeName +
            " does not implement private partial void Animate()";

        private const string Category_1005 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1005 = new DiagnosticDescriptor(DiagnosticId_1005, Title_1005,
            MessageFormat_1005, Category_1005, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
        
        #endregion


        #region AutoParametersAttribute rules

        public const string DiagnosticId_0 = "TMPAP000";

        private static readonly LocalizableString Title_0 =
            "Types decorated with " + Strings.AutoParametersAttribute + " must be partial";

        private static readonly LocalizableString MessageFormat_0 =
            "Type {0} decorated with " + Strings.AutoParametersAttribute + " is not partial";

        private const string Category_0 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_0 = new DiagnosticDescriptor(DiagnosticId_0, Title_0,
            MessageFormat_0, Category_0, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
        
        public const string DiagnosticId_1 = "TMPAP001";

        private static readonly LocalizableString Title_1 =
            "Types decorated with " + Strings.AutoParametersAttribute + " must not be nested";

        private static readonly LocalizableString MessageFormat_1 =
            "Type {0} decorated with " + Strings.AutoParametersAttribute + " must not be nested";

        private const string Category_1 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1 = new DiagnosticDescriptor(DiagnosticId_1, Title_1,
            MessageFormat_1, Category_1, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        #endregion

        #region AutoParameterAttribute rules

        //        public const string DiagnosticId_1_0 = "TMPAP100";
        //        private static readonly LocalizableString Title_1_0 = "Types decorated with " + Constants.AutoParameterAttributeName + " should have unique name and aliases within the containing class";
        //        private static readonly LocalizableString MessageFormat_1_0 = "Type {0} decorated with " + Constants.AutoParameterAttributeName + " does not have unique name and aliases within the containing class";
        //        private const string Category_1_0 = "Usage";
        //#pragma warning disable RS2008 // Enable analyzer release tracking
        //        private static readonly DiagnosticDescriptor Rule_1_0 = new DiagnosticDescriptor(DiagnosticId_1_0, Title_1_0, MessageFormat_1_0, Category_1_0, DiagnosticSeverity.Warning, isEnabledByDefault: true);
        //#pragma warning restore RS2008 // Enable analyzer release tracking


        public const string DiagnosticId_1_1 = "TMPAP101";

        private static readonly LocalizableString Title_1_1 =
            "Invalid type of field decorated with " + Strings.AutoParameterAttributeName;

        private static readonly LocalizableString MessageFormat_1_1 =
            "Field {0} decorated with " + Strings.AutoParameterAttributeName + " has invalid type {1}";

        private const string Category_1_1 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1_1 = new DiagnosticDescriptor(DiagnosticId_1_1, Title_1_1,
            MessageFormat_1_1, Category_1_1, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_1_2 = "TMPAP102";

        private static readonly LocalizableString Title_1_2 =
            "Invalid type of field decorated with " + Strings.AutoParameterAttributeName;

        private static readonly LocalizableString MessageFormat_1_2 = "Field {0} decorated with " +
                                                                      Strings.AutoParameterAttributeName +
                                                                      " has invalid type {1}; You may decorate the field with " +
                                                                      Strings.AutoParameterBundleAttributeName +
                                                                      " instead";

        private const string Category_1_2 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1_2 = new DiagnosticDescriptor(DiagnosticId_1_2, Title_1_2,
            MessageFormat_1_2, Category_1_2, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_1_3 = "TMPAP103";

        private static readonly LocalizableString Title_1_3 =
            "Invalid type of field decorated with " + Strings.AutoParameterBundleAttributeName;

        private static readonly LocalizableString MessageFormat_1_3 = "Field {0} decorated with " +
                                                                      Strings.AutoParameterBundleAttributeName +
                                                                      " has invalid type {1}";

        private const string Category_1_3 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1_3 = new DiagnosticDescriptor(DiagnosticId_1_3, Title_1_3,
            MessageFormat_1_3, Category_1_3, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking        

        public const string DiagnosticId_1_4 = "TMPAP104";

        private static readonly LocalizableString Title_1_4 =
            "Invalid type of field decorated with " + Strings.AutoParameterBundleAttributeName;

        private static readonly LocalizableString MessageFormat_1_4 = "Field {0} decorated with " +
                                                                      Strings.AutoParameterBundleAttributeName +
                                                                      " has invalid type {1}; You may decorate the field with " +
                                                                      Strings.AutoParameterAttributeName + " instead";

        private const string Category_1_4 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1_4 = new DiagnosticDescriptor(DiagnosticId_1_4, Title_1_4,
            MessageFormat_1_4, Category_1_4, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking  

        public const string DiagnosticId_1_5 = "TMPAP105";

        private static readonly LocalizableString Title_1_5 = "Fields decorated with " +
                                                              Strings.AutoParameterAttributeName +
                                                              " must be contained within a type decorated with " +
                                                              Strings.AutoParametersAttributeName;

        private static readonly LocalizableString MessageFormat_1_5 = "Field {0} decorated with " +
                                                                      Strings.AutoParameterAttributeName +
                                                                      " is not contained in a type decorated with " +
                                                                      Strings.AutoParametersAttributeName;

        private const string Category_1_5 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1_5 = new DiagnosticDescriptor(DiagnosticId_1_5, Title_1_5,
            MessageFormat_1_5, Category_1_5, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking       

        public const string DiagnosticId_1_7 = "TMPAP107";

        private static readonly LocalizableString Title_1_7 =
            "Fields decorated with " + Strings.AutoParameterAttributeName + " should be serializable";

        private static readonly LocalizableString MessageFormat_1_7 = "Field {0} decorated with " +
                                                                      Strings.AutoParameterAttributeName +
                                                                      " is not serializable; consider making it public or decorating it with " +
                                                                      Strings.SerializeFieldAttributeName;

        private const string Category_1_7 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1_7 = new DiagnosticDescriptor(DiagnosticId_1_7, Title_1_7,
            MessageFormat_1_7, Category_1_7, DiagnosticSeverity.Warning, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
        
        
        public const string DiagnosticId_1_8 = "TMPAP108";

        private static readonly LocalizableString Title_1_8 =
            "AutoParameters that use \"\" as alias must be required";

        private static readonly LocalizableString MessageFormat_1_8 = "Field {0} decorated with " +
                                                                      Strings.AutoParameterAttributeName +
                                                                      " is not required, but uses \"\" as alias";

        private const string Category_1_8 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1_8 = new DiagnosticDescriptor(DiagnosticId_1_8, Title_1_8,
            MessageFormat_1_8, Category_1_8, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        #endregion

        #region AutoParametersStorageAttribute rules

        public const string DiagnosticId_2_0 = "TMPAP200";

        private static readonly LocalizableString Title_2_0 =
            "Types decorated with " + Strings.AutoParametersStorageAttributeName + " must be partial";

        private static readonly LocalizableString MessageFormat_2_0 = "Type {0} decorated with " +
                                                                      Strings.AutoParametersStorageAttributeName +
                                                                      " is not partial";

        private const string Category_2_0 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_2_0 = new DiagnosticDescriptor(DiagnosticId_2_0, Title_2_0,
            MessageFormat_2_0, Category_2_0, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_2_1 = "TMPAP201";

        private static readonly LocalizableString Title_2_1 = "Types decorated with " +
                                                              Strings.AutoParametersStorageAttributeName +
                                                              " must not have any fields with identical name to a field decorated with " +
                                                              Strings.AutoParameterAttributeName;

        private static readonly LocalizableString MessageFormat_2_1 = "Type {0} decorated with " +
                                                                      Strings.AutoParametersStorageAttributeName +
                                                                      " has field with identical name to a field decorated with " +
                                                                      Strings.AutoParameterAttributeName + ": {1}";

        private const string Category_2_1 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_2_1 = new DiagnosticDescriptor(DiagnosticId_2_1, Title_2_1,
            MessageFormat_2_1, Category_2_1, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_2_2 = "TMPAP202";

        private static readonly LocalizableString Title_2_2 = "Types decorated with " +
                                                              Strings.AutoParametersStorageAttributeName +
                                                              " must be contained within a type decorated with " +
                                                              Strings.AutoParametersAttributeName;

        private static readonly LocalizableString MessageFormat_2_2 = "Type {0} decorated with " +
                                                                      Strings.AutoParametersStorageAttributeName +
                                                                      " is not contained within a type decorated with " +
                                                                      Strings.AutoParametersAttributeName;

        private const string Category_2_2 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_2_2 = new DiagnosticDescriptor(DiagnosticId_2_2, Title_2_2,
            MessageFormat_2_2, Category_2_2, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_2_3 = "TMPAP203";

        private static readonly LocalizableString Title_2_3 = "There may only be one nested type decorated with " +
                                                              Strings.AutoParametersStorageAttributeName +
                                                              " contained in any given type";

        private static readonly LocalizableString MessageFormat_2_3 =
            "Type {0} contains multiple types decorated with " + Strings.AutoParametersStorageAttributeName;

        private const string Category_2_3 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_2_3 = new DiagnosticDescriptor(DiagnosticId_2_3, Title_2_3,
            MessageFormat_2_3, Category_2_3, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_2_4 = "TMPAP204";

        private static readonly LocalizableString Title_2_4 = "Types decorated with " +
                                                              Strings.AutoParametersStorageAttributeName +
                                                              " must have a default constructor (zero parameters)";

        private static readonly LocalizableString MessageFormat_2_4 = "Type {0} decorated with " +
                                                                      Strings.AutoParametersStorageAttributeName +
                                                                      " does not have a default constructor (zero parameters)";

        private const string Category_2_4 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_2_4 = new DiagnosticDescriptor(DiagnosticId_2_4, Title_2_4,
            MessageFormat_2_4, Category_2_4, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        #endregion
        
        
        public const string DiagnosticId_900 = "TMPAP900";

        private static readonly LocalizableString Title_900 = "ValidateParameters hook method not implemented";

        private static readonly LocalizableString MessageFormat_900 = "Consider implementing the ValidateParameters hook method, if you need custom logic";

        private const string Category_900 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_900 = new DiagnosticDescriptor(DiagnosticId_900, Title_900,
            MessageFormat_900, Category_900, DiagnosticSeverity.Info, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
        
        
        public const string DiagnosticId_901 = "TMPAP901";

        private static readonly LocalizableString Title_901 = "SetParameters hook method not implemented";

        private static readonly LocalizableString MessageFormat_901 = "Consider implementing the SetParameters hook method, if you need custom logic";

        private const string Category_901 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_901 = new DiagnosticDescriptor(DiagnosticId_901, Title_901,
            MessageFormat_901, Category_901, DiagnosticSeverity.Info, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
        
        
        public const string DiagnosticId_902 = "TMPAP902";

        private static readonly LocalizableString Title_902 = "GetNewCustomData hook method not implemented";

        private static readonly LocalizableString MessageFormat_902 = "Consider implementing the GetNewCustomData hook method, if you need custom logic";

        private const string Category_902 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_902 = new DiagnosticDescriptor(DiagnosticId_902, Title_902,
            MessageFormat_902, Category_902, DiagnosticSeverity.Info, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
        
        
        public const string DiagnosticId_903 = "TMPAP903";

        private static readonly LocalizableString Title_903 = "ValidateParameters hook method not implemented";

        private static readonly LocalizableString MessageFormat_903 = "Consider implementing the ValidateParameters hook method, if you need custom logic";

        private const string Category_903 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_903 = new DiagnosticDescriptor(DiagnosticId_903, Title_903,
            MessageFormat_903, Category_903, DiagnosticSeverity.Info, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
        
        
        public const string DiagnosticId_904 = "TMPAP904";

        private static readonly LocalizableString Title_904 = "SetParameters hook method not implemented";

        private static readonly LocalizableString MessageFormat_904 = "Consider implementing the SetParameters hook method, if you need custom logic";

        private const string Category_904 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_904 = new DiagnosticDescriptor(DiagnosticId_904, Title_904,
            MessageFormat_904, Category_904, DiagnosticSeverity.Info, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
        
        

        public const string DiagnosticId___ = "AutoParametersException";
        private static readonly LocalizableString Title___ = "AutoParametersException";
        private static readonly LocalizableString MessageFormat___ = "{0}";
        private const string Category___ = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule___ = new DiagnosticDescriptor(DiagnosticId___, Title___,
            MessageFormat___, Category___, DiagnosticSeverity.Warning, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
    }
}