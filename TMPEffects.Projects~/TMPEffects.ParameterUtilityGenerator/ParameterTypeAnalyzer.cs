using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using TMPEffects.StringLibrary;
using System.Linq;

namespace TMPEffects.ParameterUtilityGenerator
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public partial class ParameterTypeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId_100 = "TMPPT100";

        private static readonly LocalizableString Title_100 =
            "Scene type must implement the shared type";

        private static readonly LocalizableString MessageFormat_100 =
            "Scene type {0} does not implement the shared type {1}";

        private const string Category_100 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_100 = new DiagnosticDescriptor(DiagnosticId_100, Title_100,
            MessageFormat_100, Category_100, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking


        public const string DiagnosticId_101 = "TMPPT101";

        private static readonly LocalizableString Title_101 =
            "Disk type must implement the shared type";

        private static readonly LocalizableString MessageFormat_101 =
            "Disk type {0} does not implement the shared type {1}";

        private const string Category_101 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_101 = new DiagnosticDescriptor(DiagnosticId_101, Title_101,
            MessageFormat_101, Category_101, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking


        public const string DiagnosticId_102 = "TMPPT102";

        private static readonly LocalizableString Title_102 =
            "DisplayName must be unique";

        private static readonly LocalizableString MessageFormat_102 =
            "Display name {0} is not unique";

        private const string Category_102 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_102 = new DiagnosticDescriptor(DiagnosticId_102, Title_102,
            MessageFormat_102, Category_102, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking


        public const string DiagnosticId_103 = "TMPPT103";

        private static readonly LocalizableString Title_103 =
            "Must only have one ParameterType attribute on a type";

        private static readonly LocalizableString MessageFormat_103 =
            "{0} is decorated with multiple ParameterType attributes";

        private const string Category_103 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_103 = new DiagnosticDescriptor(DiagnosticId_103, Title_103,
            MessageFormat_103, Category_103, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking


        public const string DiagnosticId___ = "TypeParametersException";
        private static readonly LocalizableString Title___ = "TypeParametersException";
        private static readonly LocalizableString MessageFormat___ = "{0}";
        private const string Category___ = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule___ = new DiagnosticDescriptor(DiagnosticId___, Title___,
            MessageFormat___, Category___, DiagnosticSeverity.Warning, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
        
        
        public const string DiagnosticId_104 = "TMPPT104";

        private static readonly LocalizableString Title_104 =
            "DisplayName must be a valid identifier";

        private static readonly LocalizableString MessageFormat_104 =
            "Display name {0} is not a valid identifier";

        private const string Category_104 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_104 = new DiagnosticDescriptor(DiagnosticId_104, Title_104,
            MessageFormat_104, Category_104, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
        
        public const string DiagnosticId_105 = "TMPPT105";

        private static readonly LocalizableString Title_105 =
            "DisplayName must not be a reserved keyword";

        private static readonly LocalizableString MessageFormat_105 =
            "Display name {0} is a reserved keyword";

        private const string Category_105 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_105 = new DiagnosticDescriptor(DiagnosticId_105, Title_105,
            MessageFormat_105, Category_105, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
        
        public const string DiagnosticId_106 = "TMPPT106";

        private static readonly LocalizableString Title_106 =
            "Types decorated with TMPParameterType must implement method StringTo";

        private static readonly LocalizableString MessageFormat_106 =
            "Type {0} decorated with TMPParameterType does not implement method public static partial bool StringTo{1}(string, out {0}, ITMPKeywordDatabase)";

        private const string Category_106 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_106 = new DiagnosticDescriptor(DiagnosticId_106, Title_106,
            MessageFormat_106, Category_106, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking
        
        public const string DiagnosticId_107 = "TMPPT107";

        private static readonly LocalizableString Title_107 =
            "Types decorated with " + Strings.TMPParameterTypeAttribute + " must not be nested";

        private static readonly LocalizableString MessageFormat_107 =
            "Type {0} decorated with " + Strings.TMPParameterTypeAttribute + " must not be nested";

        private const string Category_107 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_107 = new DiagnosticDescriptor(DiagnosticId_107, Title_107,
            MessageFormat_107, Category_107, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create
                (
                    Rule_100,
                    Rule_101,
                    Rule_102,
                    Rule_103,
                    Rule_104,
                    Rule_105,
                    Rule_106,
                    Rule_107,
                    Rule___
                );
            }
        }


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

            if (symbol.ContainingType.ToDisplayString() != Strings.TMPParameterTypeAttributeName)
                return;

            try
            {
                AnalyzeParameterTypeAttributeSyntax(context, attributeSyntax, symbol);
            }
            catch (System.Exception e)
            {
                Diagnostic d = Diagnostic.Create(Rule___, attributeSyntax.GetLocation(),
                    "Failed TryAnalyzeTypeParameterSyntax for " + attributeSyntax.Ancestors()
                        .OfType<TypeDeclarationSyntax>().First().Identifier.Text + ". " +
                    "This is most likely a bug -- feel free to open an issue or a pull request with your fix on " +
                    "https://github.com/Luca3317/TMPEffects\n" + e.Message);
                context.ReportDiagnostic(d);
            }
        }

        
        private void CheckIsNotNested(SyntaxNodeAnalysisContext context, INamedTypeSymbol symbol)
        {
            if (symbol.ContainingType == null) return;
            context.ReportDiagnostic(Diagnostic.Create(Rule_107, symbol.Locations[0], symbol.Name));
        }
        
        private void AnalyzeParameterTypeAttributeSyntax(SyntaxNodeAnalysisContext context,
            AttributeSyntax attributeSyntax, IMethodSymbol attributeSymbol)
        {
            AnalyzeDisplayName(context, attributeSyntax, attributeSymbol);
            var data = Utility.GetParameterTypeData(attributeSyntax, context.SemanticModel);

            if (!EnsureImplement(context, attributeSyntax, data.SceneType, data.SharedType))
                context.ReportDiagnostic(Diagnostic.Create(Rule_100, attributeSyntax.GetLocation(), data.SceneTypeName,
                    data.SharedTypeName));

            if (!EnsureImplement(context, attributeSyntax, data.DiskType, data.SharedType))
                context.ReportDiagnostic(Diagnostic.Create(Rule_101, attributeSyntax.GetLocation(), data.DiskTypeName,
                    data.SharedTypeName));

            var typeDecl = attributeSyntax.Ancestors().OfType<TypeDeclarationSyntax>().First();
            var typeSymbol = context.SemanticModel.GetDeclaredSymbol(typeDecl);
            var methods = typeSymbol.GetMembers().OfType<IMethodSymbol>().Where(m =>
                m.DeclaredAccessibility == Accessibility.Public &&
                m.IsStatic && (m.PartialDefinitionPart != null || m.PartialImplementationPart != null) &&
                m.Name == "StringTo" + data.DisplayName &&
                m.ReturnType.SpecialType == SpecialType.System_Boolean).ToList();
            
            CheckIsNotNested(context, typeSymbol);

            bool present = false;
            foreach (var method in methods)
            {
                if (method.Parameters == null || method.Parameters.Length != 3) continue;

                if (method.Parameters[0].Type.SpecialType != SpecialType.System_String) continue;
                if (method.Parameters[1].Type.ToDisplayString() != data.SharedTypeName) continue;
                if (method.Parameters[2].Type.ToDisplayString() != Strings.ITMPKeywordDatabaseName) continue;

                present = true;
                break;
            }

            if (!present)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule_106, attributeSyntax.GetLocation(), data.SharedType.Name,
                    data.DisplayName));
            }
        }

        private bool EnsureImplement(SyntaxNodeAnalysisContext ctx, AttributeSyntax attributeSyntax, ITypeSymbol type,
            ITypeSymbol baseType)
        {
            if (type.ToDisplayString() == baseType.ToDisplayString())
                return true;

            foreach (var inter in type.AllInterfaces)
            {
                if (inter.ToDisplayString() == baseType.ToDisplayString())
                    return true;
            }

            type = type.BaseType;
            while (type != null)
            {
                if (type.ToDisplayString() == baseType.ToDisplayString())
                    return true;
                type = type.BaseType;
            }

            return false;
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
                context.ReportDiagnostic(Diagnostic.Create(Rule_104, attributeSyntax.GetLocation(), displayName));
                return;
            }

            if (Strings.DisplayStringToType.ContainsKey(displayName))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule_105, attributeSyntax.GetLocation(), displayName));
                return;
            }
            
            // if (!SyntaxFacts.IsReservedKeyword(SyntaxFacts.GetKeywordKind(displayName)))
            // {
            //     context.ReportDiagnostic(Diagnostic.Create(Rule_102, attributeSyntax.GetLocation(), displayName));
            //     return;
            // }
            
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
                context.ReportDiagnostic(Diagnostic.Create(Rule_102, attributeSyntax.GetLocation(), displayName));
            }
        }
    }
}