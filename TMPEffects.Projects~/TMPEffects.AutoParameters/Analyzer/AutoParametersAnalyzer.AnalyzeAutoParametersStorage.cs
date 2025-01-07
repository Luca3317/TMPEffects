using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using TMPEffects.AutoParameters.Generator;
using TMPEffects.AutoParameters.TMPEffects.AutoParameters.Generator;
using TMPEffects.StringLibrary;
using Diagnostic = Microsoft.CodeAnalysis.Diagnostic;
using INamedTypeSymbol = Microsoft.CodeAnalysis.INamedTypeSymbol;

namespace TMPEffects.AutoParameters.Analyzer
{
    public partial class AutoParametersAnalyzer
    {
        private void TryAnalyzeSyntaxAutoParametersStorage(SyntaxNodeAnalysisContext context)
        {
            TypeDeclarationSyntax typeDecl = (TypeDeclarationSyntax)context.Node;

            try
            {
                var symbol = context.SemanticModel.GetDeclaredSymbol(typeDecl);

                if (symbol.GetAttributes().Any(a =>
                        a.AttributeClass?.ToDisplayString() == Strings.AutoParametersStorageAttributeName))
                {
                    AnalyzeSyntaxAutoParametersStorage(context, typeDecl, symbol);
                }
            }
            catch (Exception e)
            {
                Diagnostic d = Diagnostic.Create(Rule___, typeDecl.GetLocation(),
                    "Failed TryAnalyzeSyntaxAutoParametersStorage. " +
                    "This is most likely a bug -- feel free to open an issue or a pull request with your fix on " +
                    "https://github.com/Luca3317/TMPEffects");
                context.ReportDiagnostic(d);
            }
        }

        private void AnalyzeSyntaxAutoParametersStorage(SyntaxNodeAnalysisContext context,
            TypeDeclarationSyntax typeDecl, INamedTypeSymbol typeSymbol)
        {
            Diagnostic diagnostic;

            // If is not partial, report
            if (!typeDecl.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword)))
            {
                diagnostic = Diagnostic.Create(Rule_2_0, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }

            // If has no empty constructor, report
            if (!typeSymbol.Constructors.Any(ctr => ctr.Parameters.Length == 0))
            {
                diagnostic = Diagnostic.Create(Rule_2_4, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }

            // If is not contained in another type, report
            if (!(typeDecl.Parent is TypeDeclarationSyntax parentDecl))
            {
                diagnostic = Diagnostic.Create(Rule_2_1, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
                return;
            }

            var parentSymbol = context.SemanticModel.GetDeclaredSymbol(parentDecl);
            if (parentSymbol == null) return;

            // If is not contained in type with AutoParametersAttibute, report
            if (!parentSymbol.GetAttributes().Any(attr =>
                    attr.AttributeClass?.ToDisplayString() == Strings.AutoParametersAttributeName))
            {
                diagnostic = Diagnostic.Create(Rule_2_2, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
                return;
            }

            // If has field with same name AutoParameter, report
            var containingTypeFields = parentSymbol.GetMembers()
                .Where(member => member.Kind == SymbolKind.Field && Utility.IsAutoParameter(member as IFieldSymbol) != null);
            var fields = typeSymbol.GetMembers()
                .Where(member => member.Kind == SymbolKind.Field || member.Kind == SymbolKind.Property);
            var containingTypeFieldNames = containingTypeFields.Select(field => field.Name).ToImmutableHashSet();

            foreach (var field in fields)
            {
                if (containingTypeFieldNames.Contains(field.Name))
                {
                    diagnostic = Diagnostic.Create(Rule_2_1, field.Locations[0], typeDecl.Identifier.Text, field.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}