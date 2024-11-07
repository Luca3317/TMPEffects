﻿using System.Collections.Generic;
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
        private MethodDeclarationSyntax CreateAnimationSetParameters(INamedTypeSymbol symbol, string storageName,
            GeneratorExecutionContext context, List<(IFieldSymbol, AttributeData)> parameters)
        {
            // Prepare the parameters
            var paramList = SyntaxFactory.ParameterList()
                .AddParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("customData"))
                    .WithType(SyntaxFactory.ParseTypeName("object")))
                .AddParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("parameters"))
                    .WithType(SyntaxFactory.ParseTypeName(Strings.IDictionaryName)))
                .AddParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("context"))
                    .WithType(SyntaxFactory.ParseTypeName(Strings.IAnimationContextName)));

            // Prepare the method
            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), "SetParameters")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword)).WithParameterList(paramList);

            // Prepare all statements
            var statements = new List<StatementSyntax>();

            var condition = SyntaxFactory.BinaryExpression(
                SyntaxKind.EqualsExpression, SyntaxFactory.IdentifierName("parameters"),
                SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));

            var returnStatement = SyntaxFactory.ReturnStatement();
            var ifStatement = SyntaxFactory.IfStatement(condition, returnStatement);

            statements.Add(ifStatement);

            statements.Add(SyntaxFactory.ParseStatement($"var d = ({storageName})customData;"));

            foreach (var param in parameters)
            {
                var setParameterSyntax = Utility2.GetSetParameterSyntax("parameters", "d", param.Item1, param.Item2,
                    Strings.IAnimationContextKeywordDatabasePath);
                if (setParameterSyntax != null) statements.Add(setParameterSyntax);
            }

            var stringType = context.Compilation.GetSpecialType(SpecialType.System_String);
            var hookCandidates = symbol.GetMembers().Where(member => member.Kind == SymbolKind.Method)
                .Select(member => member as IMethodSymbol).Where(hookMethod => hookMethod.Name == "SetParameters_Hook");
            bool present = false;
            foreach (var candidate in hookCandidates)
            {
                if (candidate.Parameters == null || candidate.Parameters.Length != 3) continue;

                // If first parameter is not object, continue
                if (candidate.Parameters[0].Type.SpecialType != SpecialType.System_Object) continue;

                // If second parameter is not IDictionary<string, string>, continue
                if (!Utility.IsSymbolIDictionaryStringString(candidate.Parameters[1], stringType)) continue;

                if (candidate.Parameters[2].Type.ToDisplayString() != Strings.IAnimationContextName) continue;

                present = true;
                break;
            }

            // Call hook
            if (present)
            {
                var arguments = new List<ArgumentSyntax>()
                {
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("customData")),
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameters")),
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("context"))
                };
                var hc = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("SetParameters_Hook"),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));
                statements.Add(SyntaxFactory.ExpressionStatement(hc));
            }

            returnStatement = SyntaxFactory.ReturnStatement();
            statements.Add(returnStatement);

            // Add statements to method body
            return method.WithBody(SyntaxFactory.Block(statements));
        }

        private MethodDeclarationSyntax CreateGetNewCustomData(INamedTypeSymbol symbol, string storageSymbol,
            GeneratorExecutionContext context, List<(IFieldSymbol, AttributeData)> parameters)
        {
            // Prepare the parameters
            var paramList = SyntaxFactory.ParameterList()
                .AddParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("context"))
                    .WithType(SyntaxFactory.ParseTypeName(Strings.IAnimationContextName)));

            // Prepare the method
            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("object"), "GetNewCustomData")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword)).WithParameterList(paramList);

            List<StatementSyntax> statements = new List<StatementSyntax>();

            var createStmnt = SyntaxFactory.ParseStatement($"var d = new {storageSymbol}();");
            statements.Add(createStmnt);

            foreach (var p in parameters)
            {
                statements.Add(SyntaxFactory.ParseStatement($"d.{p.Item1.Name} = this.{p.Item1.Name};"));
            }

            var hookCandidates = symbol.GetMembers().Where(member => member.Kind == SymbolKind.Method)
                .Select(member => member as IMethodSymbol)
                .Where(hookMethod => hookMethod.Name == "GetNewCustomData_Hook");
            bool present = false;
            foreach (var candidate in hookCandidates)
            {
                if (candidate.Parameters == null || candidate.Parameters.Length != 2) continue;

                if (candidate.Parameters[0].Type.SpecialType != SpecialType.System_Object) continue;
                if (candidate.Parameters[1].Type.ToDisplayString() != Strings.IAnimationContextName) continue;

                present = true;
                break;
            }

            // Call hook
            if (present)
            {
                var arguments = new List<ArgumentSyntax>()
                {
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("d")),
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("context"))
                };
                var hc = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("GetNewCustomData_Hook"),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));
                statements.Add(SyntaxFactory.ExpressionStatement(hc));
            }

            statements.Add(SyntaxFactory.ParseStatement("return d;"));

            // Add statements to method body
            return method.WithBody(SyntaxFactory.Block(statements));
        }

        private MethodDeclarationSyntax CreateAnimate(string storageDeclName)
        {
            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    "Animate")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("cData"))
                        .WithType(SyntaxFactory.IdentifierName(Strings.CharDataName)),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("context"))
                        .WithType(SyntaxFactory.IdentifierName(Strings.IAnimationContextName))
                )
                .WithBody(
                    SyntaxFactory.Block(
                        SyntaxFactory.LocalDeclarationStatement(
                            SyntaxFactory.VariableDeclaration(
                                    SyntaxFactory.IdentifierName(storageDeclName))
                                .WithVariables(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.VariableDeclarator("d")
                                            .WithInitializer(
                                                SyntaxFactory.EqualsValueClause(
                                                    SyntaxFactory.BinaryExpression(
                                                        SyntaxKind.AsExpression,
                                                        SyntaxFactory.MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            SyntaxFactory.IdentifierName("context"),
                                                            SyntaxFactory.IdentifierName("CustomData")),
                                                        SyntaxFactory.IdentifierName(storageDeclName))))))),
                        SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.IdentifierName("Animate"))
                                .WithArgumentList(
                                    SyntaxFactory.ArgumentList(
                                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                            new SyntaxNodeOrToken[]
                                            {
                                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("cData")),
                                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("d")),
                                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("context"))
                                            }))))
                    ));
        }

        private MethodDeclarationSyntax CreatePartialAnimate(string storageDeclName)
        {
            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    "Animate")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                    SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("cData"))
                        .WithType(SyntaxFactory.IdentifierName(Strings.CharDataName)),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("data"))
                        .WithType(SyntaxFactory.IdentifierName(storageDeclName)),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("context"))
                        .WithType(SyntaxFactory.IdentifierName(Strings.IAnimationContextName))
                )
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)).WithBody(null);
        }
    }
}