using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using TMPEffects.StringLibrary;


namespace TMPEffects.AutoParameters
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    namespace TMPEffects.AutoParameters.Generator
    {
        public static class Utility
        {
            #region IDictionary<string,string>

            public static bool IsSyntaxIDictionaryStringString(SyntaxNodeAnalysisContext context, ParameterSyntax param,
                INamedTypeSymbol stringType)
            {
                var type = ModelExtensions.GetTypeInfo(context.SemanticModel, param.Type).Type as INamedTypeSymbol;
                if (type == null) return false;
                if (!type.IsGenericType) return false;
                if (type.ConstructedFrom.ToDisplayString() !=
                    "System.Collections.Generic.IDictionary<TKey, TValue>") return false;
                if (type.TypeArguments.Length != 2) return false;
                if (!SymbolEqualityComparer.Default.Equals(stringType, type.TypeArguments[0])) return false;
                if (!SymbolEqualityComparer.Default.Equals(stringType, type.TypeArguments[1])) return false;

                return true;
            }

            public static bool IsSymbolIDictionaryStringString(IParameterSymbol param, INamedTypeSymbol stringType)
            {
                if (!(param.Type is INamedTypeSymbol type)) return false;
                if (!type.IsGenericType) return false;
                if (type.ConstructedFrom.ToDisplayString() !=
                    "System.Collections.Generic.IDictionary<TKey, TValue>") return false;
                if (type.TypeArguments.Length != 2) return false;
                if (!SymbolEqualityComparer.Default.Equals(stringType, type.TypeArguments[0])) return false;
                if (!SymbolEqualityComparer.Default.Equals(stringType, type.TypeArguments[1])) return false;

                return true;
            }

            // Check whether the given type implements the given string type
            public static bool InheritsFrom(ITypeSymbol typeSymbol, string baseTypeName)
            {
                var currentSymbol = typeSymbol;

                while (currentSymbol != null)
                {
                    if (currentSymbol.ToDisplayString() == baseTypeName)
                    {
                        return true;
                    }

                    currentSymbol = currentSymbol.BaseType;
                }

                return false;
            }

            #endregion

            #region Parameter Validation Syntax Creation

            public static StatementSyntax GetValidationSyntax(string parametersName, IFieldSymbol fieldSymbol,
                AttributeData attrData, string keywordsPath)
            {
                if (attrData.ConstructorArguments.Length == 0) /*throw new System.ArgumentException();*/ return null;

                if (!Strings.TypeStringToDisplayString(fieldSymbol.Type, out string typeString))
                {
                    if (fieldSymbol.Type is IArrayTypeSymbol)
                        return GetArrayValidationSyntax(parametersName, fieldSymbol.Type as IArrayTypeSymbol, attrData);
                    if (fieldSymbol.Type.SpecialType == SpecialType.System_String)
                        return GetStringValidationSyntax(parametersName, fieldSymbol, attrData);
                    if (fieldSymbol.Type.ToDisplayString() == Strings.WaveName)
                        return GetWaveValidationSyntax(parametersName, fieldSymbol, attrData, keywordsPath);
                    throw new System.ArgumentException(nameof(fieldSymbol));
                }

                var arg0 = attrData.ConstructorArguments[0];
                int firstAliasIndex = 0;
                bool required = false;

                IEnumerable<ArgumentSyntax> arguments;
                InvocationExpressionSyntax invocation;
                ReturnStatementSyntax returnStatement;
                PrefixUnaryExpressionSyntax negationS;
                IfStatementSyntax ifStatementSyntaxxx;

                // If this is a required parameter            
                if (SpecifiesRequirement(attrData))
                {
                    firstAliasIndex++;
                    required = IsRequiredAutoParameter(attrData);
                }

                arguments =
                    new List<ArgumentSyntax>()
                        {
                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parametersName)),
                            SyntaxFactory.Argument(
                                SyntaxFactory.IdentifierName("context" + keywordsPath))
                        }
                        .Concat(attrData.ConstructorArguments.Skip(firstAliasIndex).SelectMany(tc =>
                                tc.Kind == TypedConstantKind.Array ? tc.Values.ToArray() : new TypedConstant[] { tc })
                            .Select(
                                val =>
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            SyntaxFactory.Literal((string)val.Value)))));

                returnStatement =
                    SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression));

                if (required)
                {
                    invocation = SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName(string.Format(Strings.HasTypeParameterName, typeString)),
                        SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

                    negationS = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, invocation);
                    ifStatementSyntaxxx = SyntaxFactory.IfStatement(negationS, returnStatement);
                }
                else
                {
                    invocation = SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName(string.Format(Strings.HasNonTypeParameterName, typeString)),
                        SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

                    ifStatementSyntaxxx = SyntaxFactory.IfStatement(invocation, returnStatement);
                }

                return ifStatementSyntaxxx;
            }

            private static StatementSyntax GetArrayValidationSyntax(string parametersName, IArrayTypeSymbol fieldSymbol,
                AttributeData attrData)
            {
                if (!Strings.TypeStringToDisplayString(fieldSymbol.ElementType, out string typeString))
                {
                    if (fieldSymbol.ElementType.SpecialType == SpecialType.System_String)
                        return GetStringArrayValidationSyntax(parametersName, fieldSymbol, attrData);

                    throw new System.ArgumentException(nameof(fieldSymbol));
                }

                var arg0 = attrData.ConstructorArguments[0];
                int firstAliasIndex = 0;
                bool required = false;

                IEnumerable<ArgumentSyntax> arguments;
                InvocationExpressionSyntax invocation;
                ReturnStatementSyntax returnStatement;
                PrefixUnaryExpressionSyntax negationS;
                IfStatementSyntax ifStatementSyntaxxx;

                // If this is a required parameter            
                if (SpecifiesRequirement(attrData))
                {
                    firstAliasIndex++;
                    required = IsRequiredAutoParameter(attrData);
                }

                arguments = new List<ArgumentSyntax>()
                {
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parametersName)),
                    SyntaxFactory.Argument(
                        SyntaxFactory.IdentifierName(string.Format(Strings.StringToTypeName, typeString)))
                }.Concat(attrData.ConstructorArguments.Skip(firstAliasIndex).SelectMany(tc =>
                    tc.Kind == TypedConstantKind.Array ? tc.Values.ToArray() : new TypedConstant[] { tc }).Select(val =>
                    SyntaxFactory.Argument(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            SyntaxFactory.Literal((string)val.Value)))));

                returnStatement =
                    SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression));

                if (required)
                {
                    var genericName = SyntaxFactory.GenericName(
                        SyntaxFactory.Identifier(string.Format(Strings.HasTypeParameterName, "Array")),
                        SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                SyntaxFactory.IdentifierName(fieldSymbol.ElementType.ToDisplayString())))
                    );

                    invocation = SyntaxFactory.InvocationExpression(
                        genericName,
                        SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments))
                    );

                    negationS = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, invocation);
                    ifStatementSyntaxxx = SyntaxFactory.IfStatement(negationS, returnStatement);
                }
                else
                {
                    var genericName = SyntaxFactory.GenericName(
                        SyntaxFactory.Identifier(string.Format(Strings.HasNonTypeParameterName, "Array")),
                        SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                SyntaxFactory.IdentifierName(fieldSymbol.ElementType.ToDisplayString())))
                    );

                    invocation = SyntaxFactory.InvocationExpression(
                        genericName,
                        SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments))
                    );

                    ifStatementSyntaxxx = SyntaxFactory.IfStatement(invocation, returnStatement);
                }

                return ifStatementSyntaxxx;
            }

            private static StatementSyntax GetStringArrayValidationSyntax(string parametersName,
                IArrayTypeSymbol fieldSymbol, AttributeData attrData)
            {
                var arg0 = attrData.ConstructorArguments[0];
                int firstAliasIndex = 0;
                bool required = false;

                IEnumerable<ArgumentSyntax> arguments;
                InvocationExpressionSyntax invocation;
                ReturnStatementSyntax returnStatement;
                PrefixUnaryExpressionSyntax negationS;
                IfStatementSyntax ifStatementSyntaxxx;

                // If this is a required parameter                    
                if (SpecifiesRequirement(attrData))
                {
                    firstAliasIndex++;
                    required = IsRequiredAutoParameter(attrData);
                }

                arguments =
                    new List<ArgumentSyntax>() { SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parametersName)) }
                        .Concat(attrData.ConstructorArguments.Skip(firstAliasIndex).SelectMany(tc =>
                                tc.Kind == TypedConstantKind.Array ? tc.Values.ToArray() : new TypedConstant[] { tc })
                            .Select(
                                val =>
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            SyntaxFactory.Literal((string)val.Value)))));

                returnStatement =
                    SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression));

                if (required)
                {
                    invocation = SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName(string.Format(Strings.HasTypeParameterName, "Array")),
                        SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

                    negationS = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, invocation);
                    ifStatementSyntaxxx = SyntaxFactory.IfStatement(negationS, returnStatement);
                }
                else
                {
                    invocation = SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName(string.Format(Strings.HasNonTypeParameterName, "Array")),
                        SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

                    ifStatementSyntaxxx = SyntaxFactory.IfStatement(invocation, returnStatement);
                }

                return ifStatementSyntaxxx;
            }

            private static StatementSyntax GetStringValidationSyntax(string parametersName, IFieldSymbol fieldSymbol,
                AttributeData attrData)
            {
                if (IsRequiredAutoParameter(attrData))
                {
                    var returnStatement =
                        SyntaxFactory.ReturnStatement(
                            SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression));
                    var arguments =
                        new List<ArgumentSyntax>()
                                { SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parametersName)) }
                            .Concat(attrData.ConstructorArguments.Skip(1).SelectMany(tc =>
                                    tc.Kind == TypedConstantKind.Array
                                        ? tc.Values.ToArray()
                                        : new TypedConstant[] { tc })
                                .Select(val =>
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            SyntaxFactory.Literal((string)val.Value)))));

                    var invocation = SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName(Strings.ParameterDefinedName),
                        SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

                    var ifStatementSyntaxxx = SyntaxFactory.IfStatement(invocation, returnStatement);
                    return ifStatementSyntaxxx;
                }
                else
                {
                    return SyntaxFactory.EmptyStatement();
                }
            }

            // Wave special case handling
            private static StatementSyntax GetWaveValidationSyntax(string parametersName, IFieldSymbol fieldSymbol,
                AttributeData attrData, string keywordsPath)
            {
                return SyntaxFactory.ParseStatement(
                    $"if (!{Strings.ValidateWaveParameters}({parametersName}, " +
                    $"{"context" + keywordsPath}, " +
                    $"\"{(string)attrData.ConstructorArguments[0].Value}\")) return false;");
            }

            #endregion

            #region Parameter Setting Syntax Creation

            public static StatementSyntax GetSetParameterSyntax(string parametersName, string storageName,
                IFieldSymbol fieldSymbol, AttributeData attrData, string keywordsPath)
            {
                if (attrData.ConstructorArguments.Length == 0) return null;

                if (!Strings.TypeStringToDisplayString(fieldSymbol.Type, out string typeString))
                {
                    if (fieldSymbol.Type is IArrayTypeSymbol)
                        return GetSetArraySyntax(parametersName, storageName, fieldSymbol,
                            fieldSymbol.Type as IArrayTypeSymbol, attrData);
                    if (fieldSymbol.Type.SpecialType == SpecialType.System_String)
                        return GetSetStringSyntax(parametersName, storageName, fieldSymbol, attrData);
                    if (fieldSymbol.Type.ToDisplayString() == Strings.WaveName)
                        return GetSetWaveSyntax(parametersName, storageName, fieldSymbol, attrData, keywordsPath);
                    throw new System.ArgumentException(nameof(fieldSymbol));
                }

                var arg0 = attrData.ConstructorArguments[0];
                int firstAliasIndex = 0;

                // If this is a required parameter            
                if (SpecifiesRequirement(attrData))
                {
                    firstAliasIndex++;
                }

                IEnumerable<ArgumentSyntax> arguments;
                InvocationExpressionSyntax invocation;
                IfStatementSyntax ifStatementSyntaxxx;
                ArgumentSyntax outArg;
                ExpressionStatementSyntax assignment;

                outArg = SyntaxFactory.Argument(SyntaxFactory.DeclarationExpression(SyntaxFactory.IdentifierName("var"),
                    SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier(fieldSymbol.Name))));

                arguments = new List<ArgumentSyntax>()
                {
                    outArg.WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)),
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parametersName)),
                    SyntaxFactory.Argument(
                        SyntaxFactory.IdentifierName("context" + keywordsPath))
                }.Concat(attrData.ConstructorArguments.Skip(firstAliasIndex).SelectMany(tc =>
                    tc.Kind == TypedConstantKind.Array ? tc.Values.ToArray() : new TypedConstant[] { tc }).Select(val =>
                    SyntaxFactory.Argument(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            SyntaxFactory.Literal((string)val.Value)))));

                invocation = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName(string.Format(Strings.TryGetTypeParameterName, typeString)),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

                assignment = SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(storageName), SyntaxFactory.IdentifierName(fieldSymbol.Name)),
                    SyntaxFactory.IdentifierName(fieldSymbol.Name)));

                ifStatementSyntaxxx = SyntaxFactory.IfStatement(invocation, assignment);

                return ifStatementSyntaxxx;
            }

            private static StatementSyntax GetSetArraySyntax(string parametersName, string storageName,
                IFieldSymbol fieldSymbol, IArrayTypeSymbol arrayTypeSymbol, AttributeData attrData)
            {
                if (!Strings.TypeStringToDisplayString(arrayTypeSymbol.ElementType, out var typeString))
                {
                    if (arrayTypeSymbol.ElementType.SpecialType == SpecialType.System_String)
                        return GetSetStringArraySyntax(parametersName, storageName, fieldSymbol, arrayTypeSymbol,
                            attrData);

                    throw new System.ArgumentException(nameof(fieldSymbol));
                }

                var arg0 = attrData.ConstructorArguments[0];
                int firstAliasIndex = 0;
                bool required = false;

                IEnumerable<ArgumentSyntax> arguments;
                InvocationExpressionSyntax invocation;
                IfStatementSyntax ifStatementSyntaxxx;

                // If this is a required parameter                      
                if (SpecifiesRequirement(attrData))
                {
                    firstAliasIndex++;
                    required = IsRequiredAutoParameter(attrData);
                }

                var outArg = SyntaxFactory.Argument(SyntaxFactory.DeclarationExpression(
                    SyntaxFactory.IdentifierName("var"),
                    SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier(fieldSymbol.Name))));

                arguments = new List<ArgumentSyntax>()
                {
                    outArg.WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)),
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parametersName)),
                    SyntaxFactory.Argument(
                        SyntaxFactory.IdentifierName(string.Format(Strings.StringToTypeName, typeString)))
                }.Concat(attrData.ConstructorArguments.Skip(firstAliasIndex).SelectMany(tc =>
                    tc.Kind == TypedConstantKind.Array ? tc.Values.ToArray() : new TypedConstant[] { tc }).Select(val =>
                    SyntaxFactory.Argument(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            SyntaxFactory.Literal((string)val.Value)))));

                var genericName = SyntaxFactory.GenericName(
                    SyntaxFactory.Identifier(string.Format(Strings.TryGetTypeParameterName, "Array")),
                    SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                            SyntaxFactory.IdentifierName(
                                arrayTypeSymbol.ElementType.ToDisplayString()
                            )))
                );

                invocation = SyntaxFactory.InvocationExpression(
                    genericName,
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments))
                );

                var assignment = SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(storageName), SyntaxFactory.IdentifierName(fieldSymbol.Name)),
                    SyntaxFactory.IdentifierName(fieldSymbol.Name)));

                ifStatementSyntaxxx = SyntaxFactory.IfStatement(invocation, assignment);

                return ifStatementSyntaxxx;
            }

            private static StatementSyntax GetSetStringArraySyntax(string parametersName, string storageName,
                IFieldSymbol fieldSymbol, IArrayTypeSymbol arrayTypeSymbol, AttributeData attrData)
            {
                var arg0 = attrData.ConstructorArguments[0];
                int firstAliasIndex = 0;
                bool required = false;

                IEnumerable<ArgumentSyntax> arguments;
                InvocationExpressionSyntax invocation;
                IfStatementSyntax ifStatementSyntaxxx;
                ExpressionStatementSyntax assignment;

                // If this is a required parameter            
                if (SpecifiesRequirement(attrData))
                {
                    firstAliasIndex++;
                    required = IsRequiredAutoParameter(attrData);
                }

                var outArg = SyntaxFactory.Argument(SyntaxFactory.DeclarationExpression(
                    SyntaxFactory.IdentifierName("var"),
                    SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier(fieldSymbol.Name))));

                arguments = new List<ArgumentSyntax>()
                {
                    outArg.WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)),
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parametersName))
                }.Concat(attrData.ConstructorArguments.Skip(firstAliasIndex).SelectMany(tc =>
                    tc.Kind == TypedConstantKind.Array ? tc.Values.ToArray() : new TypedConstant[] { tc }).Select(val =>
                    SyntaxFactory.Argument(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            SyntaxFactory.Literal((string)val.Value)))));

                invocation = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName(string.Format(Strings.TryGetTypeParameterName, "Array")),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

                assignment = SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(storageName), SyntaxFactory.IdentifierName(fieldSymbol.Name)),
                    SyntaxFactory.IdentifierName(fieldSymbol.Name)));

                ifStatementSyntaxxx = SyntaxFactory.IfStatement(invocation, assignment);
                return ifStatementSyntaxxx;
            }

            private static StatementSyntax GetSetStringSyntax(string parametersName, string storageName,
                IFieldSymbol fieldSymbol, AttributeData attrData)
            {
                int firstAliasIndex = 0;
                if (SpecifiesRequirement(attrData))
                {
                    firstAliasIndex++;
                }

                var outArg = SyntaxFactory.Argument(SyntaxFactory.DeclarationExpression(
                    SyntaxFactory.IdentifierName("var"),
                    SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier(fieldSymbol.Name))));

                var arguments =
                    new List<ArgumentSyntax>()
                    {
                        outArg.WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)),
                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parametersName))
                    }.Concat(attrData.ConstructorArguments.Skip(firstAliasIndex).SelectMany(tc =>
                        tc.Kind == TypedConstantKind.Array ? tc.Values.ToArray() : new TypedConstant[] { tc }).Select(
                        val =>
                            SyntaxFactory.Argument(
                                SyntaxFactory.LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    SyntaxFactory.Literal((string)val.Value)))));

                var invocation = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName(Strings.TryGetDefinedParameter),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

                var assignment = SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(storageName), SyntaxFactory.IdentifierName(fieldSymbol.Name)),
                    SyntaxFactory.IdentifierName(fieldSymbol.Name)));

                var ifStatementSyntaxxx = SyntaxFactory.IfStatement(invocation, assignment);

                return ifStatementSyntaxxx;
            }

            private static StatementSyntax GetSetWaveSyntax(string parametersName, string storageName,
                IFieldSymbol fieldSymbol, AttributeData attrData, string keywordsPath)
            {
                return SyntaxFactory.ParseStatement($"{storageName}.{fieldSymbol.Name} = " +
                                                    $"{Strings.ParameterUtilityPath}.CreateWave({storageName}.{fieldSymbol.Name}, " +
                                                    $"{Strings.ParameterUtilityPath}.GetWaveParameters({parametersName}, " +
                                                    $"{"context" + keywordsPath}, " +
                                                    $"\"{(string)attrData.ConstructorArguments[0].Value}\"));");
            }

            #endregion


            public static bool Implements(INamedTypeSymbol symbol, string interfacename)
            {
                foreach (var interfaceSymbol in symbol.AllInterfaces)
                {
                    if (interfaceSymbol.ToDisplayString() == interfacename)
                    {
                        return true;
                    }
                }

                return false;
            }

            #region Get AutoParameters

            public static List<AutoParameterInfo> GetAutoParametersNEW(INamedTypeSymbol typeSymbol)
            {
                List<AutoParameterInfo> autoParameters = new List<AutoParameterInfo>();

                IFieldSymbol field;
                AttributeData attrData;
                foreach (var member in typeSymbol.GetMembers())
                {
                    field = member as IFieldSymbol;
                    if (field == null) continue;

                    if (TryGetAutoParameterInfo(field, out var info))
                    {
                        autoParameters.Add(info);
                    }
                }

                return autoParameters;
            }

            public static List<AutoParameterBundleInfo> GetAutoParameterBundlesNEW(INamedTypeSymbol typeSymbol)
            {
                List<AutoParameterBundleInfo> autoParameters = new List<AutoParameterBundleInfo>();

                IFieldSymbol field;
                AttributeData attrData;
                foreach (var member in typeSymbol.GetMembers())
                {
                    field = member as IFieldSymbol;
                    if (field == null) continue;

                    if (TryGetAutoParameterBundleInfo(field, out var info))
                    {
                        autoParameters.Add(info);
                    }
                }

                return autoParameters;
            }
            
            public static List<(IFieldSymbol, AttributeData)> GetAutoParameters(INamedTypeSymbol typeSymbol)
            {
                List<(IFieldSymbol, AttributeData)> autoParameters = new List<(IFieldSymbol, AttributeData)>();

                IFieldSymbol field;
                AttributeData attrData;
                foreach (var member in typeSymbol.GetMembers())
                {
                    field = member as IFieldSymbol;
                    if (field == null) continue;

                    if ((attrData = Utility.IsValidAutoParameter(field)) != null)
                    {
                        autoParameters.Add((field, attrData));
                    }
                }

                return autoParameters;
            }

            public static List<(IFieldSymbol, AttributeData)> GetAutoParameterBundles(INamedTypeSymbol typeSymbol)
            {
                List<(IFieldSymbol, AttributeData)> autoParameters = new List<(IFieldSymbol, AttributeData)>();

                IFieldSymbol field;
                AttributeData attrData;
                foreach (var member in typeSymbol.GetMembers())
                {
                    field = member as IFieldSymbol;
                    if (field == null) continue;

                    if ((attrData = Utility.IsValidAutoParameterBundle(field)) != null)
                    {
                        autoParameters.Add((field, attrData));
                    }
                }

                return autoParameters;
            }

            public static INamedTypeSymbol GetAutoParametersStorage(INamedTypeSymbol typeSymbol)
            {
                foreach (var member in typeSymbol.GetMembers())
                {
                    INamedTypeSymbol type = member as INamedTypeSymbol;
                    if (type == null) continue;

                    var attributes = type.GetAttributes();
                    foreach (var attribute in attributes)
                    {
                        if (attribute?.AttributeClass.ToDisplayString() == Strings.AutoParametersStorageAttributeName)
                        {
                            return type;
                        }
                    }
                }

                return null;
            }

            #endregion

            #region Auto Parameter Checks

            // Check whether the required parameter is set in the given AutoParameter attribute
            public static bool SpecifiesRequirement(AttributeData attrData)
            {
                if (attrData == null) throw new System.ArgumentNullException(nameof(attrData));
                if (attrData.AttributeClass.ToDisplayString() != Strings.AutoParameterAttributeName) return false;
                if (attrData.ConstructorArguments.Count() < 1) throw new System.ArgumentException(nameof(attrData));
                return attrData.ConstructorArguments[0].Type.SpecialType == SpecialType.System_Boolean;
            }

            // Check whether the required parameter is set to true in the given AutoParameter attribute
            public static bool IsRequiredAutoParameter(AttributeData attrData)
            {
                if (attrData == null) throw new System.ArgumentNullException(nameof(attrData));
                if (attrData.AttributeClass.ToDisplayString() != Strings.AutoParameterAttributeName) return false;
                if (attrData.ConstructorArguments.Count() < 1) throw new System.ArgumentException(nameof(attrData));
                if (attrData.ConstructorArguments[0].Type.SpecialType != SpecialType.System_Boolean) return false;
                if (attrData.ConstructorArguments.Count() < 2) throw new System.ArgumentException(nameof(attrData));
                return (bool)attrData.ConstructorArguments[0].Value;
            }


            public static bool TryGetAutoParameterBundleInfo(IFieldSymbol symbol, out AutoParameterBundleInfo info)
            {
                info = new AutoParameterBundleInfo();

                // Check if is a valid AutoParameter
                if (!IsDecoratedAsAutoParameterBundle(symbol, out var attData)) return false;
                if (!TryGetClosestFitAutoParamBundleType(symbol, out var fitType)) return false;
                
                // Parse name & aliases
                var arguments = attData.ConstructorArguments.SelectMany(tc =>
                        tc.Kind == TypedConstantKind.Array ? tc.Values.ToArray() : new TypedConstant[] { tc })
                    .Select(val => val.Value as string);

                info.prefix = arguments.First() as string;
                
                info.TypeSymbol = fitType;
                info.TypeString = fitType.ToDisplayString();
                Strings.TypeStringToDisplayString(fitType, out info.NameString);
                info.RawData = attData;

                return true;
            }
            
            public static bool TryGetAutoParameterInfo(IFieldSymbol symbol, out AutoParameterInfo info)
            {
                info = new AutoParameterInfo();

                // Check if is a valid AutoParameter
                if (!IsDecoratedAsAutoParameter(symbol, out var attData)) return false;
                if (!TryGetClosestFitAutoParamType(symbol, out var fitType)) return false;

                // Check whether requirement specified
                info.specifiesRequirement = SpecifiesRequirement(attData);

                int index = 0;
                if (info.specifiesRequirement)
                {
                    index++;
                    info.required = (bool)attData.ConstructorArguments[0].Value;
                }
                else
                {
                    info.required = false;
                }


                // Parse name & aliases
                var arguments = attData.ConstructorArguments.Skip(index).SelectMany(tc =>
                        tc.Kind == TypedConstantKind.Array ? tc.Values.ToArray() : new TypedConstant[] { tc })
                    .Select(val => val.Value as string);

                info.AliasesWithName = arguments.ToArray();
                info.FirstAlias = info.AliasesWithName.First();
                info.Aliases = info.AliasesWithName.Skip(1).ToArray();

                info.TypeSymbol = fitType;
                info.TypeString = fitType.ToDisplayString();
                Strings.TypeStringToDisplayString(fitType, out info.NameString);
                info.RawData = attData;

                return true;
            }

            private static bool IsDecoratedAsAutoParameter(IFieldSymbol symbol, out AttributeData attData)
            {
                var attributes = symbol.GetAttributes();
                foreach (var attribute in attributes)
                {
                    var baseClass = attribute?.AttributeClass;
                    while (baseClass != null)
                    {
                        if (baseClass.ToDisplayString() == Strings.AutoParameterAttributeName)
                        {
                            attData = attribute;
                            return true;
                        }

                        baseClass = baseClass.BaseType;
                    }
                }

                attData = null;
                return false;
            }

            private static bool IsDecoratedAsAutoParameterBundle(IFieldSymbol symbol, out AttributeData attData)
            {
                var attributes = symbol.GetAttributes();
                foreach (var attribute in attributes)
                {
                    var baseClass = attribute?.AttributeClass;
                    while (baseClass != null)
                    {
                        if (baseClass.ToDisplayString() == Strings.AutoParameterBundleAttributeName)
                        {
                            attData = attribute;
                            return true;
                        }

                        baseClass = baseClass.BaseType;
                    }
                }

                attData = null;
                return false;
            }
            
            private static bool TryGetClosestFitAutoParamType(IFieldSymbol symbol, out ITypeSymbol fit)
            {
                fit = symbol.Type;

                // Check direct type
                if (Strings.IsValidAutoParameterType(fit))
                    return true;

                // All base types
                var tmp = fit.BaseType;
                while (tmp != null)
                {
                    if (IsValidAutoParameterType(tmp))
                    {
                        fit = tmp;
                        return true;
                    }

                    tmp = tmp.BaseType;
                }

                // All interfaces
                foreach (var i in fit.AllInterfaces)
                {
                    if (Strings.IsValidAutoParameterType(i))
                    {
                        fit = i;
                        return true;
                    }
                }

                return false;
            }

            private static bool TryGetClosestFitAutoParamBundleType(IFieldSymbol symbol, out ITypeSymbol fit)
            {
                fit = symbol.Type;

                // Check direct type
                if (Strings.IsValidAutoParameterBundleType(fit))
                    return true;

                // All base types
                var tmp = fit.BaseType;
                while (tmp != null)
                {
                    if (IsValidAutoParameterBundleType(tmp))
                    {
                        fit = tmp;
                        return true;
                    }

                    tmp = tmp.BaseType;
                }

                // All interfaces
                foreach (var i in fit.AllInterfaces)
                {
                    if (Strings.IsValidAutoParameterBundleType(i))
                    {
                        fit = i;
                        return true;
                    }
                }

                return false;
            }

            public struct AutoParameterInfo
            {
                public ITypeSymbol TypeSymbol;

                public string TypeString;
                public string NameString;

                public bool specifiesRequirement;
                public bool required;

                public string FirstAlias;
                public string[] Aliases;
                public string[] AliasesWithName;
                public AttributeData RawData;
            }

            public struct AutoParameterBundleInfo
            {
                public ITypeSymbol TypeSymbol;

                public string TypeString;
                public string NameString;

                public string prefix;

                public AttributeData RawData;
            }

            [Flags]
            public enum AutoParametersDecoration : byte
            {
                None = 0,
                AutoParameter = 1,
                AutoParameterBundle = 1 << 1,
                All = byte.MaxValue

                // TODO Put other stuff in here to do to be more universally reusable?
                // AutoParameterStorage = 1 << 2,
                // AutoParameters = 1 << 3,
            }


            public static bool IsValidAutoParameter(IFieldSymbol fieldSymbol, out AttributeData attData)
            {
                attData = IsValidAutoParameter(fieldSymbol);
                return attData != null;
            }

            // Check whether the given field is a valid AutoParameter
            public static AttributeData IsValidAutoParameter(IFieldSymbol fieldSymbol)
            {
                if (IsValidAutoParameterType(fieldSymbol.Type))
                {
                    return IsAutoParameter(fieldSymbol);
                }

                return null;
            }

            // Check whether the given type is valid to be an AutoParameter
            public static bool IsValidAutoParameterType(ITypeSymbol typeSymbol)
            {
                if (Strings.IsValidAutoParameterType(typeSymbol))
                    return true;

                return false;
            }

            public static bool IsAutoParameter(SyntaxNodeAnalysisContext context, FieldDeclarationSyntax field)
            {
                foreach (var variable in field.Declaration.Variables)
                {
                    var fieldSymbol =
                        ModelExtensions.GetDeclaredSymbol(context.SemanticModel, variable) as IFieldSymbol;
                    if (IsAutoParameter(fieldSymbol) != null) return true;
                }

                return false;
            }

            // Check whether the given field is decorated with AutoParameter
            public static AttributeData IsAutoParameter(IFieldSymbol fieldSymbol)
            {
                var attributes = fieldSymbol.GetAttributes();
                foreach (var attribute in attributes)
                {
                    var baseClass = attribute?.AttributeClass;
                    while (baseClass != null)
                    {
                        if (baseClass.ToDisplayString() == Strings.AutoParameterAttributeName)
                        {
                            return attribute;
                        }

                        baseClass = baseClass.BaseType;
                    }
                }

                return null;
            }

            // Check whether the given field is a valid AutoParameterBundle
            public static AttributeData IsValidAutoParameterBundle(IFieldSymbol fieldSymbol)
            {
                if (IsValidAutoParameterBundleType(fieldSymbol.Type))
                {
                    return IsAutoParameterBundle(fieldSymbol);
                }

                return null;
            }

            // Check whether the given type is valid to be an AutoParameterBundle
            public static bool IsValidAutoParameterBundleType(ITypeSymbol typeSymbol)
            {
                return Strings.IsValidAutoParameterBundleType(typeSymbol);
            }

            // Check whether the given field an AutoParameterBundle
            public static AttributeData IsAutoParameterBundle(IFieldSymbol fieldSymbol)
            {
                var attributes = fieldSymbol.GetAttributes();
                foreach (var attribute in attributes)
                {
                    var baseClass = attribute?.AttributeClass;
                    while (baseClass != null)
                    {
                        if (baseClass.ToDisplayString() == Strings.AutoParameterBundleAttributeName)
                        {
                            return attribute;
                        }

                        baseClass = baseClass.BaseType;
                    }
                }

                return null;
            }

            #endregion
        }
    }
}