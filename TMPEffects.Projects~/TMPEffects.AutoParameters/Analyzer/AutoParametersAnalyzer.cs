using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TMPEffects.AutoParameters.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public partial class AutoParametersAnalyzer : DiagnosticAnalyzer
    {
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(TryAnalyzeSyntaxAutoParameters, SyntaxKind.ClassDeclaration,
                SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(TryAnalyzeSyntaxAutoParametersStorage, SyntaxKind.ClassDeclaration,
                SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(TryAnalyzeSyntaxAutoParameter, SyntaxKind.FieldDeclaration);
        }
    }
}