using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TMPEffects.AutoParameters.TMPEffects.AutoParameters.Generator;
using TMPEffects.StringLibrary;

namespace TMPEffects.AutoParameters.Generator.Generator
{
    public partial class AutoParametersGenerator
    {
        private MethodDeclarationSyntax CreateExecute(string storageDeclName)
        {
            string code = $@"{{
    {storageDeclName} d = ({storageDeclName}) context.CustomData;
    ExecuteCommand(d, context);
}}";

            BlockSyntax blockSyntax = SyntaxFactory.ParseStatement(code) as BlockSyntax;

            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    "ExecuteCommand")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("context"))
                        .WithType(SyntaxFactory.IdentifierName(Strings.ICommandContextName))
                )
                .WithBody(blockSyntax);
        }

        private MethodDeclarationSyntax CreatePartialExecute(string storageDeclName)
        {
            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    "ExecuteCommand")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                    SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("data"))
                        .WithType(SyntaxFactory.IdentifierName(storageDeclName)),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("context"))
                        .WithType(SyntaxFactory.IdentifierName(Strings.ICommandContextName))
                )
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)).WithBody(null);
        }
    }
}