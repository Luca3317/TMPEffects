using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Metadata;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.AutoParameters.Generator;

namespace TMPEffects.AutoParameters.Analyzers
{
    public static class Constants
    {
        public const string DefaultStorageName = "AutoParametersData";


        #region AutoParametersAttribute

        public const string AutoParametersAttributeName =
            "TMPEffects.AutoParameters.Attributes.AutoParametersAttribute";

        public const string AutoParametersStorageAttributeName =
            "TMPEffects.AutoParameters.Attributes.AutoParametersStorageAttribute";

        public const string AutoParameterAttributeName =
            "TMPEffects.AutoParameters.Attributes.AutoParameterAttribute";

        public const string AutoParameterBundleAttributeName =
            "TMPEffects.AutoParameters.Attributes.AutoParameterBundleAttribute";

        #endregion


        #region Various TMPEffects namespace paths and names

        public const string AnimationUtilityPath = "TMPEffects.TMPAnimations.AnimationUtility";
        public const string ParameterUtilityPath = "TMPEffects.Parameters.ParameterUtility";
        public const string ParameterTypesPath = "TMPEffects.Parameters.ParameterTypes";
        public const string ParameterParsingPath = "TMPEffects.Parameters.ParameterParsing";

        public const string CharDataName = "TMPEffects.CharacterData.CharData";
        public const string IAnimationContextName = "TMPEffects.TMPAnimations.IAnimationContext";

        #endregion

        #region Supported Types

        #region Unity

        public const string SerializeFieldAttributeName = "UnityEngine.SerializeField";
        public const string Vector4Name = "UnityEngine.Vector4";
        public const string Vector3Name = "UnityEngine.Vector3";
        public const string Vector2Name = "UnityEngine.Vector2";
        public const string AnimationCurveName = "UnityEngine.AnimationCurve";
        public const string ColorName = "UnityEngine.Color";

        #endregion

        #region Custom

        public const string WaveOffsetTypeName = ParameterTypesPath + ".WaveOffsetType";
        public const string WaveName = AnimationUtilityPath + ".Wave";
        public const string TypedVector3Name = ParameterTypesPath + ".TypedVector3";
        public const string TypedVector2Name = ParameterTypesPath + ".TypedVector2";

        #endregion

        #endregion

        #region Validate, Set and StringTo methods

        public const string ParameterDefinedName = ParameterUtilityPath + ".ParameterDefined";
        public const string TryGetDefinedParameter = ParameterUtilityPath + ".TryGetDefinedParameter";
        public const string GetDefinedParameter = ParameterUtilityPath + ".GetDefinedParameter";
        public const string HasNonTypeParameterName = ParameterUtilityPath + ".HasNon{0}Parameter";
        public const string HasTypeParameterName = ParameterUtilityPath + ".Has{0}Parameter";
        public const string TryGetTypeParameterName = ParameterUtilityPath + ".TryGet{0}Parameter";
        public const string StringToTypeName = ParameterParsingPath + ".StringTo{0}";
        public const string ValidateWaveParameters = ParameterUtilityPath + ".ValidateWaveParameters";

        #endregion

        public static Dictionary<string, string> SupportedTypes =>
            SupportedTypesList.GroupBy(t => t.Item1)
                .Select(t => t.First())
                .ToDictionary(t => t.Item1, t => t.Item2);

        public static readonly List<(string, string)> SupportedTypesList = new List<(string, string)>()
        {
            ("float", "Float"),
            ("int", "Int"),
            ("bool", "Bool"),

            (WaveOffsetTypeName, "WaveOffset"),
            (TypedVector3Name, "TypedVector3"),
            (TypedVector2Name, "TypedVector2"),
            (Vector2Name, "Vector2"),
            (Vector3Name, "Vector3"),
            (Vector2Name, "Vector2Offset"),
            (Vector3Name, "Vector3Offset"),
            (Vector2Name, "Anchor"),
            (ColorName, "Color"),
            (AnimationCurveName, "AnimCurve"),
        };

        public static readonly List<string> ValidAutoParameterTypes = new List<string>()
        {
            "float",
            "int",
            "bool",
            "string",
            WaveOffsetTypeName,
            TypedVector3Name,
            TypedVector2Name,
            Vector3Name,
            Vector2Name,
            ColorName,
            AnimationCurveName,
        };

        public static readonly List<string> ValidAutoParameterBundleTypes = new List<string>()
        {
            WaveName
        };

