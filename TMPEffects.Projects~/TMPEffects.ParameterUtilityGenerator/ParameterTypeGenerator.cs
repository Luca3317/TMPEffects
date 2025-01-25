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
            ParameterTypeAttributeSyntaxReceiver receiver =
                context.SyntaxReceiver as ParameterTypeAttributeSyntaxReceiver;
            if (receiver == null) return;
            
            foreach (var attribute in receiver.AttributeSyntaxes)
            {
                SemanticModel model = context.Compilation.GetSemanticModel(attribute.SyntaxTree);
                IMethodSymbol symbol = ModelExtensions.GetSymbolInfo(model, attribute).Symbol as IMethodSymbol;

                // Check whether attribute actually is the correct attribute
                List<string> displayNames = new List<string>();

                if (symbol.ContainingType.ToDisplayString() == Strings.TMPParameterTypeAttributeName)
                {
                    try
                    {
                        CreateParameterType(context, model, attribute, symbol, displayNames);
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
            Utility.ParameterTypeData ptd = Utility.GetParameterTypeData(attrSyntax, model);
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

#if DEBUG
            context.ReportDiagnostic(Diagnostic.Create(Rule___, attrSyntax.GetLocation(),
                "Starting creation"));
#endif
            
            var typeSymbol = model.GetDeclaredSymbol(typeDecl);

            // Create all necessary static functions on the type
            CreateHasNonTryGet(typeDecl, typeSymbol, ptd, context);
            
#if DEBUG
            context.ReportDiagnostic(Diagnostic.Create(Rule___, attrSyntax.GetLocation(),
                "Created HasNonTryGet"));
#endif

            // Create the keyword database stuff, if specified
            if (ptd.GenerateKeywordDatabase)
            {
                CreateKeywordDatabases(typeDecl, typeSymbol, ptd, context);
#if DEBUG
                context.ReportDiagnostic(Diagnostic.Create(Rule___, attrSyntax.GetLocation(),
                    "Created Keyword database stuff"));
#endif
            }
        }

        private void CreateKeywordDatabases(TypeDeclarationSyntax typeDecl, INamedTypeSymbol typeSymbol,
            Utility.ParameterTypeData ptd, GeneratorExecutionContext context)
        {
            // Interface
            string code = $@"using UnityEngine;
using TMPEffects.SerializedCollections;

namespace TMPEffects.Databases
{{
    // This file was auto-generated by AutoParameters.
    // Changes to this file may cause incorrect behaviour and will be lost when the code is regenerated.
    // To learn more about AutoParameter, read <see href=""https://tmpeffects.luca3317.dev/manual/autoparameters.html"">HERE</see>.
    public partial interface ITMPKeywordDatabase
    {{
        public bool TryGet{ptd.DisplayName}(string str, out {ptd.SharedTypeName} result);
    }}
}}
";
            var source = SourceText.From(code, Encoding.UTF8);
            context.AddSource($"ITMPKeywordDatabase.{ptd.DisplayName}.g.cs", source);

            // TMPKeywordDatabaseBase
            code = $@"using UnityEngine;
using TMPEffects.SerializedCollections;

namespace TMPEffects.Databases
{{
    // This file was auto-generated by AutoParameters.
    // Changes to this file may cause incorrect behaviour and will be lost when the code is regenerated.
    // To learn more about AutoParameter, read <see href=""https://tmpeffects.luca3317.dev/manual/autoparameters.html"">HERE</see>.
    public abstract partial class TMPKeywordDatabaseBase
    {{
        public abstract bool TryGet{ptd.DisplayName}(string str, out {ptd.SharedTypeName} result);
    }}
}}
";
            source = SourceText.From(code, Encoding.UTF8);
            context.AddSource($"TMPKeywordDatabaseBase.{ptd.DisplayName}.g.cs", source);
            
            // TMPKeywordDatabase
            code = $@"using UnityEngine;
using TMPEffects.SerializedCollections;

namespace TMPEffects.Databases
{{
    // This file was auto-generated by AutoParameters.
    // Changes to this file may cause incorrect behaviour and will be lost when the code is regenerated.
    // To learn more about AutoParameter, read <see href=""https://tmpeffects.luca3317.dev/manual/autoparameters.html"">HERE</see>.
    public sealed partial class TMPKeywordDatabase
    {{
        public override bool TryGet{ptd.DisplayName}(string str, out {ptd.SharedTypeName} result)
        {{
            {(ptd.SharedTypeName == ptd.DiskTypeName ? $"return {ptd.DisplayName}Dict.TryGetValue(str, out result);" :
                $@"bool success = {ptd.DisplayName}Dict.TryGetValue(str, out var tmp);
            result = tmp;
            return success;")}
        }}

        [SerializeField, SerializedDictionary(""Keyword"", ""{ptd.DisplayName}"")]
        private SerializedDictionary<string, {ptd.DiskTypeName}> {ptd.DisplayName}Dict = new SerializedDictionary<string, {ptd.DiskTypeName}>();
    }}
}}
";
            source = SourceText.From(code, Encoding.UTF8);
            context.AddSource($"TMPKeywordDatabase.{ptd.DisplayName}.g.cs", source);

            // TMPSceneKeywordDatabaseBase
            code = $@"using UnityEngine;
using TMPEffects.SerializedCollections;

namespace TMPEffects.Databases
{{
    // This file was auto-generated by AutoParameters.
    // Changes to this file may cause incorrect behaviour and will be lost when the code is regenerated.
    // To learn more about AutoParameter, read <see href=""https://tmpeffects.luca3317.dev/manual/autoparameters.html"">HERE</see>.
    public partial class TMPSceneKeywordDatabaseBase
    {{
        public abstract bool TryGet{ptd.DisplayName}(string str, out {ptd.SharedTypeName} result);
    }}
}}
";
            source = SourceText.From(code, Encoding.UTF8);
            context.AddSource($"TMPSceneKeywordDatabaseBase.{ptd.DisplayName}.g.cs", source);
            
            // TMPSceneKeywordDatabase
            code = $@"using UnityEngine;
using TMPEffects.SerializedCollections;

namespace TMPEffects.Databases
{{
    // This file was auto-generated by AutoParameters.
    // Changes to this file may cause incorrect behaviour and will be lost when the code is regenerated.
    // To learn more about AutoParameter, read <see href=""https://tmpeffects.luca3317.dev/manual/autoparameters.html"">HERE</see>.
    public sealed partial class TMPSceneKeywordDatabase
    {{
        public override bool TryGet{ptd.DisplayName}(string str, out {ptd.SharedTypeName} result)
        {{
            {(ptd.SharedTypeName == ptd.SceneTypeName ? $"return {ptd.DisplayName}Dict.TryGetValue(str, out result);" :
                $@"bool success = {ptd.DisplayName}Dict.TryGetValue(str, out var tmp);
            result = tmp;
            return success;")}
        }}

        [SerializeField, SerializedDictionary(""Keyword"", ""{ptd.DisplayName}"")]
        private SerializedDictionary<string, {ptd.SceneTypeName}> {ptd.DisplayName}Dict = new SerializedDictionary<string, {ptd.SceneTypeName}>();
    }}
}}
";
            source = SourceText.From(code, Encoding.UTF8);
            context.AddSource($"TMPSceneKeywordDatabase.{ptd.DisplayName}.g.cs", source);

            // CompositeTMPKeywordDatabase
            code = $@"using UnityEngine;
using TMPEffects.SerializedCollections;

namespace TMPEffects.Databases
{{
    // This file was auto-generated by AutoParameters.
    // Changes to this file may cause incorrect behaviour and will be lost when the code is regenerated.
    // To learn more about AutoParameter, read <see href=""https://tmpeffects.luca3317.dev/manual/autoparameters.html"">HERE</see>.
    public sealed partial class CompositeTMPKeywordDatabase
    {{
        public bool TryGet{ptd.DisplayName}(string str, out {ptd.SharedTypeName} result)
        {{
            for (int i = 0; i < databases.Length; i++)
            {{
                var db = databases[i];
                if (db != null)
                    if (db.TryGet{ptd.DisplayName}(str, out result))
                        return true;
            }}

            result = default;
            return false;
        }}
    }}
}}
";
            source = SourceText.From(code, Encoding.UTF8);
            context.AddSource($"CompositeTMPKeywordDatabase.{ptd.DisplayName}.g.cs", source);
        }

        private void CreateHasNonTryGet(TypeDeclarationSyntax typeDecl, INamedTypeSymbol typeSymbol,
            Utility.ParameterTypeData ptd, GeneratorExecutionContext context)
        {
            string t = typeDecl.Modifiers.ToString() + " " + typeDecl.Keyword.ToString() + " " +
                       typeDecl.Identifier.Text;

            string code;
            if (typeSymbol.ContainingNamespace.IsGlobalNamespace)
            {
                code = $@"using System;
using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Databases;
using TMPEffects.Parameters;
using static TMPEffects.Parameters.TMPParameterTypes;
using static TMPEffects.Parameters.TMPParameterUtility;

// This file was auto-generated by AutoParameters.
// Changes to this file may cause incorrect behaviour and will be lost when the code is regenerated.
// To learn more about AutoParameter, read <see href=""https://tmpeffects.luca3317.dev/manual/autoparameters.html"">HERE</see>.
{t}
{{
    public static partial bool StringTo{ptd.DisplayName}(string str, out {ptd.SharedTypeName} result, ITMPKeywordDatabase db);
";
                code = ParameterUtilityGenerator.HandleHasParameter(typeSymbol, (ptd.SharedTypeName, ptd.DisplayName),
                    code);
                code = ParameterUtilityGenerator.HandleHasNonParameter(typeSymbol,
                    (ptd.SharedTypeName, ptd.DisplayName),
                    code);
                code = ParameterUtilityGenerator.HandleTryGetParameter(typeSymbol,
                    (ptd.SharedTypeName, ptd.DisplayName),
                    code);

                code += @"
    }";
            }
            else
            {
                code = $@"using System;
using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Databases;
using TMPEffects.Parameters;
using static TMPEffects.Parameters.TMPParameterTypes;
using static TMPEffects.Parameters.TMPParameterUtility;

namespace {typeSymbol.ContainingNamespace.ToDisplayString()}
{{

    // This file was auto-generated by AutoParameters.
    // Changes to this file may cause incorrect behaviour and will be lost when the code is regenerated.
    // To learn more about AutoParameter, read <see href=""""""""https://tmpeffects.luca3317.dev/plugins/autoparameters.html"""""""">HERE</see>.
    // TODO Update link once docs updated
    {t}
    {{
        public static partial bool StringTo{ptd.DisplayName}(string str, out {ptd.SharedTypeName} result, ITMPKeywordDatabase db);
";

                code = ParameterUtilityGenerator.HandleHasParameter(typeSymbol, (ptd.SharedTypeName, ptd.DisplayName),
                    code);
                code = ParameterUtilityGenerator.HandleHasNonParameter(typeSymbol,
                    (ptd.SharedTypeName, ptd.DisplayName),
                    code);
                code = ParameterUtilityGenerator.HandleTryGetParameter(typeSymbol,
                    (ptd.SharedTypeName, ptd.DisplayName),
                    code);

                code += @"
    }
}";
            }

            // Prepare and add source
            var source = SourceText.From(code, Encoding.UTF8);
            context.AddSource($"{typeDecl.Identifier.Text}.{ptd.DisplayName}.g.cs", source);
        }

        public static MethodDeclarationSyntax BuildHasParameterMethod(string displayName, string typeName)
        {
            // Create the method declaration
            var methodDeclaration = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                    string.Format(Strings.HasTypeParameterName, displayName))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("parameters"))
                        .WithType(SyntaxFactory.ParseTypeName("IDictionary<string, string>")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("name"))
                        .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("aliases"))
                        .WithType(SyntaxFactory.ArrayType(
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                            SyntaxFactory.SingletonList<ArrayRankSpecifierSyntax>(
                                SyntaxFactory.ArrayRankSpecifier(
                                    SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                        SyntaxFactory.OmittedArraySizeExpression())))))
                        .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ParamsKeyword))))
                .WithBody(SyntaxFactory.Block(
                    SyntaxFactory.ReturnStatement(
                        SyntaxFactory.InvocationExpression(
                                SyntaxFactory.IdentifierName(
                                    string.Format(Strings.TryGetTypeParameterName, displayName)))
                            .AddArgumentListArguments(
                                SyntaxFactory.Argument(SyntaxFactory.DeclarationExpression(
                                        SyntaxFactory.IdentifierName(typeName),
                                        SyntaxFactory.DiscardDesignation(
                                            SyntaxFactory.Token(SyntaxKind.UnderscoreToken))))
                                    .WithRefOrOutKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)),
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameters")),
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("name")),
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("aliases"))
                            )
                    )
                ));

            return methodDeclaration;
        }

        public static MethodDeclarationSyntax BuildHasKeywordsParameterMethod(string displayName, string typeName)
        {
            // Create the method declaration
            var methodDeclaration = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                    string.Format(Strings.HasTypeParameterName, displayName))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("parameters"))
                        .WithType(SyntaxFactory.ParseTypeName("IDictionary<string, string>")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("name"))
                        .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("aliases"))
                        .WithType(SyntaxFactory.ArrayType(
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                            SyntaxFactory.SingletonList<ArrayRankSpecifierSyntax>(
                                SyntaxFactory.ArrayRankSpecifier(
                                    SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                        SyntaxFactory.OmittedArraySizeExpression())))))
                        .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ParamsKeyword))))
                .WithBody(SyntaxFactory.Block(
                    SyntaxFactory.ReturnStatement(
                        SyntaxFactory.InvocationExpression(
                                SyntaxFactory.IdentifierName(
                                    string.Format(Strings.TryGetTypeParameterName, displayName)))
                            .AddArgumentListArguments(
                                SyntaxFactory.Argument(SyntaxFactory.DeclarationExpression(
                                        SyntaxFactory.IdentifierName(typeName),
                                        SyntaxFactory.DiscardDesignation(
                                            SyntaxFactory.Token(SyntaxKind.UnderscoreToken))))
                                    .WithRefOrOutKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)),
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameters")),
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("name")),
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("aliases"))
                            )
                    )
                ));

            return methodDeclaration;
        }

        public static MethodDeclarationSyntax BuildHasNonParameterMethod(string displayName, string typeName)
        {
            // Create the method declaration
            var methodDeclaration = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                    string.Format(Strings.HasNonTypeParameterName, displayName))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("parameters"))
                        .WithType(SyntaxFactory.ParseTypeName("IDictionary<string, string>")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("name"))
                        .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("aliases"))
                        .WithType(SyntaxFactory.ArrayType(
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                            SyntaxFactory.SingletonList<ArrayRankSpecifierSyntax>(
                                SyntaxFactory.ArrayRankSpecifier(
                                    SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                        SyntaxFactory.OmittedArraySizeExpression())))))
                        .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ParamsKeyword))))
                .WithBody(SyntaxFactory.Block(
                    SyntaxFactory.IfStatement(
                        SyntaxFactory.PrefixUnaryExpression(
                            SyntaxKind.LogicalNotExpression,
                            SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.IdentifierName("ParameterDefined"))
                                .AddArgumentListArguments(
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameters")),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("name")),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("aliases"))
                                )
                        ),
                        SyntaxFactory.ReturnStatement(
                            SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression))
                    ),
                    SyntaxFactory.ReturnStatement(
                        SyntaxFactory.PrefixUnaryExpression(
                            SyntaxKind.LogicalNotExpression,
                            SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.IdentifierName(string.Format(Strings.TryGetTypeParameterName,
                                        displayName)))
                                .AddArgumentListArguments(
                                    SyntaxFactory.Argument(SyntaxFactory.DeclarationExpression(
                                            SyntaxFactory.IdentifierName(typeName),
                                            SyntaxFactory.DiscardDesignation(
                                                SyntaxFactory.Token(SyntaxKind.UnderscoreToken))))
                                        .WithRefOrOutKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameters")),
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("name")),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("aliases"))
                                )
                        )
                    )
                ));

            return methodDeclaration;
        }

        public static MethodDeclarationSyntax BuildHasNonKeywordParameterMethod(string displayName, string typeName)
        {
            var methodDeclaration = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                    string.Format(Strings.HasNonTypeParameterName, displayName))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("parameters"))
                        .WithType(SyntaxFactory.ParseTypeName("IDictionary<string, string>")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("keywords"))
                        .WithType(SyntaxFactory.ParseTypeName("ITMPKeywordDatabase")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("name"))
                        .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("aliases"))
                        .WithType(SyntaxFactory.ArrayType(
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                            SyntaxFactory.SingletonList<ArrayRankSpecifierSyntax>(
                                SyntaxFactory.ArrayRankSpecifier(
                                    SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                        SyntaxFactory.OmittedArraySizeExpression())))))
                        .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ParamsKeyword))))
                .WithBody(SyntaxFactory.Block(
                    SyntaxFactory.IfStatement(
                        SyntaxFactory.PrefixUnaryExpression(
                            SyntaxKind.LogicalNotExpression,
                            SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.IdentifierName("ParameterDefined"))
                                .AddArgumentListArguments(
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameters")),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("name")),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("aliases"))
                                )
                        ),
                        SyntaxFactory.ReturnStatement(
                            SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression))
                    ),
                    SyntaxFactory.ReturnStatement(
                        SyntaxFactory.PrefixUnaryExpression(
                            SyntaxKind.LogicalNotExpression,
                            SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.IdentifierName(string.Format(Strings.TryGetTypeParameterName,
                                        displayName)))
                                .AddArgumentListArguments(
                                    SyntaxFactory.Argument(SyntaxFactory.DeclarationExpression(
                                            SyntaxFactory.IdentifierName(typeName),
                                            SyntaxFactory.DiscardDesignation(
                                                SyntaxFactory.Token(SyntaxKind.UnderscoreToken))))
                                        .WithRefOrOutKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameters")),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("keywords")),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("name")),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("aliases"))
                                )
                        )
                    )
                ));

            return methodDeclaration;
        }

        public static MethodDeclarationSyntax BuildTryGetParameterMethod(string displayName, string typeName)
        {
            var methodDeclaration = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                    string.Format(Strings.TryGetTypeParameterName, displayName))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("value"))
                        .WithType(SyntaxFactory.ParseTypeName(typeName))
                        .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.OutKeyword))),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("parameters"))
                        .WithType(SyntaxFactory.ParseTypeName("IDictionary<string, string>")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("name"))
                        .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("aliases"))
                        .WithType(SyntaxFactory.ArrayType(
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                            SyntaxFactory.SingletonList<ArrayRankSpecifierSyntax>(
                                SyntaxFactory.ArrayRankSpecifier(
                                    SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                        SyntaxFactory.OmittedArraySizeExpression())))))
                        .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ParamsKeyword))))
                .WithBody(SyntaxFactory.Block(
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            SyntaxFactory.IdentifierName("value"),
                            SyntaxFactory.DefaultExpression(
                                SyntaxFactory.ParseTypeName(typeName)))),
                    SyntaxFactory.IfStatement(
                        SyntaxFactory.PrefixUnaryExpression(
                            SyntaxKind.LogicalNotExpression,
                            SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.IdentifierName(string.Format(Strings.TryGetTypeParameterName,
                                        displayName)))
                                .AddArgumentListArguments(
                                    SyntaxFactory.Argument(SyntaxFactory.DeclarationExpression(
                                            SyntaxFactory.IdentifierName("string"),
                                            SyntaxFactory.SingleVariableDesignation(
                                                SyntaxFactory.Identifier("parameterName"))))
                                        .WithRefOrOutKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameters")),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("name")),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("aliases"))
                                )
                        ),
                        SyntaxFactory.ReturnStatement(
                            SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression))
                    ),
                    SyntaxFactory.ReturnStatement(
                        SyntaxFactory.InvocationExpression(
                                SyntaxFactory.IdentifierName("StringTo" + displayName))
                            .AddArgumentListArguments(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.ElementAccessExpression(SyntaxFactory.IdentifierName("parameters"))
                                        .AddArgumentListArguments(
                                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameterName")))),
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("value"))
                                    .WithRefOrOutKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)),
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression))
                            )
                    )
                ));
            return methodDeclaration;
        }

        public static MethodDeclarationSyntax BuildTryGetKeywordParameterMethod(string displayName, string typeName)
        {
            var methodDeclaration = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                    string.Format(Strings.TryGetTypeParameterName, displayName))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("value"))
                        .WithType(SyntaxFactory.ParseTypeName(typeName))
                        .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.OutKeyword))),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("parameters"))
                        .WithType(SyntaxFactory.ParseTypeName("IDictionary<string, string>")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("keywords"))
                        .WithType(SyntaxFactory.ParseTypeName("ITMPKeywordDatabase")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("name"))
                        .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("aliases"))
                        .WithType(SyntaxFactory.ArrayType(
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                            SyntaxFactory.SingletonList<ArrayRankSpecifierSyntax>(
                                SyntaxFactory.ArrayRankSpecifier(
                                    SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                        SyntaxFactory.OmittedArraySizeExpression())))))
                        .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ParamsKeyword))))
                .WithBody(SyntaxFactory.Block(
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            SyntaxFactory.IdentifierName("value"),
                            SyntaxFactory.DefaultExpression(SyntaxFactory.ParseTypeName(typeName)))),
                    SyntaxFactory.IfStatement(
                        SyntaxFactory.PrefixUnaryExpression(
                            SyntaxKind.LogicalNotExpression,
                            SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.IdentifierName("TryGetDefinedParameter"))
                                .AddArgumentListArguments(
                                    SyntaxFactory.Argument(SyntaxFactory.DeclarationExpression(
                                            SyntaxFactory.IdentifierName("string"),
                                            SyntaxFactory.SingleVariableDesignation(
                                                SyntaxFactory.Identifier("parameterName"))))
                                        .WithRefOrOutKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameters")),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("name")),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("aliases"))
                                )
                        ),
                        SyntaxFactory.ReturnStatement(
                            SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression))
                    ),
                    SyntaxFactory.ReturnStatement(
                        SyntaxFactory.InvocationExpression(
                                SyntaxFactory.IdentifierName("StringTo" + displayName))
                            .AddArgumentListArguments(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.ElementAccessExpression(SyntaxFactory.IdentifierName("parameters"))
                                        .AddArgumentListArguments(
                                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameterName")))),
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("value"))
                                    .WithRefOrOutKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)),
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("keywords"))
                            )
                    )
                ));

            return methodDeclaration;
        }
    }
}