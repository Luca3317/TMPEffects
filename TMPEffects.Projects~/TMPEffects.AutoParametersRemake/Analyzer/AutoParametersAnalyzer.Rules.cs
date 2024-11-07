using System.Collections.Immutable;
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
                    Rule_2,
                    Rule_3,
                    Rule_4 /*, Rule_1_0*/,
                    Rule_5,
                    Rule_1_1,
                    Rule_1_2,
                    Rule_1_3,
                    Rule_1_4,
                    Rule_1_5,
                    Rule_1_7,
                    Rule_2_0,
                    Rule_2_1,
                    Rule_2_2,
                    Rule_2_3,
                    Rule_2_4,
                    Rule___
                );
            }
        }
        
        #region AutoParametersAttribute rules

        public const string DiagnosticId_0 = "TMPAP000";

        private static readonly LocalizableString Title_0 =
            "Types decorated with " + Strings.AutoParametersAttributeName + " must be partial";

        private static readonly LocalizableString MessageFormat_0 =
            "Type {0} decorated with " + Strings.AutoParametersAttributeName + " is not partial";

        private const string Category_0 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_0 = new DiagnosticDescriptor(DiagnosticId_0, Title_0,
            MessageFormat_0, Category_0, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_1 = "TMPAP001";

        private static readonly LocalizableString Title_1 = "Types decorated with " +
                                                            Strings.AutoParametersAttributeName +
                                                            " must not implement SetParameters(object, IDictionary<string, string>)";

        private static readonly LocalizableString MessageFormat_1 = "Type {0} decorated with " +
                                                                    Strings.AutoParametersAttributeName +
                                                                    " implements SetParameters(object, IDictionary<string, string>)";

        private const string Category_1 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1 = new DiagnosticDescriptor(DiagnosticId_1, Title_1,
            MessageFormat_1, Category_1, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_2 = "TMPAP002";

        private static readonly LocalizableString Title_2 = "Types decorated with " +
                                                            Strings.AutoParametersAttributeName +
                                                            " must not implement ValidateParameters(IDictionary<string, string>)";

        private static readonly LocalizableString MessageFormat_2 = "Type {0} decorated with " +
                                                                    Strings.AutoParametersAttributeName +
                                                                    " implements ValidateParameters(IDictionary<string, string>)";

        private const string Category_2 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_2 = new DiagnosticDescriptor(DiagnosticId_2, Title_2,
            MessageFormat_2, Category_2, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_3 = "TMPAP003";

        private static readonly LocalizableString Title_3 = "Types decorated with " +
                                                            Strings.AutoParametersAttributeName +
                                                            " must not implement GetNewCustomData()";

        private static readonly LocalizableString MessageFormat_3 = "Type {0} decorated with " +
                                                                    Strings.AutoParametersAttributeName +
                                                                    " implements GetNewCustomData()";

        private const string Category_3 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_3 = new DiagnosticDescriptor(DiagnosticId_3, Title_3,
            MessageFormat_3, Category_3, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_4 = "TMPAP004";
        private static readonly LocalizableString Title_4 = "Direct reference to AutoParameter field in Animate method";

        private static readonly LocalizableString MessageFormat_4 =
            "Direct reference to AutoParameter field {0} in Animate method; did you mean to use the {1} field of your AutoParameterStorage?";

        private const string Category_4 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_4 = new DiagnosticDescriptor(DiagnosticId_4, Title_4,
            MessageFormat_4, Category_4, DiagnosticSeverity.Warning, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_5 = "TMPAP005";

        private static readonly LocalizableString Title_5 = "Types decorated with " +
                                                            Strings.AutoParameterAttributeName +
                                                            " must implement private partial void Animate()";

        private static readonly LocalizableString MessageFormat_5 =
            "Type {0} decorated with " + Strings.AutoParameterAttributeName +
            " does not implement private partial void Animate()";

        private const string Category_5 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_5 = new DiagnosticDescriptor(DiagnosticId_5, Title_5,
            MessageFormat_5, Category_5, DiagnosticSeverity.Warning, isEnabledByDefault: true);
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

        public const string DiagnosticId___ = "AutoParametersException";
        private static readonly LocalizableString Title___ = "AutoParametersException";
        private static readonly LocalizableString MessageFormat___ = "{0}";
        private const string Category___ = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule___ = new DiagnosticDescriptor(DiagnosticId___, Title___,
            MessageFormat___, Category___, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
    }
}