        public static StatementSyntax GetValidationSyntax(string parametersName, IFieldSymbol fieldSymbol,
            AttributeData attrData)
        {
            if (attrData.ConstructorArguments.Length == 0) /*throw new System.ArgumentException();*/ return null;

            if (!SupportedTypes.TryGetValue(fieldSymbol.Type.ToDisplayString(), out string typeString))
            {
                if (fieldSymbol.Type is IArrayTypeSymbol)
                    return GetArrayValidationSyntax(parametersName, fieldSymbol.Type as IArrayTypeSymbol, attrData);
                if (fieldSymbol.Type.SpecialType == SpecialType.System_String)
                    return GetStringValidationSyntax(parametersName, fieldSymbol, attrData);
                if (fieldSymbol.Type.ToDisplayString() == WaveName)
                    return GetWaveValidationSyntax(parametersName, fieldSymbol, attrData);
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
                    SyntaxFactory.IdentifierName(string.Format(HasTypeParameterName, typeString)),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

                negationS = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, invocation);
                ifStatementSyntaxxx = SyntaxFactory.IfStatement(negationS, returnStatement);
            }
            else
            {
                invocation = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName(string.Format(HasNonTypeParameterName, typeString)),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

                ifStatementSyntaxxx = SyntaxFactory.IfStatement(invocation, returnStatement);
            }

            return ifStatementSyntaxxx;
        }

