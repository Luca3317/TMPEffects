using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using TMPEffects.AutoParameters.Generator;
using TMPEffects.StringLibrary;

namespace TMPEffects.ParameterUtilityGenerator
{
    [Generator]
    public class ParameterTypeGenerator : ISourceGenerator
    {
        public const string DiagnosticId___ = "DebuggingError2";
        private static readonly LocalizableString Title___ = "Debugerror";
        private static readonly LocalizableString MessageFormat___ = "{0}";
        private const string Category___ = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule___ = new DiagnosticDescriptor(DiagnosticId___, Title___,
            MessageFormat___, Category___, DiagnosticSeverity.Warning, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ParameterTypeAttributeSyntaxReceiver("TMPParameterType"));
        }

        private static readonly SymbolDisplayFormat attrDisplayFormat = new SymbolDisplayFormat(
            SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
            SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType
        );

        public void Execute(GeneratorExecutionContext context)
        {
            Diagnostic d1 = Diagnostic.Create(Rule___, context.Compilation.SyntaxTrees.First().GetRoot().GetLocation(),
                "XYZ I AM RUNNING");
            context.ReportDiagnostic(d1);

            ParameterTypeAttributeSyntaxReceiver receiver =
                context.SyntaxReceiver as ParameterTypeAttributeSyntaxReceiver;
            if (receiver == null) return;

            d1 = Diagnostic.Create(Rule___, context.Compilation.SyntaxTrees.First().GetRoot().GetLocation(),
                "XYZ I AM RUNNING 2: " + receiver.AttributeSyntaxes.Count);
            context.ReportDiagnostic(d1);

            foreach (var attribute in receiver.AttributeSyntaxes)
            {
                SemanticModel model = context.Compilation.GetSemanticModel(attribute.SyntaxTree);
                IMethodSymbol symbol = ModelExtensions.GetSymbolInfo(model, attribute).Symbol as IMethodSymbol;

                // Check whether attribute actually is the correct attribute
                List<string> displayNames = new List<string>();

                if (symbol.ContainingType.ToDisplayString() == Strings.TMPParameterTypeName)
                {
                    d1 = Diagnostic.Create(Rule___, attribute.GetLocation(),
                        "XYZ I AM RUNNING 2.8");
                    context.ReportDiagnostic(d1);

                    try
                    {
                        d1 = Diagnostic.Create(Rule___, attribute.GetLocation(),
                            "XYZ I AM RUNNING 2.9");
                        context.ReportDiagnostic(d1);

                        CreateParameterType(context, model, attribute, symbol, displayNames);

                        d1 = Diagnostic.Create(Rule___, attribute.GetLocation(),
                            "XYZ I AM RUNNING 2.95");
                        context.ReportDiagnostic(d1);
                    }
                    catch (System.Exception ex)
                    {
                        Diagnostic d = Diagnostic.Create(Rule___, attribute.GetLocation(),
                            "Failed to create ParameterType on " +
                            attribute.Ancestors().OfType<TypeDeclarationSyntax>().First().Identifier.Text + ". " +
                            "This is most likely a bug -- feel free to open an issue or a pull request with your fix on " +
                            "https://github.com/Luca3317/TMPEffects");
                        context.ReportDiagnostic(d);
                    }
                }
            }
        }

        private void CreateParameterType(GeneratorExecutionContext context, SemanticModel model,
            AttributeSyntax attrSyntax, IMethodSymbol attrSymbol, List<string> displayNames)
        {
#if DEBUG
            context.ReportDiagnostic(Diagnostic.Create(Rule___, attrSyntax.GetLocation(),
                "Attempting to create ParameterType"));
#endif
            ParameterTypeData ptd = GetParameterTypeData(attrSyntax, context, model);
#if DEBUG
            context.ReportDiagnostic(Diagnostic.Create(Rule___, attrSyntax.GetLocation(),
                "Successfully got all parameter types"));
#endif

            var typeDecl = attrSyntax.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();

            if (typeDecl == null)
            {
#if DEBUG
                context.ReportDiagnostic(Diagnostic.Create(Rule___, attrSyntax.GetLocation(),
                    "Could not get a type declaration in ancestors"));
#endif
                return;
            }

            var typeSymbol = model.GetDeclaredSymbol(typeDecl);

            // Create all necessary static functions on the type
            CreateHasNonTryGet(typeDecl, typeSymbol, ptd, context);

            // Create the keyword database stuff, if specified
            if (ptd.GenerateKeywordDatabase)
                CreateKeywordDatabases(typeDecl, typeSymbol, ptd, context);
        }

