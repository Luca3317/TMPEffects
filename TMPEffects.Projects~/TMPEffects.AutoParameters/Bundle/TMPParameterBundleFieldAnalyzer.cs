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
    public class TMPParameterBundleFieldAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId_305 = "TMPPT305";

        private static readonly LocalizableString Title_305 =
            "DisplayName must be a valid identifier";

        private static readonly LocalizableString MessageFormat_305 =
            "Display name {0} is not a valid identifier";

        private const string Category_305 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_305 = new DiagnosticDescriptor(DiagnosticId_305, Title_305,
            MessageFormat_305, Category_305, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_306 = "TMPPT306";

        private static readonly LocalizableString Title_306 =
            "DisplayName must not be a reserved keyword";

        private static readonly LocalizableString MessageFormat_306 =
            "Display name {0} is a reserved keyword";

        private const string Category_306 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_306 = new DiagnosticDescriptor(DiagnosticId_306, Title_306,
            MessageFormat_306, Category_306, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking


        public const string DiagnosticId_307 = "TMPPT307";


        private static readonly LocalizableString Title_307 = "Fields decorated with " +
                                                              Strings.TMPParameterBundleFieldAttributeName +
                                                              " must be contained within a type decorated with " +
                                                              Strings.TMPParameterBundleAttributeName;

        private static readonly LocalizableString MessageFormat_307 = "Field {0} decorated with " +
                                                                      Strings.TMPParameterBundleFieldAttributeName +
                                                                      " is not contained in a type decorated with " +
                                                                      Strings.TMPParameterBundleAttributeName;

        private const string Category_307 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_307 = new DiagnosticDescriptor(DiagnosticId_307, Title_307,
            MessageFormat_307, Category_307, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
        

        public const string DiagnosticId_308 = "TMPPT308";

        private static readonly LocalizableString Title_308 =
            "Invalid type of field decorated with " + Strings.TMPParameterBundleFieldAttribute;

        private static readonly LocalizableString MessageFormat_308 =
            "Field {0} decorated with " + Strings.TMPParameterBundleFieldAttribute + " has invalid type {1}";

        private const string Category_308 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_308 = new DiagnosticDescriptor(DiagnosticId_308, Title_308,
            MessageFormat_308, Category_308, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
        
        
        public const string DiagnosticId_309 = "TMPPT309";

        private static readonly LocalizableString Title_309 =
            "Invalid type of field decorated with " + Strings.TMPParameterBundleFieldAttribute;

        private static readonly LocalizableString MessageFormat_309 =
            "Field {0} decorated with " + Strings.TMPParameterBundleFieldAttribute + " has invalid type {1}; bundles may only contain directly supported types (no subtypes)";

        private const string Category_309 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_309 = new DiagnosticDescriptor(DiagnosticId_309, Title_309,
            MessageFormat_309, Category_309, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking


        public const string DiagnosticId_302 = "TMPPT302";

        private static readonly LocalizableString Title_302 =
            "DisplayName must be unique";

        private static readonly LocalizableString MessageFormat_302 =
            "Display name {0} is not unique";

        private const string Category_302 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_302 = new DiagnosticDescriptor(DiagnosticId_302, Title_302,
            MessageFormat_302, Category_302, DiagnosticSeverity.Error, isEnabledByDefault: true);
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
                Rule_302,
                Rule_305,
                Rule_306,
                Rule_307,
                Rule_308,
                Rule_309
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

            if (symbol.ContainingType.ToDisplayString() != Strings.TMPParameterBundleFieldAttributeName)
                return;

            try
            {
                AnalyzeParameterTypeAttributeSyntax(context, attributeSyntax, symbol);
            }
            catch (System.Exception e)
            {
                Diagnostic d = Diagnostic.Create(Rule___, attributeSyntax.GetLocation(),
                    "Failed TryAnalyzeParameterBundleFieldSyntax for " + attributeSyntax.Ancestors()
                        .OfType<TypeDeclarationSyntax>().First().Identifier.Text + ". " +
                    "This is most likely a bug -- feel free to open an issue or a pull request with your fix on " +
                    "https://github.com/Luca3317/TMPEffects\n" + e.Message);
                context.ReportDiagnostic(d);
            }
        }

        private void AnalyzeParameterTypeAttributeSyntax(SyntaxNodeAnalysisContext context,
            AttributeSyntax attributeSyntax, IMethodSymbol symbol)
        {
            // Rules that need to be enforced
            // 1. Must be contained in tmpparameterbundle
            // 2. Must be of valid autoparametertype -- issue: need to duplicate autoparameter roslyn code.
            //      There will be similar issues with the generator for Validate Set etc syntax
            //      Do i move this into the autoparameter roslyn code?

            // AnalyzeDisplayName(context, attributeSyntax, symbol);

            // If not contained in decorated type, report
            var f = attributeSyntax.Ancestors().OfType<FieldDeclarationSyntax>().First();
            var containerType = f.Ancestors().OfType<TypeDeclarationSyntax>().First();
            var containerSymbol = context.SemanticModel.GetDeclaredSymbol(containerType) as INamedTypeSymbol;
            if (!containerSymbol.GetAttributes().Any(attr =>
                    attr.AttributeClass.ToDisplayString() == Strings.TMPParameterBundleAttributeName))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule_307, attributeSyntax.GetLocation(), f.Declaration.Variables[0].Identifier.Text));
            }
            
            // If not valid AutoParameter type, report
            var typeSymbol = context.SemanticModel.GetTypeInfo(f.Declaration.Type).Type;
            if (!Utility.IsValidAutoParameterType(typeSymbol, true))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule_308, attributeSyntax.GetLocation(), f.Declaration.Variables[0].Identifier.Text, f.Declaration.Type.ToString()));
            }
            
            // If not directly valid AutoParameter type (eg not decorated by TMPParameterType itself), report
            if (!Utility.IsValidAutoParameterType(typeSymbol, false))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule_309, attributeSyntax.GetLocation(), f.Declaration.Variables[0].Identifier.Text, f.Declaration.Type.ToString()));
            }
        }
    }
}