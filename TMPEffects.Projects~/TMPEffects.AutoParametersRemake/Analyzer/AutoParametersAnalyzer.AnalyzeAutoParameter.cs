using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using TMPEffects.AutoParameters.Generator;
using TMPEffects.StringLibrary;

namespace TMPEffects.AutoParameters.Analyzer
{
    public partial class AutoParametersAnalyzer
    {
        private void TryAnalyzeSyntaxAutoParameter(SyntaxNodeAnalysisContext context)
        {
            FieldDeclarationSyntax fieldDecl = (FieldDeclarationSyntax)context.Node;

            try
            {
                foreach (var v in fieldDecl.Declaration.Variables)
                {
                    IFieldSymbol varSymbol = context.SemanticModel.GetDeclaredSymbol(v) as IFieldSymbol;
                    if (Utility.IsAutoParameter(varSymbol) != null) AnalyzeSyntaxAutoParameter(context, v, varSymbol);
                }
            }
            catch (System.Exception e)
            {
                Diagnostic d = Diagnostic.Create(Rule___, fieldDecl.GetLocation(),
                    "Failed TryAnalyzeSyntaxAutoParameter for " +
                    fieldDecl.Declaration.Variables.First().Identifier.Text + ". " +
                    "This is most likely a bug -- feel free to open an issue or a pull request with your fix on " +
                    "https://github.com/Luca3317/TMPEffects\n" + e.Message);
                context.ReportDiagnostic(d);
            }
        }

        private void AnalyzeSyntaxAutoParameter(SyntaxNodeAnalysisContext context, VariableDeclaratorSyntax varDecl,
            IFieldSymbol varSymbol)
        {
            Diagnostic diagnostic;

            // Check validity of type
            if (Utility.IsAutoParameterBundle(varSymbol) != null)
            {
                string typestring = varSymbol.Type.ToDisplayString();
                if (!Strings.ValidAutoParameterBundleTypes.Contains(typestring))
                {
                    if (Strings.ValidAutoParameterTypes.Contains(typestring))
                    {
                        diagnostic = Diagnostic.Create(Rule_1_4, varDecl.GetLocation(), varSymbol.Name,
                            varSymbol.Type.ToMinimalDisplayString(context.SemanticModel, 0));
                        context.ReportDiagnostic(diagnostic);
                    }
                    else
                    {
                        diagnostic = Diagnostic.Create(Rule_1_3, varDecl.GetLocation(), varSymbol.Name,
                            varSymbol.Type.ToMinimalDisplayString(context.SemanticModel, 0));
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
            else
            {
                string typestring = varSymbol.Type is IArrayTypeSymbol arr
                    ? arr.ElementType.ToDisplayString()
                    : varSymbol.Type.ToDisplayString();
                if (!Strings.ValidAutoParameterTypes.Contains(typestring))
                {
                    if (Strings.ValidAutoParameterBundleTypes.Contains(typestring))
                    {
                        if (varSymbol.Type is IArrayTypeSymbol)
                        {
                            diagnostic = Diagnostic.Create(Rule_1_1, varDecl.GetLocation(), varSymbol.Name,
                                varSymbol.Type.ToMinimalDisplayString(context.SemanticModel, 0));
                            context.ReportDiagnostic(diagnostic);
                        }
                        else
                        {
                            diagnostic = Diagnostic.Create(Rule_1_2, varDecl.GetLocation(), varSymbol.Name,
                                varSymbol.Type.ToMinimalDisplayString(context.SemanticModel, 0));
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                    else
                    {
                        diagnostic = Diagnostic.Create(Rule_1_1, varDecl.GetLocation(), varSymbol.Name,
                            varSymbol.Type.ToMinimalDisplayString(context.SemanticModel, 0));
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }

            // If is not contained in another type, return; shouldnt really be possible
            if (varSymbol.ContainingType == null)
            {
                diagnostic = Diagnostic.Create(Rule_1_5, varDecl.GetLocation(), varDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
                return;
            }

            // If is not contained in type with AutoParametersAttibute, report
            if (!varSymbol.ContainingType.GetAttributes().Any(attr =>
                    attr.AttributeClass?.ToDisplayString() == Strings.AutoParametersAttributeName))
            {
                diagnostic = Diagnostic.Create(Rule_1_5, varDecl.GetLocation(), varDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
                return;
            }

            // // If is not serialized (public or SerializedField), report
            // if (varSymbol.DeclaredAccessibility == Accessibility.Public) return;
            // if (varSymbol.GetAttributes().Any(attr =>
            //         attr.AttributeClass?.ToDisplayString() == Strings.SerializeFieldAttributeName
            //         || attr.AttributeClass?.ToDisplayString() == Strings.SerializeReferenceAttributeName)) return;
            //
            // diagnostic = Diagnostic.Create(Rule_1_7, varDecl.Identifier.GetLocation(), varDecl.Identifier.Text);
            // context.ReportDiagnostic(diagnostic);
        }
    }
}