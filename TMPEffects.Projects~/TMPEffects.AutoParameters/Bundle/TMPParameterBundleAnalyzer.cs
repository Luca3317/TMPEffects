using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using TMPEffects.AutoParameters.TMPEffects.AutoParameters.Generator;
using TMPEffects.StringLibrary;

namespace TMPEffects.ParameterUtilityGenerator
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TMPParameterBundleAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId_205 = "TMPPT205";

        private static readonly LocalizableString Title_205 =
            "DisplayName must be a valid identifier";

        private static readonly LocalizableString MessageFormat_205 =
            "Display name {0} is not a valid identifier";

        private const string Category_205 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_205 = new DiagnosticDescriptor(DiagnosticId_205, Title_205,
            MessageFormat_205, Category_205, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_206 = "TMPPT206";

        private static readonly LocalizableString Title_206 =
            "DisplayName must not be a reserved keyword";

        private static readonly LocalizableString MessageFormat_206 =
            "Display name {0} is a reserved keyword";

        private const string Category_206 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_206 = new DiagnosticDescriptor(DiagnosticId_206, Title_206,
            MessageFormat_206, Category_206, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking


        public const string DiagnosticId_202 = "TMPPT202";

        private static readonly LocalizableString Title_202 =
            "DisplayName must be unique";

        private static readonly LocalizableString MessageFormat_202 =
            "Display name {0} is not unique";

        private const string Category_202 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_202 = new DiagnosticDescriptor(DiagnosticId_202, Title_202,
            MessageFormat_202, Category_202, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking


        public const string DiagnosticId_207 = "TMPPT207";

        private static readonly LocalizableString Title_207 =
            "ValidateParameters hook method not implemented";

        private static readonly LocalizableString MessageFormat_207 =
            "Consider implementing the ValidateParameters hook method, if you need custom logic";

        private const string Category_207 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_207 = new DiagnosticDescriptor(DiagnosticId_207, Title_207,
            MessageFormat_207, Category_207, DiagnosticSeverity.Info, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking


        public const string DiagnosticId_208 = "TMPPT208";

        private static readonly LocalizableString Title_208 =
            "SetParameters hook method not implemented";

        private static readonly LocalizableString MessageFormat_208 =
            "Consider implementing the SetParameters hook method, if you need custom logic";

        private const string Category_208 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_208 = new DiagnosticDescriptor(DiagnosticId_208, Title_208,
            MessageFormat_208, Category_208, DiagnosticSeverity.Info, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking


        public const string DiagnosticId_209 = "TMPPT209";

        private static readonly LocalizableString Title_209 =
            "Create hook method not implemented";

        private static readonly LocalizableString MessageFormat_209 =
            "Consider implementing the Create hook method, if you need custom logic";

        private const string Category_209 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_209 = new DiagnosticDescriptor(DiagnosticId_209, Title_209,
            MessageFormat_209, Category_209, DiagnosticSeverity.Info, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
        
        
        public const string DiagnosticId_210 = "TMPPT210";

        private static readonly LocalizableString Title_210 =
            "Does not have an empty constructor";

        private static readonly LocalizableString MessageFormat_210 =
            "Types decorated with " + Strings.TMPParameterBundleAttribute + " must have an empty constructor";

        private const string Category_210 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_210 = new DiagnosticDescriptor(DiagnosticId_210, Title_210,
            MessageFormat_210, Category_210, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
                
        
        public const string DiagnosticId_211 = "TMPPT211";

        private static readonly LocalizableString Title_211 =
            "Types decorated with " + Strings.TMPParameterBundleAttribute + " must not be nested";

        private static readonly LocalizableString MessageFormat_211 =
            "Type {0} decorated with " + Strings.TMPParameterBundleAttribute + " must not be nested";

        private const string Category_211 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_211 = new DiagnosticDescriptor(DiagnosticId_211, Title_211,
            MessageFormat_211, Category_211, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking


        public const string DiagnosticId___ = "ParametersBundleException";
        private static readonly LocalizableString Title___ = "ParametersBundleException";
        private static readonly LocalizableString MessageFormat___ = "{0}";
        private const string Category___ = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule___ = new DiagnosticDescriptor(DiagnosticId___, Title___,
            MessageFormat___, Category___, DiagnosticSeverity.Warning, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create
            (
                Rule___,
                Rule_202,
                Rule_205,
                Rule_206,
                Rule_207,
                Rule_208,
                Rule_209,
                Rule_210,
                Rule_211
            );

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(TryAnalyzeSyntax, SyntaxKind.Attribute);
        }

        private void TryAnalyzeSyntax(SyntaxNodeAnalysisContext context)
        {
            // TypeDeclarationSyntax typeDecl = (TypeDeclarationSyntax)context.Node;
            AttributeSyntax attributeSyntax = context.Node as AttributeSyntax;
            var symbol = context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol as IMethodSymbol;

            if (symbol.ContainingType.ToDisplayString() != Strings.TMPParameterBundleAttributeName)
                return;

            try
            {
                AnalyzeParameterTypeAttributeSyntax(context, attributeSyntax, symbol);
            }
            catch (System.Exception e)
            {
                Diagnostic d = Diagnostic.Create(Rule___, attributeSyntax.GetLocation(),
                    "Failed TryAnalyzeParameterBundleSyntax for " + attributeSyntax.Ancestors()
                        .OfType<TypeDeclarationSyntax>().First().Identifier.Text + ". " +
                    "This is most likely a bug -- feel free to open an issue or a pull request with your fix on " +
                    "https://github.com/Luca3317/TMPEffects\n" + e.Message);
                context.ReportDiagnostic(d);
            }
        }

        private void AnalyzeParameterTypeAttributeSyntax(SyntaxNodeAnalysisContext context,
            AttributeSyntax attributeSyntax, IMethodSymbol symbol)
        {
            AnalyzeDisplayName(context, attributeSyntax, symbol);
        }

        private static void AnalyzeDisplayName(SyntaxNodeAnalysisContext context, AttributeSyntax attributeSyntax,
            ISymbol attributeSymbol)
        {
            var attributeTypeSymbol = attributeSymbol.ContainingType;

            var literal =
                attributeSyntax.ArgumentList?.Arguments.FirstOrDefault()?.Expression as LiteralExpressionSyntax;
            var displayName = literal?.Token.ValueText;

            if (literal == null) return;

            if (!SyntaxFacts.IsValidIdentifier(displayName))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule_205, attributeSyntax.GetLocation(), displayName));
                return;
            }

            // TODO this doesnt check built in bundles just types
            if (Strings.DisplayStringToType.ContainsKey(displayName))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule_206, attributeSyntax.GetLocation(), displayName));
                return;
            }

            var compilation = context.SemanticModel.Compilation;
            var allAttributes = new List<AttributeSyntax>();

            foreach (var tree in compilation.SyntaxTrees)
            {
                var root = tree.GetRoot();
                var model = compilation.GetSemanticModel(tree);

                var attributeNodes = root.DescendantNodes().OfType<AttributeSyntax>();

                foreach (var attr in attributeNodes)
                {
                    var symbol = model.GetSymbolInfo(attr).Symbol as IMethodSymbol;
                    if (symbol != null &&
                        SymbolEqualityComparer.Default.Equals(symbol.ContainingType, attributeTypeSymbol))
                    {
                        var firstArgument =
                            attr.ArgumentList?.Arguments.FirstOrDefault()?.Expression as LiteralExpressionSyntax;
                        if (firstArgument != null && firstArgument.Token.ValueText == displayName)
                            allAttributes.Add(attr);
                    }
                }
            }

            // If name not unique, report
            if (allAttributes.Count > 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule_202, attributeSyntax.GetLocation(), displayName));
            }

            var typeDecl = attributeSyntax.Ancestors().OfType<TypeDeclarationSyntax>().First();
            var typeSymbol = context.SemanticModel.GetDeclaredSymbol(typeDecl);
            CheckHookMethods(displayName, context, typeSymbol);

            CheckEmptyConstructor(context, typeSymbol);
            CheckIsNotNested(context, typeSymbol);
        }
        
        private static void CheckIsNotNested(SyntaxNodeAnalysisContext context, INamedTypeSymbol symbol)
        {
            if (symbol.ContainingType == null) return;
            context.ReportDiagnostic(Diagnostic.Create(Rule_211, symbol.Locations[0], symbol.Name));
        }

        private static void CheckEmptyConstructor(SyntaxNodeAnalysisContext context, INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeKind == TypeKind.Struct) return;

            if (!typeSymbol.Constructors.Any()) return;
            if (typeSymbol.Constructors.Any(ctr => ctr.Parameters.Length == 0)) return;
            
            context.ReportDiagnostic(Diagnostic.Create(Rule_210, typeSymbol.Locations[0]));
        }

        private static void CheckHookMethods(string displayName, SyntaxNodeAnalysisContext context,
            INamedTypeSymbol typeSymbol)
        {
            var stringType = context.Compilation.GetSpecialType(SpecialType.System_String);
            var validateHook = Utility.ImplementsBundleValidateParametersHook(typeSymbol, stringType);
            var setParamsHook = Utility.ImplementsBundleGetParametersHook(typeSymbol, stringType);
            var getCustomHook = Utility.ImplementsBundleCreateHook(displayName, typeSymbol, stringType);

            if (!validateHook)
                context.ReportDiagnostic(Diagnostic.Create(Rule_207, typeSymbol.Locations[0]));
            if (!setParamsHook)
                context.ReportDiagnostic(Diagnostic.Create(Rule_208, typeSymbol.Locations[0]));
            if (!getCustomHook)
                context.ReportDiagnostic(Diagnostic.Create(Rule_209, typeSymbol.Locations[0]));
        }
    }
}