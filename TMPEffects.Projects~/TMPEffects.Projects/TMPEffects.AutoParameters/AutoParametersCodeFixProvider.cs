using System.Collections.Generic;
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
using TMPEffects.AutoParameters.Analyzer;
using TMPEffects.StringLibrary;

namespace TMPEffects.AutoParameters.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AutoParametersCodeFixProvider)), Shared]
    public class AutoParametersCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(AutoParametersAnalyzer.DiagnosticId_5);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;


        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken)
                .ConfigureAwait(false);

            var nestedTypes =
                GetNestedTypesWithAttribute(root, semanticModel, Strings.AutoParametersStorageAttributeName);
            string name = nestedTypes.Any() ? nestedTypes.First().Name : Strings.DefaultStorageName;

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the class declaration identified by the diagnostic.
            var classDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf()
                .OfType<ClassDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Implement Animate",
                    createChangedDocument: c => ImplementPartialAnimate(context.Document, classDeclaration, c, name),
                    equivalenceKey: "Implement Animate"),
                diagnostic);
        }

        public static IEnumerable<INamedTypeSymbol> GetNestedTypesWithAttribute(SyntaxNode root,
            SemanticModel semanticModel, string attributeName)
        {
            var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

            foreach (var classDeclaration in classDeclarations)
            {
                var symbol = semanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
                if (symbol != null && HasAttribute(symbol, attributeName))
                {
                    yield return symbol;
                }

                foreach (var nestedType in GetNestedTypes(symbol, attributeName))
                {
                    yield return nestedType;
                }
            }
        }

        private static bool HasAttribute(INamedTypeSymbol symbol, string attributeName)
        {
            return symbol.GetAttributes().Any(attr => attr.AttributeClass.ToDisplayString() == attributeName);
        }

        private static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol symbol, string attributeName)
        {
            foreach (var nestedType in symbol.GetTypeMembers())
            {
                if (HasAttribute(nestedType, attributeName))
                {
                    yield return nestedType;
                }

                foreach (var nestedNestedType in GetNestedTypes(nestedType, attributeName))
                {
                    yield return nestedNestedType;
                }
            }
        }

        private async Task<Document> ImplementPartialAnimate(Document document, ClassDeclarationSyntax classDeclaration,
            CancellationToken cancellationToken, string storageName)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

            var methodDeclaration = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    "Animate")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                    SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("cData"))
                        .WithType(SyntaxFactory.IdentifierName("CharData")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("data"))
                        .WithType(SyntaxFactory.IdentifierName(storageName)),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("context"))
                        .WithType(SyntaxFactory.IdentifierName("IAnimationContext"))
                )
                .WithBody(SyntaxFactory.Block());

            // Add the method to the class
            var newClassDeclaration = classDeclaration.AddMembers(methodDeclaration);
            editor.ReplaceNode(classDeclaration, newClassDeclaration);

            // var newUsings = new[]
            // {
            //     Constants.CharDataPath,
            //     Constants.IAnimationContextPath
            // };
            //
            // var root = editor.OriginalRoot as CompilationUnitSyntax;
            // var usings = root.Usings.Select(ns => ns.Name.ToString());
            // var addUsings = newUsings.Where(ns => !usings.Contains(ns));
            //
            // foreach (var usng in addUsings)
            // {
            //     editor.InsertBefore(root.Members.First(), SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(usng)));
            // }

            return editor.GetChangedDocument();
        }
    }
}