﻿using System.Collections.Generic;
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
using TMPEffects.AutoParameters.TMPEffects.AutoParameters.Generator;
using TMPEffects.StringLibrary;

namespace TMPEffects.AutoParameters.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AutoParametersSetParametersHookFix)), Shared]
    public class AutoParametersSetParametersHookFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create
                (
                    AutoParametersAnalyzer.DiagnosticId_901
                );
            }
        }

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
                    title: "Implement SetParameters hook",
                    createChangedDocument: c => ImplementSetParametersHook(context, context.Document, classDeclaration, c, name),
                    equivalenceKey: "Implement SetParameters hook"),
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

        private async Task<Document> ImplementSetParametersHook(CodeFixContext context, Document document, ClassDeclarationSyntax classDeclaration,
            CancellationToken cancellationToken, string storageName)
        {
            var usingDirective =
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Systems.Collections.Generic"));
            var usingDirective2 =
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(Strings.ITMPKeywordDatabasePath));

            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var methodDeclaration = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    "SetParameters_Hook")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("customData"))
                        .WithType(SyntaxFactory.IdentifierName("object")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("parameters"))
                        .WithType(SyntaxFactory.IdentifierName("IDictionary<string, string>")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("keywordDatabase"))
                        .WithType(SyntaxFactory.IdentifierName(Strings.ITMPKeywordDatabase))
                )
                .WithBody(SyntaxFactory.Block());

            var compilationUnit = root as CompilationUnitSyntax;

            var newclassdecl = classDeclaration.AddMembers(methodDeclaration);
            compilationUnit = compilationUnit.ReplaceNode(classDeclaration, newclassdecl);

            if (compilationUnit.Usings.All(u => u.Name.ToString() != "System.Collections.Generic"))
            {
                compilationUnit =
                    compilationUnit.AddUsings(usingDirective);
            }
            if (compilationUnit.Usings.All(u => u.Name.ToString() != Strings.ITMPKeywordDatabasePath))
            {
                compilationUnit =
                    compilationUnit.AddUsings(usingDirective2);
            }

            var newdocument = document.WithSyntaxRoot(compilationUnit);

            return newdocument;
        }
    }
}