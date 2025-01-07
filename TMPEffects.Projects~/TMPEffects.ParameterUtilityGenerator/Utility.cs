using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TMPEffects.ParameterUtilityGenerator
{
    public static class Utility
    {
        public static bool TryGetParameterTypeDisplayName(AttributeSyntax syntax, out string displayName)
        {
            displayName = null;
            if (syntax.ArgumentList == null) return false;
            if (syntax.ArgumentList.Arguments.Count < 1) return false;
            if (!(syntax.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax le)) return false;
            if (!le.IsKind(SyntaxKind.StringLiteralExpression)) return false;
            displayName = le.Token.ValueText;
            return true;
        }

        public static ParameterTypeData GetParameterTypeData(AttributeSyntax attrSyntax,
            SemanticModel model)
        {
            // Get the attribute arguments
            var arguments = attrSyntax.ArgumentList.Arguments;

            AttributeArgumentSyntax displayNameArg;
            AttributeArgumentSyntax typeArg;
            AttributeArgumentSyntax sceneTypeArg;
            AttributeArgumentSyntax diskTypeArg;
            AttributeArgumentSyntax sharedBaseTypeArg;
            AttributeArgumentSyntax generateKeywordDatabaseArg = null;


            ParameterTypeData parameterTypeData = new ParameterTypeData();
            if (arguments.Count <= 2)
            {
                displayNameArg = arguments[0];

                if (arguments.Count > 1)
                    generateKeywordDatabaseArg = arguments[1];

                var constval = model.GetConstantValue(displayNameArg.Expression);
                if (constval.HasValue && constval.Value is string str)
                {
                    parameterTypeData.DisplayName = str;
                }
                else
                {
                    throw new System.InvalidOperationException();
                }

                if (generateKeywordDatabaseArg != null)
                {
                    constval = model.GetConstantValue(generateKeywordDatabaseArg.Expression);
                    if (constval.HasValue && constval.Value is bool bl)
                    {
                        parameterTypeData.GenerateKeywordDatabase = bl;
                    }
                    else
                    {
                        throw new System.InvalidOperationException();
                    }
                }

                parameterTypeData.SharedType =
                    ModelExtensions.GetDeclaredSymbol(model, attrSyntax.Ancestors().OfType<TypeDeclarationSyntax>().First()) as
                        ITypeSymbol;
                parameterTypeData.SharedTypeName = parameterTypeData.SharedType.ToDisplayString();

                parameterTypeData.SceneTypeName = parameterTypeData.SharedTypeName;
                parameterTypeData.SceneType = parameterTypeData.SharedType;
                parameterTypeData.DiskTypeName = parameterTypeData.SharedTypeName;
                parameterTypeData.DiskType = parameterTypeData.SharedType;
            }
            else
            {
                displayNameArg = arguments[0];
                diskTypeArg = arguments[1];
                sceneTypeArg = arguments[2];

                if (arguments.Count > 3)
                    generateKeywordDatabaseArg = arguments[3];

                var constval = model.GetConstantValue(displayNameArg.Expression);
                if (constval.HasValue && constval.Value is string str)
                {
                    parameterTypeData.DisplayName = str;
                }
                else
                {
                    throw new System.InvalidOperationException();
                }

                if (sceneTypeArg.Expression is TypeOfExpressionSyntax typeOfExpressionSyntax2)
                {
                    var ti = ModelExtensions.GetTypeInfo(model, typeOfExpressionSyntax2.Type);
                    if (ti.Type != null)
                    {
                        parameterTypeData.SceneType = ti.Type;
                        parameterTypeData.SceneTypeName = ti.Type.ToDisplayString();
                    }
                    else
                    {
                        throw new System.InvalidOperationException();
                    }
                }

                if (diskTypeArg.Expression is TypeOfExpressionSyntax typeOfExpressionSyntax3)
                {
                    var ti = ModelExtensions.GetTypeInfo(model, typeOfExpressionSyntax3.Type);
                    if (ti.Type != null)
                    {
                        parameterTypeData.DiskType = ti.Type;
                        parameterTypeData.DiskTypeName = ti.Type.ToDisplayString();
                    }
                    else
                    {
                        throw new System.InvalidOperationException();
                    }
                }

                if (generateKeywordDatabaseArg != null)
                {
                    constval = model.GetConstantValue(generateKeywordDatabaseArg.Expression);
                    if (constval.HasValue && constval.Value is bool bl)
                    {
                        parameterTypeData.GenerateKeywordDatabase = bl;
                    }
                    else
                    {
                        throw new System.InvalidOperationException();
                    }
                }

                parameterTypeData.SharedType =
                    ModelExtensions.GetDeclaredSymbol(model, attrSyntax.Ancestors().OfType<TypeDeclarationSyntax>().First()) as
                        ITypeSymbol;
                parameterTypeData.SharedTypeName = parameterTypeData.SharedType.ToDisplayString();
            }

            // context.ReportDiagnostic(Diagnostic.Create(Rule___, attrSyntax.GetLocation(),
            //     "Made it to the end! DisplayName: " + parameterTypeData.DisplayName +
            //     " SceneTypeName: " + parameterTypeData.SceneTypeName +
            //     " DiskTypeName: " + parameterTypeData.DiskTypeName +
            //     " SharedTypeName: " + parameterTypeData.SharedTypeName +
            //     " GenKeyword: " + parameterTypeData.GenerateKeywordDatabase));

            return parameterTypeData;
        }

        public struct ParameterTypeData
        {
            public string DisplayName;
            public string SceneTypeName;
            public string DiskTypeName;
            public string SharedTypeName;

            public ITypeSymbol SharedType;
            public ITypeSymbol DiskType;
            public ITypeSymbol SceneType;

            public bool GenerateKeywordDatabase;
        }
    }
}