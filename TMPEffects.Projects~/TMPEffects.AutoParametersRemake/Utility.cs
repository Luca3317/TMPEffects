using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using TMPEffects.StringLibrary;

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

            if (!Strings.TypeToDisplayString.TryGetValue(fieldSymbol.Type.ToDisplayString(), out string typeString))
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
                        tc.Kind == TypedConstantKind.Array ? tc.Values.ToArray() : new TypedConstant[] { tc }).Select(
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
            if (!Strings.TypeToDisplayString.TryGetValue(fieldSymbol.ElementType.ToDisplayString(),
                    out string typeString))
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
                        tc.Kind == TypedConstantKind.Array ? tc.Values.ToArray() : new TypedConstant[] { tc }).Select(
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
                    SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression));
                var arguments =
                    new List<ArgumentSyntax>() { SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parametersName)) }
                        .Concat(attrData.ConstructorArguments.Skip(1).SelectMany(tc =>
                                tc.Kind == TypedConstantKind.Array ? tc.Values.ToArray() : new TypedConstant[] { tc })
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
            if (!Strings.TypeToDisplayString.TryGetValue(fieldSymbol.Type.ToDisplayString(), out string typeString))
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
            if (!Strings.TypeToDisplayString.TryGetValue(arrayTypeSymbol.ElementType.ToDisplayString(),
                    out string typeString))
            {
                if (arrayTypeSymbol.ElementType.SpecialType == SpecialType.System_String)
                    return GetSetStringArraySyntax(parametersName, storageName, fieldSymbol, arrayTypeSymbol, attrData);

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

            var outArg = SyntaxFactory.Argument(SyntaxFactory.DeclarationExpression(SyntaxFactory.IdentifierName("var"),
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
                        SyntaxFactory.IdentifierName(arrayTypeSymbol.ElementType.ToDisplayString())))
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

            var outArg = SyntaxFactory.Argument(SyntaxFactory.DeclarationExpression(SyntaxFactory.IdentifierName("var"),
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

            var outArg = SyntaxFactory.Argument(SyntaxFactory.DeclarationExpression(SyntaxFactory.IdentifierName("var"),
                SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier(fieldSymbol.Name))));

            var arguments =
                new List<ArgumentSyntax>()
                {
                    outArg.WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)),
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parametersName))
                }.Concat(attrData.ConstructorArguments.Skip(firstAliasIndex).SelectMany(tc =>
                    tc.Kind == TypedConstantKind.Array ? tc.Values.ToArray() : new TypedConstant[] { tc }).Select(val =>
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
            if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
                typeSymbol = arrayTypeSymbol.ElementType;

            if (Strings.ValidAutoParameterTypes.Contains(typeSymbol.ToDisplayString()))
                return true;

            if (InheritsFrom(typeSymbol, Strings.UnityObjectName))
                return true;

            return false;
        }

        public static bool IsAutoParameter(SyntaxNodeAnalysisContext context, FieldDeclarationSyntax field)
        {
            foreach (var variable in field.Declaration.Variables)
            {
                var fieldSymbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, variable) as IFieldSymbol;
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
            return Strings.ValidAutoParameterBundleTypes.Contains(typeSymbol.ToDisplayString());
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