        public static StatementSyntax GetSetParameterSyntax(string parametersName, string storageName,
            IFieldSymbol fieldSymbol, AttributeData attrData)
        {
            if (attrData.ConstructorArguments.Length == 0) return null;
            if (!SupportedTypes.TryGetValue(fieldSymbol.Type.ToDisplayString(), out string typeString))
            {
                if (fieldSymbol.Type is IArrayTypeSymbol)
                    return GetSetArraySyntax(parametersName, storageName, fieldSymbol,
                        fieldSymbol.Type as IArrayTypeSymbol, attrData);
                if (fieldSymbol.Type.SpecialType == SpecialType.System_String)
                    return GetSetStringSyntax(parametersName, storageName, fieldSymbol, attrData);
                if (fieldSymbol.Type.ToDisplayString() == WaveName)
                    return GetSetWaveSyntax(parametersName, storageName, fieldSymbol, attrData);
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
                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parametersName))
            }.Concat(attrData.ConstructorArguments.Skip(firstAliasIndex).SelectMany(tc =>
                tc.Kind == TypedConstantKind.Array ? tc.Values.ToArray() : new TypedConstant[] { tc }).Select(val =>
                SyntaxFactory.Argument(
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal((string)val.Value)))));

            invocation = SyntaxFactory.InvocationExpression(
                SyntaxFactory.IdentifierName(string.Format(TryGetTypeParameterName, typeString)),
                SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

            assignment = SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(storageName), SyntaxFactory.IdentifierName(fieldSymbol.Name)),
                SyntaxFactory.IdentifierName(fieldSymbol.Name)));

            ifStatementSyntaxxx = SyntaxFactory.IfStatement(invocation, assignment);

            return ifStatementSyntaxxx;
        }


        // Array special case handling
        private static StatementSyntax GetArrayValidationSyntax(string parametersName, IArrayTypeSymbol fieldSymbol,
            AttributeData attrData)
        {
            if (!SupportedTypes.TryGetValue(fieldSymbol.ElementType.ToDisplayString(), out string typeString))
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
                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(string.Format(StringToTypeName, typeString)))
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
                    SyntaxFactory.Identifier(string.Format(HasTypeParameterName, "Array")),
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
                    SyntaxFactory.Identifier(string.Format(HasNonTypeParameterName, "Array")),
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
                    SyntaxFactory.IdentifierName(string.Format(HasTypeParameterName, "Array")),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

                negationS = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, invocation);
                ifStatementSyntaxxx = SyntaxFactory.IfStatement(negationS, returnStatement);
            }
            else
            {
                invocation = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName(string.Format(HasNonTypeParameterName, "Array")),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

                ifStatementSyntaxxx = SyntaxFactory.IfStatement(invocation, returnStatement);
            }

            return ifStatementSyntaxxx;
        }

        private static StatementSyntax GetSetArraySyntax(string parametersName, string storageName,
            IFieldSymbol fieldSymbol, IArrayTypeSymbol arrayTypeSymbol, AttributeData attrData)
        {
            if (!SupportedTypes.TryGetValue(arrayTypeSymbol.ElementType.ToDisplayString(), out string typeString))
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
                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(string.Format(StringToTypeName, typeString)))
            }.Concat(attrData.ConstructorArguments.Skip(firstAliasIndex).SelectMany(tc =>
                tc.Kind == TypedConstantKind.Array ? tc.Values.ToArray() : new TypedConstant[] { tc }).Select(val =>
                SyntaxFactory.Argument(
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal((string)val.Value)))));

            var genericName = SyntaxFactory.GenericName(
                SyntaxFactory.Identifier(string.Format(TryGetTypeParameterName, "Array")),
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
                SyntaxFactory.IdentifierName(string.Format(TryGetTypeParameterName, "Array")),
                SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

            assignment = SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(storageName), SyntaxFactory.IdentifierName(fieldSymbol.Name)),
                SyntaxFactory.IdentifierName(fieldSymbol.Name)));

            ifStatementSyntaxxx = SyntaxFactory.IfStatement(invocation, assignment);
            return ifStatementSyntaxxx;
        }


        // string special case handling
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
                    SyntaxFactory.IdentifierName(ParameterDefinedName),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

                var ifStatementSyntaxxx = SyntaxFactory.IfStatement(invocation, returnStatement);
                return ifStatementSyntaxxx;
            }
            else
            {
                return SyntaxFactory.EmptyStatement();
            }
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
                SyntaxFactory.IdentifierName(TryGetDefinedParameter),
                SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

            var assignment = SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(storageName), SyntaxFactory.IdentifierName(fieldSymbol.Name)),
                SyntaxFactory.IdentifierName(fieldSymbol.Name)));

            var ifStatementSyntaxxx = SyntaxFactory.IfStatement(invocation, assignment);

            return ifStatementSyntaxxx;
        }


        // Wave special case handling
        private static StatementSyntax GetWaveValidationSyntax(string parametersName, IFieldSymbol fieldSymbol,
            AttributeData attrData)
        {
            return SyntaxFactory.ParseStatement(
                $"if (!{ValidateWaveParameters}({parametersName}, \"{(string)attrData.ConstructorArguments[0].Value}\")) return false;");
        }

        private static StatementSyntax GetSetWaveSyntax(string parametersName, string storageName,
            IFieldSymbol fieldSymbol, AttributeData attrData)
        {
            return SyntaxFactory.ParseStatement($"{storageName}.{fieldSymbol.Name} = " +
                                                $"{ParameterUtilityPath}.CreateWave({storageName}.{fieldSymbol.Name}, {ParameterUtilityPath}.GetWaveParameters({parametersName}, \"{(string)attrData.ConstructorArguments[0].Value}\"));");
        }


        public static bool SpecifiesRequirement(AttributeData attrData)
        {
            if (attrData == null) throw new System.ArgumentNullException(nameof(attrData));
            if (attrData.AttributeClass.ToDisplayString() != AutoParameterAttributeName) return false;
            if (attrData.ConstructorArguments.Count() < 1) throw new System.ArgumentException(nameof(attrData));
            return attrData.ConstructorArguments[0].Type.SpecialType == SpecialType.System_Boolean;
        }

        public static bool IsRequiredAutoParameter(AttributeData attrData)
        {
            if (attrData == null) throw new System.ArgumentNullException(nameof(attrData));
            if (attrData.AttributeClass.ToDisplayString() != AutoParameterAttributeName) return false;
            if (attrData.ConstructorArguments.Count() < 1) throw new System.ArgumentException(nameof(attrData));
            if (attrData.ConstructorArguments[0].Type.SpecialType != SpecialType.System_Boolean) return false;
            if (attrData.ConstructorArguments.Count() < 2) throw new System.ArgumentException(nameof(attrData));
            return (bool)attrData.ConstructorArguments[0].Value;
        }


        public static AttributeData IsValidAutoParameter(IFieldSymbol fieldSymbol)
        {
            if (IsValidAutoParameterType(fieldSymbol.Type))
            {
                return IsAutoParameter(fieldSymbol);
            }

            return null;
        }

        public static bool IsValidAutoParameterType(ITypeSymbol typeSymbol)
        {
            if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            {
                return ValidAutoParameterTypes.Contains(arrayTypeSymbol.ElementType.ToDisplayString());
            }
            else
            {
                return ValidAutoParameterTypes.Contains(typeSymbol.ToDisplayString());
            }
        }

        public static AttributeData IsAutoParameter(IFieldSymbol fieldSymbol)
        {
            var attributes = fieldSymbol.GetAttributes();
            foreach (var attribute in attributes)
            {
                var baseClass = attribute?.AttributeClass;
                while (baseClass != null)
                {
                    if (baseClass.ToDisplayString() == Constants.AutoParameterAttributeName)
                    {
                        return attribute;
                    }

                    baseClass = baseClass.BaseType;
                }
            }

            return null;
        }


        public static AttributeData IsValidAutoParameterBundle(IFieldSymbol fieldSymbol)
        {
            if (IsValidAutoParameterBundleType(fieldSymbol.Type))
            {
                return IsAutoParameterBundle(fieldSymbol);
            }

            return null;
        }

        public static bool IsValidAutoParameterBundleType(ITypeSymbol typeSymbol)
        {
            return ValidAutoParameterBundleTypes.Contains(typeSymbol.ToDisplayString());
        }

        public static AttributeData IsAutoParameterBundle(IFieldSymbol fieldSymbol)
        {
            var attributes = fieldSymbol.GetAttributes();
            foreach (var attribute in attributes)
            {
                var baseClass = attribute?.AttributeClass;
                while (baseClass != null)
                {
                    if (baseClass.ToDisplayString() == Constants.AutoParameterBundleAttributeName)
                    {
                        return attribute;
                    }

                    baseClass = baseClass.BaseType;
                }
            }

            return null;
        }
    }
}