        private void CreateKeywordDatabases(TypeDeclarationSyntax typeDecl, INamedTypeSymbol typeSymbol,
            ParameterTypeData ptd, GeneratorExecutionContext context)
        {
            string code = $@"using UnityEngine;
using TMPEffects.SerializedCollections;

namespace TMPEffects.Databases
{{
    public partial interface ITMPKeywordDatabase
    {{
        public bool TryGet{ptd.DisplayName}(string str, out {ptd.SharedTypeName} result);
    }}
}}
";
            var source = SourceText.From(code, Encoding.UTF8);
            context.AddSource($"ITMPKeywordDatabase.{ptd.DisplayName}.g.cs", source);

            code = $@"using UnityEngine;
using TMPEffects.SerializedCollections;

namespace TMPEffects.Databases
{{
    public partial class TMPKeywordDatabase
    {{
        public bool TryGet{ptd.DisplayName}(string str, out {ptd.SharedTypeName} result)
        {{
            {(ptd.SharedTypeName == ptd.DiskTypeName ? $"return {ptd.DisplayName}Dict.TryGetValue(str, out result);" :
                @"bool success = {ptd.DisplayName}Dict.TryGetValue(str, out var tmp);
            result = tmp;
            return success;")}
        }}

        [SerializeField, SerializedDictionary(""Keyword"", ""{ptd.DisplayName}"")]
        private SerializedDictionary<string, {ptd.DiskTypeName}> {ptd.DisplayName}Dict = new SerializedDictionary<string, {ptd.SharedTypeName}>();
    }}
}}
";
            source = SourceText.From(code, Encoding.UTF8);
            context.AddSource($"TMPSceneKeywordDatabase.{ptd.DisplayName}.g.cs", source);

            code = $@"using UnityEngine;
using TMPEffects.SerializedCollections;

namespace TMPEffects.Databases
{{
    public partial class TMPSceneKeywordDatabase
    {{
        public bool TryGet{ptd.DisplayName}(string str, out {ptd.SharedTypeName} result)
        {{
            {(ptd.SharedTypeName == ptd.SceneTypeName ? $"return {ptd.DisplayName}Dict.TryGetValue(str, out result);" :
                @"bool success = {ptd.DisplayName}Dict.TryGetValue(str, out var tmp);
            result = tmp;
            return success;")}
        }}

        [SerializeField, SerializedDictionary(""Keyword"", ""{ptd.DisplayName}"")]
        private SerializedDictionary<string, {ptd.SceneTypeName}> {ptd.DisplayName}Dict = new SerializedDictionary<string, {ptd.SharedTypeName}>();
    }}
}}
";
            source = SourceText.From(code, Encoding.UTF8);
            context.AddSource($"TMPSceneKeywordDatabase.{ptd.DisplayName}.g.cs", source);
        }

        private void CreateHasNonTryGet(TypeDeclarationSyntax typeDecl, INamedTypeSymbol typeSymbol,
            ParameterTypeData ptd, GeneratorExecutionContext context)
        {
            string t = typeDecl.Modifiers.ToString() + " " + typeDecl.Keyword.ToString() + " " +
                       typeDecl.Identifier.Text;

            string code = $@"using System;
using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Databases;
using TMPEffects.Parameters;
using static TMPEffects.Parameters.ParameterTypes;
using static TMPEffects.Parameters.ParameterUtility;

    // <auto-generated />
    // This class is auto-generated; changing it might break [AutoParameters] and existing animations + commands
    {t}
    {{
        public static partial bool StringTo{ptd.DisplayName}(string str, out {ptd.SharedTypeName} result, ITMPKeywordDatabase db = null);
";

            code = ParameterUtilityGenerator.HandleHasParameter(typeSymbol, (ptd.SharedTypeName, ptd.DisplayName),
                code);
            code = ParameterUtilityGenerator.HandleHasNonParameter(typeSymbol, (ptd.SharedTypeName, ptd.DisplayName),
                code);
            code = ParameterUtilityGenerator.HandleTryGetParameter(typeSymbol, (ptd.SharedTypeName, ptd.DisplayName),
                code);

            code += @"
    }";

            // Prepare and add source
            var source = SourceText.From(code, Encoding.UTF8);
            context.AddSource($"{typeDecl.Identifier.Text}.{ptd.DisplayName}.g.cs", source);
        }

        private ParameterTypeData GetParameterTypeData(AttributeSyntax attrSyntax, GeneratorExecutionContext context,
            SemanticModel model)
        {
            // Get the attribute arguments
            var arguments = attrSyntax.ArgumentList.Arguments;

            AttributeArgumentSyntax displayNameArg;
            AttributeArgumentSyntax typeArg;
            AttributeArgumentSyntax sceneTypeArg;
            AttributeArgumentSyntax diskTypeArg;
            AttributeArgumentSyntax sharedBaseTypeArg;
            AttributeArgumentSyntax generateKeywordDatabaseArg = null;


            ParameterTypeData parameterTypeData = new ParameterTypeData();
            if (arguments.Count <= 3)
            {
                displayNameArg = arguments[0];
                typeArg = arguments[1];

                if (arguments.Count > 2)
                    generateKeywordDatabaseArg = arguments[2];

                var constval = model.GetConstantValue(displayNameArg.Expression);
                if (constval.HasValue && constval.Value is string str)
                {
                    parameterTypeData.DisplayName = str;
                }
                else
                {
#if DEBUG
                    context.ReportDiagnostic(Diagnostic.Create(Rule___, attrSyntax.GetLocation(),
                        "DisplayName argument not set"));
#endif
                    throw new System.InvalidOperationException();
                }

                if (typeArg.Expression is TypeOfExpressionSyntax typeOfExpressionSyntax)
                {
                    var ti = ModelExtensions.GetTypeInfo(model, typeOfExpressionSyntax.Type);
                    if (ti.Type != null)
                    {
                        parameterTypeData.SharedTypeName = ti.Type.ToDisplayString();
                    }
                    else
                    {
#if DEBUG
                        context.ReportDiagnostic(Diagnostic.Create(Rule___, attrSyntax.GetLocation(),
                            "TypeArg argument not set"));
#endif
                        throw new System.InvalidOperationException();
                    }
                }

                if (generateKeywordDatabaseArg != null)
                {
                    constval = model.GetConstantValue(generateKeywordDatabaseArg.Expression);
                    if (constval.HasValue && constval.Value is bool bl)
                    {
                        parameterTypeData.GenerateKeywordDatabase = bl;
                    }
                    else
                    {
#if DEBUG
                        context.ReportDiagnostic(Diagnostic.Create(Rule___, attrSyntax.GetLocation(),
                            "GenerateKeywordDatabase argument not set"));
#endif
                        throw new System.InvalidOperationException();
                    }
                }
            }
            else
            {
                displayNameArg = arguments[0];
                typeArg = arguments[1];
                sceneTypeArg = arguments[2];
                diskTypeArg = arguments[3];

                if (arguments.Count > 4)
                    generateKeywordDatabaseArg = arguments[4];

                var constval = model.GetConstantValue(displayNameArg.Expression);
                if (constval.HasValue && constval.Value is string str)
                {
                    parameterTypeData.DisplayName = str;
                }
                else
                {
#if DEBUG
                    context.ReportDiagnostic(Diagnostic.Create(Rule___, attrSyntax.GetLocation(),
                        "DisplayName argument not set"));
#endif
                    throw new System.InvalidOperationException();
                }

                if (typeArg.Expression is TypeOfExpressionSyntax typeOfExpressionSyntax)
                {
                    var ti = ModelExtensions.GetTypeInfo(model, typeOfExpressionSyntax.Type);
                    if (ti.Type != null)
                    {
                        parameterTypeData.SharedTypeName = ti.Type.ToDisplayString();
                    }
                    else
                    {
#if DEBUG
                        context.ReportDiagnostic(Diagnostic.Create(Rule___, attrSyntax.GetLocation(),
                            "TypeArg argument not set"));
#endif
                        throw new System.InvalidOperationException();
                    }
                }

                if (sceneTypeArg.Expression is TypeOfExpressionSyntax typeOfExpressionSyntax2)
                {
                    var ti = ModelExtensions.GetTypeInfo(model, typeOfExpressionSyntax2.Type);
                    if (ti.Type != null)
                    {
                        parameterTypeData.SceneTypeName = ti.Type.ToDisplayString();
                    }
                    else
                    {
#if DEBUG
                        context.ReportDiagnostic(Diagnostic.Create(Rule___, attrSyntax.GetLocation(),
                            "SceneTypeArg argument not set"));
#endif
                        throw new System.InvalidOperationException();
                    }
                }

                if (diskTypeArg.Expression is TypeOfExpressionSyntax typeOfExpressionSyntax3)
                {
                    var ti = ModelExtensions.GetTypeInfo(model, typeOfExpressionSyntax3.Type);
                    if (ti.Type != null)
                    {
                        parameterTypeData.DiskTypeName = ti.Type.ToDisplayString();
                    }
                    else
                    {
#if DEBUG
                        context.ReportDiagnostic(Diagnostic.Create(Rule___, attrSyntax.GetLocation(),
                            "DiskTypeArg argument not set"));
#endif
                        throw new System.InvalidOperationException();
                    }
                }

                if (generateKeywordDatabaseArg != null)
                {
                    constval = model.GetConstantValue(generateKeywordDatabaseArg.Expression);
                    if (constval.HasValue && constval.Value is bool bl)
                    {
                        parameterTypeData.GenerateKeywordDatabase = bl;
                    }
                    else
                    {
#if DEBUG
                        context.ReportDiagnostic(Diagnostic.Create(Rule___, attrSyntax.GetLocation(),
                            "GenerateKeywordDatabase argument not set"));
#endif
                        throw new System.InvalidOperationException();
                    }
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule___, attrSyntax.GetLocation(),
                "Made it to the end! DisplayName: " + parameterTypeData.DisplayName +
                " SceneTypeName: " + parameterTypeData.SceneTypeName +
                " DiskTypeName: " + parameterTypeData.DiskTypeName +
                " SharedTypeName: " + parameterTypeData.SharedTypeName +
                " GenKeyword: " + parameterTypeData.GenerateKeywordDatabase));

            return parameterTypeData;
        }

        public struct ParameterTypeData
        {
            public string DisplayName;
            public string SceneTypeName;
            public string DiskTypeName;
            public string SharedTypeName;
            public bool GenerateKeywordDatabase;
        }
    }
}