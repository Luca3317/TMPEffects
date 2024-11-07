using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TMPEffects.StringLibrary;

namespace TMPEffects.AutoParameters.Generator.Generator
{
    public partial class AutoParametersGenerator
    {
        private MethodDeclarationSyntax CreateCommandSetParameters(INamedTypeSymbol symbol, string storageName,
            GeneratorExecutionContext context, List<(IFieldSymbol, AttributeData)> parameters)
        {
            // Prepare the parameters
            var paramList = SyntaxFactory.ParameterList()
                .AddParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("parameters"))
                    .WithType(SyntaxFactory.ParseTypeName(Strings.IDictionaryName)))
                .AddParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("context"))
                    .WithType(SyntaxFactory.ParseTypeName(Strings.ICommandContextName)));

            // Prepare the method
            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(storageName), "SetParameters")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)).WithParameterList(paramList);

            // Prepare all statements
            var statements = new List<StatementSyntax>();

            var condition = SyntaxFactory.BinaryExpression(
                SyntaxKind.EqualsExpression, SyntaxFactory.IdentifierName("parameters"),
                SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));

            var returnStatement = SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("d"));
            var ifStatement = SyntaxFactory.IfStatement(condition, returnStatement);

            statements.Add(SyntaxFactory.ParseStatement($"var d = new " + storageName + "();"));
            statements.Add(ifStatement);

            foreach (var param in parameters)
            {
                var setParameterSyntax = Utility.GetSetParameterSyntax("parameters", "d", param.Item1, param.Item2,
                    Strings.ICommandContexKeywordDatabasePath);
                if (setParameterSyntax != null) statements.Add(setParameterSyntax);
            }


            var stringType = context.Compilation.GetSpecialType(SpecialType.System_String);
            var hookCandidates = symbol.GetMembers().Where(member => member.Kind == SymbolKind.Method)
                .Select(member => member as IMethodSymbol).Where(hookMethod => hookMethod.Name == "SetParameters_Hook");
            bool present = false;
            foreach (var candidate in hookCandidates)
            {
                if (candidate.Parameters == null || candidate.Parameters.Length != 3) continue;

                // If first parameter is not storage, continue
                if (candidate.Parameters[0].Type.ToDisplayString() != storageName) continue;

                // If second parameter is not IDictionary<string, string>, continue
                if (!Utility.IsSymbolIDictionaryStringString(candidate.Parameters[1], stringType)) continue;

                // If thirs parameter is not context, continue
                if (candidate.Parameters[2].Type.ToDisplayString() != Strings.ICommandContextName) continue;

                present = true;
                break;
            }

            // Call hook
            if (present)
            {
                var arguments = new List<ArgumentSyntax>()
                {
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("d")),
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameters")),
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("context"))
                };
                var hc = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("SetParameters_Hook"),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));
                statements.Add(SyntaxFactory.ExpressionStatement(hc));
            }

            returnStatement = SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("d"));
            statements.Add(returnStatement);

            // Add statements to method body
            return method.WithBody(SyntaxFactory.Block(statements));
        }

        private MethodDeclarationSyntax CreateExecute(string storageDeclName)
        {
            string code = $@"{{
    {storageDeclName} d = SetParameters(parameters, context);
    ExecuteCommand(parameters, d, context);
}}";

            BlockSyntax blockSyntax = SyntaxFactory.ParseStatement(code) as BlockSyntax;

            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    "ExecuteCommand")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("parameters"))
                        .WithType(SyntaxFactory.IdentifierName(Strings.IDictionaryName)),
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
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("parameters"))
                        .WithType(SyntaxFactory.IdentifierName(Strings.IDictionaryName)),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("data"))
                        .WithType(SyntaxFactory.IdentifierName(storageDeclName)),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("context"))
                        .WithType(SyntaxFactory.IdentifierName(Strings.ICommandContextName))
                )
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)).WithBody(null);
        }
    }
}