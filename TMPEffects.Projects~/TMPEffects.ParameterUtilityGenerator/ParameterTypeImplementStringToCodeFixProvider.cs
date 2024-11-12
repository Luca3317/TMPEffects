using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using TMPEffects.StringLibrary;

namespace TMPEffects.ParameterUtilityGenerator
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ParameterTypeImplementStringToCodeFixProvider)), Shared]
    public class ParameterTypeImplementStringToCodeFixProvider : CodeFixProvider
    {
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken)
                .ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the class declaration identified by the diagnostic.
            var attributeSyntax = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf()
                .OfType<AttributeSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Implement StringTo",
                    createChangedDocument: c => ImplementStringTo(context.Document, attributeSyntax, c),
                    equivalenceKey: "Implement StringTo"),
                diagnostic);
        }

        private async Task<Document> ImplementStringTo(Document document, AttributeSyntax attributeSyntax,
            CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

            if (!Utility.TryGetParameterTypeDisplayName(attributeSyntax, out var displayName))
                return editor.GetChangedDocument();

            var typeDecl = attributeSyntax.Ancestors().OfType<TypeDeclarationSyntax>().First();

            var methodDeclaration = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    "StringTo" + displayName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                    SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("str"))
                        .WithType(SyntaxFactory.IdentifierName("string")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("result"))
                        .WithType(SyntaxFactory.IdentifierName(typeDecl.Identifier.Text))
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.OutKeyword)),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("keywords"))
                        .WithType(SyntaxFactory.IdentifierName(Strings.ITMPKeywordDatabaseName))
                )
                .WithBody(SyntaxFactory.Block())
                .WithReturnType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)));

            // Add the method to the class
            var newClassDeclaration = typeDecl.AddMembers(methodDeclaration);
            editor.ReplaceNode(typeDecl, newClassDeclaration);

            return editor.GetChangedDocument();
        }

        public override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(ParameterTypeAnalyzer.DiagnosticId_106);
    }
}