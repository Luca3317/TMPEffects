using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using TMPEffects.AutoParameters.Analyzers;
using TMPEffects.AutoParameters.Attributes;
using static System.Reflection.Metadata.Ecma335.MethodBodyStreamEncoder;

namespace TMPEffects.AutoParameters.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AutoParametersAnalyzer : DiagnosticAnalyzer
    {
        #region AutoParametersAttribute rules

        public const string DiagnosticId_0 = "TMPAP000";

        private static readonly LocalizableString Title_0 =
            "Types decorated with " + Constants.AutoParametersAttributeName + " must be partial";

        private static readonly LocalizableString MessageFormat_0 =
            "Type {0} decorated with " + Constants.AutoParametersAttributeName + " is not partial";

        private const string Category_0 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_0 = new DiagnosticDescriptor(DiagnosticId_0, Title_0,
            MessageFormat_0, Category_0, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_1 = "TMPAP001";

        private static readonly LocalizableString Title_1 = "Types decorated with " +
                                                            Constants.AutoParametersAttributeName +
                                                            " must not implement SetParameters(object, IDictionary<string, string>)";

        private static readonly LocalizableString MessageFormat_1 = "Type {0} decorated with " +
                                                                    Constants.AutoParametersAttributeName +
                                                                    " implements SetParameters(object, IDictionary<string, string>)";

        private const string Category_1 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1 = new DiagnosticDescriptor(DiagnosticId_1, Title_1,
            MessageFormat_1, Category_1, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_2 = "TMPAP002";

        private static readonly LocalizableString Title_2 = "Types decorated with " +
                                                            Constants.AutoParametersAttributeName +
                                                            " must not implement ValidateParameters(IDictionary<string, string>)";

        private static readonly LocalizableString MessageFormat_2 = "Type {0} decorated with " +
                                                                    Constants.AutoParametersAttributeName +
                                                                    " implements ValidateParameters(IDictionary<string, string>)";

        private const string Category_2 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_2 = new DiagnosticDescriptor(DiagnosticId_2, Title_2,
            MessageFormat_2, Category_2, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_3 = "TMPAP003";

        private static readonly LocalizableString Title_3 = "Types decorated with " +
                                                            Constants.AutoParametersAttributeName +
                                                            " must not implement GetNewCustomData()";

        private static readonly LocalizableString MessageFormat_3 = "Type {0} decorated with " +
                                                                    Constants.AutoParametersAttributeName +
                                                                    " implements GetNewCustomData()";

        private const string Category_3 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_3 = new DiagnosticDescriptor(DiagnosticId_3, Title_3,
            MessageFormat_3, Category_3, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_4 = "TMPAP004";
        private static readonly LocalizableString Title_4 = "Direct reference to AutoParameter field in Animate method";

        private static readonly LocalizableString MessageFormat_4 =
            "Direct reference to AutoParameter field {0} in Animate method; did you mean to use the {1} field of your AutoParameterStorage?";

        private const string Category_4 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_4 = new DiagnosticDescriptor(DiagnosticId_4, Title_4,
            MessageFormat_4, Category_4, DiagnosticSeverity.Warning, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_5 = "TMPAP005";

        private static readonly LocalizableString Title_5 = "Types decorated with " +
                                                            Constants.AutoParameterAttributeName +
                                                            " must implement private partial void Animate()";

        private static readonly LocalizableString MessageFormat_5 =
            "Type {0} decorated with " + Constants.AutoParameterAttributeName +
            " does not implement private partial void Animate()";

        private const string Category_5 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_5 = new DiagnosticDescriptor(DiagnosticId_5, Title_5,
            MessageFormat_5, Category_5, DiagnosticSeverity.Warning, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        #endregion

        #region AutoParameterAttribute rules

        //        public const string DiagnosticId_1_0 = "TMPAP100";
        //        private static readonly LocalizableString Title_1_0 = "Types decorated with " + Constants.AutoParameterAttributeName + " should have unique name and aliases within the containing class";
        //        private static readonly LocalizableString MessageFormat_1_0 = "Type {0} decorated with " + Constants.AutoParameterAttributeName + " does not have unique name and aliases within the containing class";
        //        private const string Category_1_0 = "Usage";
        //#pragma warning disable RS2008 // Enable analyzer release tracking
        //        private static readonly DiagnosticDescriptor Rule_1_0 = new DiagnosticDescriptor(DiagnosticId_1_0, Title_1_0, MessageFormat_1_0, Category_1_0, DiagnosticSeverity.Warning, isEnabledByDefault: true);
        //#pragma warning restore RS2008 // Enable analyzer release tracking


        public const string DiagnosticId_1_1 = "TMPAP101";

        private static readonly LocalizableString Title_1_1 =
            "Invalid type of field decorated with " + Constants.AutoParameterAttributeName;

        private static readonly LocalizableString MessageFormat_1_1 =
            "Field {0} decorated with " + Constants.AutoParameterAttributeName + " has invalid type {1}";

        private const string Category_1_1 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1_1 = new DiagnosticDescriptor(DiagnosticId_1_1, Title_1_1,
            MessageFormat_1_1, Category_1_1, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_1_2 = "TMPAP102";

        private static readonly LocalizableString Title_1_2 =
            "Invalid type of field decorated with " + Constants.AutoParameterAttributeName;

        private static readonly LocalizableString MessageFormat_1_2 = "Field {0} decorated with " +
                                                                      Constants.AutoParameterAttributeName +
                                                                      " has invalid type {1}; You may decorate the field with " +
                                                                      Constants.AutoParameterBundleAttributeName +
                                                                      " instead";

        private const string Category_1_2 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1_2 = new DiagnosticDescriptor(DiagnosticId_1_2, Title_1_2,
            MessageFormat_1_2, Category_1_2, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_1_3 = "TMPAP103";

        private static readonly LocalizableString Title_1_3 =
            "Invalid type of field decorated with " + Constants.AutoParameterBundleAttributeName;

        private static readonly LocalizableString MessageFormat_1_3 = "Field {0} decorated with " +
                                                                      Constants.AutoParameterBundleAttributeName +
                                                                      " has invalid type {1}";

        private const string Category_1_3 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1_3 = new DiagnosticDescriptor(DiagnosticId_1_3, Title_1_3,
            MessageFormat_1_3, Category_1_3, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking        

        public const string DiagnosticId_1_4 = "TMPAP104";

        private static readonly LocalizableString Title_1_4 =
            "Invalid type of field decorated with " + Constants.AutoParameterBundleAttributeName;

        private static readonly LocalizableString MessageFormat_1_4 = "Field {0} decorated with " +
                                                                      Constants.AutoParameterBundleAttributeName +
                                                                      " has invalid type {1}; You may decorate the field with " +
                                                                      Constants.AutoParameterAttributeName + " instead";

        private const string Category_1_4 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1_4 = new DiagnosticDescriptor(DiagnosticId_1_4, Title_1_4,
            MessageFormat_1_4, Category_1_4, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking  

        public const string DiagnosticId_1_5 = "TMPAP105";

        private static readonly LocalizableString Title_1_5 = "Fields decorated with " +
                                                              Constants.AutoParameterAttributeName +
                                                              " must be contained within a type decorated with " +
                                                              Constants.AutoParametersAttributeName;

        private static readonly LocalizableString MessageFormat_1_5 = "Field {0} decorated with " +
                                                                      Constants.AutoParameterAttributeName +
                                                                      " is not contained in a type decorated with " +
                                                                      Constants.AutoParametersAttributeName;

        private const string Category_1_5 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1_5 = new DiagnosticDescriptor(DiagnosticId_1_5, Title_1_5,
            MessageFormat_1_5, Category_1_5, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking       

        public const string DiagnosticId_1_7 = "TMPAP107";

        private static readonly LocalizableString Title_1_7 =
            "Fields decorated with " + Constants.AutoParameterAttributeName + " should be serializable";

        private static readonly LocalizableString MessageFormat_1_7 = "Field {0} decorated with " +
                                                                      Constants.AutoParameterAttributeName +
                                                                      " is not serializable; consider making it public or decorating it with " +
                                                                      Constants.SerializeFieldAttributeName;

        private const string Category_1_7 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_1_7 = new DiagnosticDescriptor(DiagnosticId_1_7, Title_1_7,
            MessageFormat_1_7, Category_1_7, DiagnosticSeverity.Warning, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        #endregion

        #region AutoParametersStorageAttribute rules

        public const string DiagnosticId_2_0 = "TMPAP200";

        private static readonly LocalizableString Title_2_0 =
            "Types decorated with " + Constants.AutoParametersStorageAttributeName + " must be partial";

        private static readonly LocalizableString MessageFormat_2_0 = "Type {0} decorated with " +
                                                                      Constants.AutoParametersStorageAttributeName +
                                                                      " is not partial";

        private const string Category_2_0 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_2_0 = new DiagnosticDescriptor(DiagnosticId_2_0, Title_2_0,
            MessageFormat_2_0, Category_2_0, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_2_1 = "TMPAP201";

        private static readonly LocalizableString Title_2_1 = "Types decorated with " +
                                                              Constants.AutoParametersStorageAttributeName +
                                                              " must not have any fields with identical name to a field decorated with " +
                                                              Constants.AutoParameterAttributeName;

        private static readonly LocalizableString MessageFormat_2_1 = "Type {0} decorated with " +
                                                                      Constants.AutoParametersStorageAttributeName +
                                                                      " has field with identical name to a field decorated with " +
                                                                      Constants.AutoParameterAttributeName + ": {1}";

        private const string Category_2_1 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_2_1 = new DiagnosticDescriptor(DiagnosticId_2_1, Title_2_1,
            MessageFormat_2_1, Category_2_1, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_2_2 = "TMPAP202";

        private static readonly LocalizableString Title_2_2 = "Types decorated with " +
                                                              Constants.AutoParametersStorageAttributeName +
                                                              " must be contained within a type decorated with " +
                                                              Constants.AutoParametersAttributeName;

        private static readonly LocalizableString MessageFormat_2_2 = "Type {0} decorated with " +
                                                                      Constants.AutoParametersStorageAttributeName +
                                                                      " is not contained within a type decorated with " +
                                                                      Constants.AutoParametersAttributeName;

        private const string Category_2_2 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_2_2 = new DiagnosticDescriptor(DiagnosticId_2_2, Title_2_2,
            MessageFormat_2_2, Category_2_2, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_2_3 = "TMPAP203";

        private static readonly LocalizableString Title_2_3 = "There may only be one nested type decorated with " +
                                                              Constants.AutoParametersStorageAttributeName +
                                                              " contained in any given type";

        private static readonly LocalizableString MessageFormat_2_3 =
            "Type {0} contains multiple types decorated with " + Constants.AutoParametersStorageAttributeName;

        private const string Category_2_3 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_2_3 = new DiagnosticDescriptor(DiagnosticId_2_3, Title_2_3,
            MessageFormat_2_3, Category_2_3, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string DiagnosticId_2_4 = "TMPAP204";

        private static readonly LocalizableString Title_2_4 = "Types decorated with " +
                                                              Constants.AutoParametersStorageAttributeName +
                                                              " must have a default constructor (zero parameters)";

        private static readonly LocalizableString MessageFormat_2_4 = "Type {0} decorated with " +
                                                                      Constants.AutoParametersStorageAttributeName +
                                                                      " does not have a default constructor (zero parameters)";

        private const string Category_2_4 = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule_2_4 = new DiagnosticDescriptor(DiagnosticId_2_4, Title_2_4,
            MessageFormat_2_4, Category_2_4, DiagnosticSeverity.Error, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        #endregion

        public const string DiagnosticId___ = "DebuggingError";
        private static readonly LocalizableString Title___ = "Debugerror";
        private static readonly LocalizableString MessageFormat___ = "{0}";
        private const string Category___ = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule___ = new DiagnosticDescriptor(DiagnosticId___, Title___,
            MessageFormat___, Category___, DiagnosticSeverity.Warning, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create
                (
                    Rule_0,
                    Rule_1,
                    Rule_2,
                    Rule_3,
                    Rule_4 /*, Rule_1_0*/,
                    Rule_5,
                    Rule_1_1,
                    Rule_1_2,
                    Rule_1_3,
                    Rule_1_4,
                    Rule_1_5,
                    Rule_1_7,
                    Rule_2_0,
                    Rule_2_1,
                    Rule_2_2,
                    Rule_2_3,
                    Rule_2_4,
                    Rule___
                );
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(TryAnalyzeSyntaxAutoParameters, SyntaxKind.ClassDeclaration,
                SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(TryAnalyzeSyntaxAutoParametersStorage, SyntaxKind.ClassDeclaration,
                SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(TryAnalyzeSyntaxAutoParameter, SyntaxKind.FieldDeclaration);
        }

        private void TryAnalyzeSyntaxAutoParameters(SyntaxNodeAnalysisContext context)
        {
            TypeDeclarationSyntax typeDecl = (TypeDeclarationSyntax)context.Node;

            try
            {
                var symbol = context.SemanticModel.GetDeclaredSymbol(typeDecl);

                if (symbol.GetAttributes().Any(a =>
                        a.AttributeClass?.ToDisplayString() == Constants.AutoParametersAttributeName))
                {
                    AnalyzeSyntaxAutoParameters(context, typeDecl, symbol);
                }
            }
            catch
            {
                Diagnostic d = Diagnostic.Create(Rule___, typeDecl.GetLocation(),
                    "Failed TryAnalyzeSyntaxAutoParameters;");
                context.ReportDiagnostic(d);
            }
        }

        private void AnalyzeSyntaxAutoParameters(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDecl,
            INamedTypeSymbol symbol)
        {
            Diagnostic diagnostic;

            // If type is not partial, report
            if (!typeDecl.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword)))
            {
                diagnostic = Diagnostic.Create(Rule_0, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }

            var methods = typeDecl.Members.Where(member => member.Kind() == SyntaxKind.MethodDeclaration)
                .Select(method => method as MethodDeclarationSyntax);
            var objectType = context.Compilation.GetSpecialType(SpecialType.System_Object);
            var stringType = context.Compilation.GetSpecialType(SpecialType.System_String);

            // If one of the methods to generate is implemented, report
            CheckSyntaxSetParameters(context, typeDecl, methods, objectType, stringType);
            CheckSyntaxValidateParameters(context, typeDecl, methods, stringType);
            CheckSyntaxGetNewCustomData(context, typeDecl, methods);

            // If any autoparameters / bundles are referenced in Animate(), report
            CheckAutoParameterReferenced(context, typeDecl,
                GetStorageName(typeDecl.Identifier.Text, typeDecl.Members.OfType<TypeDeclarationSyntax>(), context));

            // If multiple storages are defined, report
            var nestedtypes = typeDecl.Members.OfType<BaseTypeDeclarationSyntax>();
            int count = 0;
            string firstFoundName = "";
            foreach (var type in nestedtypes)
            {
                INamedTypeSymbol typeSymbol = context.SemanticModel.GetDeclaredSymbol(type);
                foreach (var att in typeSymbol.GetAttributes())
                {
                    if (att.AttributeClass.ToDisplayString() == Constants.AutoParametersStorageAttributeName)
                    {
                        count++;
                        if (count > 1)
                        {
                            diagnostic = Diagnostic.Create(Rule_2_3, typeDecl.Identifier.GetLocation(),
                                typeDecl.Identifier.Text);
                            context.ReportDiagnostic(diagnostic);
                            break;
                        }
                        else
                        {
                            firstFoundName = typeSymbol.ToDisplayString();
                        }
                    }
                }
            }

            if (count == 0) firstFoundName = typeDecl.Identifier.Text + "." + Constants.DefaultStorageName;

            // If no partial Animate is implemented, report
            CheckPartialAnimate(context, typeDecl, methods, firstFoundName);
        }

        public static string GetStorageName(string enclosingTypeName,
            IEnumerable<BaseTypeDeclarationSyntax> nestedTypes,
            SyntaxNodeAnalysisContext context)
        {
            foreach (var type in nestedTypes)
            {
                INamedTypeSymbol typeSymbol = context.SemanticModel.GetDeclaredSymbol(type);
                foreach (var att in typeSymbol.GetAttributes())
                {
                    if (att.AttributeClass.ToDisplayString() == Constants.AutoParametersStorageAttributeName)
                    {
                        return typeSymbol.ToDisplayString();
                    }
                }
            }

            return enclosingTypeName + "." + Constants.DefaultStorageName;
        }

        private void CheckPartialAnimate(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDecl,
            IEnumerable<MethodDeclarationSyntax> methods, string storageDeclName)
        {
            var model = context.SemanticModel;
            var setParamCandidates = methods.Where(method => method.Identifier.Text == "Animate");

            Diagnostic diagnostic;
            foreach (var candidate in setParamCandidates)
            {
                // If doesnt have exactly two parameter, continue
                if (candidate.ParameterList.Parameters == null ||
                    candidate.ParameterList.Parameters.Count != 3) continue;

                // If has no body, continue
                if (candidate.Body == null) continue;

                // If isnt private partial, continue
                bool isPartial = candidate.Modifiers.Any(SyntaxKind.PartialKeyword);
                // bool isPrivate = candidate.Modifiers.Any(SyntaxKind.PrivateKeyword);
                if (!isPartial /*|| !isPrivate*/) continue;

                // If first parameter is not CharData, continue
                var expressionSyntax = candidate.ParameterList.Parameters[0].Type;
                if (expressionSyntax == null) continue;
                var type = context.SemanticModel.GetTypeInfo(expressionSyntax).Type as INamedTypeSymbol;
                if (type == null || type.ToDisplayString() != Constants.CharDataName) continue;

                // If second parameter is not the storage, continue
                expressionSyntax = candidate.ParameterList.Parameters[1].Type;
                if (expressionSyntax == null) continue;
                type = context.SemanticModel.GetTypeInfo(expressionSyntax).Type as INamedTypeSymbol;

                if (type == null || type.ToDisplayString() != storageDeclName) continue;

                // If third parameter is not the animationcontext, continue
                expressionSyntax = candidate.ParameterList.Parameters[2].Type;
                if (expressionSyntax == null) continue;
                type = context.SemanticModel.GetTypeInfo(expressionSyntax).Type as INamedTypeSymbol;
                if (type == null || type.ToDisplayString() != Constants.IAnimationContextName) continue;

                return;
            }

            diagnostic =
                Diagnostic.Create(Rule_5, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
            context.ReportDiagnostic(diagnostic);
        }

        private void CheckAutoParameterReferenced(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDecl,
            string storageDeclName)
        {
            var animateCandidates = typeDecl.Members.Where(member => member.Kind() == SyntaxKind.MethodDeclaration)
                .Select(member => member as MethodDeclarationSyntax)
                .Where(method => method.Identifier.Text == "Animate").ToList();

            MethodDeclarationSyntax animate = null;
            int a = 0;
            foreach (var candidate in animateCandidates)
            {
                var candidateSymbol = context.SemanticModel.GetDeclaredSymbol(candidate);
                if (candidateSymbol.Parameters == null || candidateSymbol.Parameters.Length != 3) continue;
                if (candidateSymbol.Parameters[0].Type.ToDisplayString() != Constants.CharDataName) continue;
                if (candidateSymbol.Parameters[2].Type.ToDisplayString() != Constants.IAnimationContextName) continue;
                if (candidateSymbol.Parameters[1].Type.ToDisplayString() != storageDeclName) continue;
                animate = candidate;
                break;
            }

            if (animate == null) return;

            var autoparameters = typeDecl.Members.OfType<FieldDeclarationSyntax>()
                .Where(field => IsAutoParameter(context, field));
            var autoparameterSymbols = autoparameters.SelectMany(field => field.Declaration.Variables)
                .Select(variable => context.SemanticModel.GetDeclaredSymbol(variable));
            var identifierNodes = animate.DescendantNodes().OfType<IdentifierNameSyntax>();

            foreach (var identifier in identifierNodes)
            {
                ISymbol identifierSymbol = context.SemanticModel.GetSymbolInfo(identifier).Symbol;

                if (autoparameterSymbols.Contains(identifierSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule_4, identifier.GetLocation(), identifierSymbol.Name,
                        identifierSymbol.Name));
                }
            }
        }


        private void TryAnalyzeSyntaxAutoParametersStorage(SyntaxNodeAnalysisContext context)
        {
            TypeDeclarationSyntax typeDecl = (TypeDeclarationSyntax)context.Node;

            try
            {
                var symbol = context.SemanticModel.GetDeclaredSymbol(typeDecl);

                if (symbol.GetAttributes().Any(a =>
                        a.AttributeClass?.ToDisplayString() == Constants.AutoParametersStorageAttributeName))
                {
                    AnalyzeSyntaxAutoParametersStorage(context, typeDecl, symbol);
                }
            }
            catch (Exception e)
            {
                Diagnostic d = Diagnostic.Create(Rule___, typeDecl.GetLocation(),
                    "Failed TryAnalyzeSyntaxAutoParametersStorage: " + e.Message);
                context.ReportDiagnostic(d);
            }
        }

        private void AnalyzeSyntaxAutoParametersStorage(SyntaxNodeAnalysisContext context,
            TypeDeclarationSyntax typeDecl, INamedTypeSymbol typeSymbol)
        {
            Diagnostic diagnostic;

            // If is not partial, report
            if (!typeDecl.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword)))
            {
                diagnostic = Diagnostic.Create(Rule_2_0, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }

            // If has no empty constructor, report
            if (!typeSymbol.Constructors.Any(ctr => ctr.Parameters.Length == 0))
            {
                diagnostic = Diagnostic.Create(Rule_2_4, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }

            // If is not contained in another type, report
            if (!(typeDecl.Parent is TypeDeclarationSyntax parentDecl))
            {
                diagnostic = Diagnostic.Create(Rule_2_1, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
                return;
            }

            var parentSymbol = context.SemanticModel.GetDeclaredSymbol(parentDecl);
            if (parentSymbol == null) return;

            // If is not contained in type with AutoParametersAttibute, report
            if (!parentSymbol.GetAttributes().Any(attr =>
                    attr.AttributeClass?.ToDisplayString() == Constants.AutoParametersAttributeName))
            {
                diagnostic = Diagnostic.Create(Rule_2_2, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
                return;
            }

            // If has field with same name AutoParameter, report
            var containingTypeFields = parentSymbol.GetMembers()
                .Where(member => member.Kind == SymbolKind.Field && IsAutoParameter(member));
            var fields = typeSymbol.GetMembers()
                .Where(member => member.Kind == SymbolKind.Field || member.Kind == SymbolKind.Property);
            var containingTypeFieldNames = containingTypeFields.Select(field => field.Name).ToImmutableHashSet();

            foreach (var field in fields)
            {
                if (containingTypeFieldNames.Contains(field.Name))
                {
                    diagnostic = Diagnostic.Create(Rule_2_1, field.Locations[0], typeDecl.Identifier.Text, field.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }


        private void TryAnalyzeSyntaxAutoParameter(SyntaxNodeAnalysisContext context)
        {
            FieldDeclarationSyntax fieldDecl = (FieldDeclarationSyntax)context.Node;

            foreach (var v in fieldDecl.Declaration.Variables)
            {
                IFieldSymbol varSymbol = context.SemanticModel.GetDeclaredSymbol(v) as IFieldSymbol;
                if (IsAutoParameter(varSymbol)) AnalyzeSyntaxAutoParameter(context, v, varSymbol);
            }
        }

        private void AnalyzeSyntaxAutoParameter(SyntaxNodeAnalysisContext context, VariableDeclaratorSyntax varDecl,
            IFieldSymbol varSymbol)
        {
            Diagnostic diagnostic;

            // Check validity of type
            if (IsAutoParameterBundle(varSymbol))
            {
                string typestring = varSymbol.Type.ToDisplayString();
                if (!Constants.ValidAutoParameterBundleTypes.Contains(typestring))
                {
                    if (Constants.ValidAutoParameterTypes.Contains(typestring))
                    {
                        diagnostic = Diagnostic.Create(Rule_1_4, varDecl.GetLocation(), varSymbol.Name,
                            varSymbol.Type.ToMinimalDisplayString(context.SemanticModel, 0));
                        context.ReportDiagnostic(diagnostic);
                    }
                    else
                    {
                        diagnostic = Diagnostic.Create(Rule_1_3, varDecl.GetLocation(), varSymbol.Name,
                            varSymbol.Type.ToMinimalDisplayString(context.SemanticModel, 0));
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
            else
            {
                string typestring = varSymbol.Type is IArrayTypeSymbol arr
                    ? arr.ElementType.ToDisplayString()
                    : varSymbol.Type.ToDisplayString();
                if (!Constants.ValidAutoParameterTypes.Contains(typestring))
                {
                    if (Constants.ValidAutoParameterBundleTypes.Contains(typestring))
                    {
                        if (varSymbol.Type is IArrayTypeSymbol)
                        {
                            diagnostic = Diagnostic.Create(Rule_1_1, varDecl.GetLocation(), varSymbol.Name,
                                varSymbol.Type.ToMinimalDisplayString(context.SemanticModel, 0));
                            context.ReportDiagnostic(diagnostic);
                        }
                        else
                        {
                            diagnostic = Diagnostic.Create(Rule_1_2, varDecl.GetLocation(), varSymbol.Name,
                                varSymbol.Type.ToMinimalDisplayString(context.SemanticModel, 0));
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                    else
                    {
                        diagnostic = Diagnostic.Create(Rule_1_1, varDecl.GetLocation(), varSymbol.Name,
                            varSymbol.Type.ToMinimalDisplayString(context.SemanticModel, 0));
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }

            // If is not contained in another type, return; shouldnt really be possible
            if (varSymbol.ContainingType == null)
            {
                diagnostic = Diagnostic.Create(Rule_1_5, varDecl.GetLocation(), varDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
                return;
            }

            // If is not contained in type with AutoParametersAttibute, report
            if (!varSymbol.ContainingType.GetAttributes().Any(attr =>
                    attr.AttributeClass?.ToDisplayString() == Constants.AutoParametersAttributeName))
            {
                diagnostic = Diagnostic.Create(Rule_1_5, varDecl.GetLocation(), varDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
                return;
            }

            if (varSymbol.DeclaredAccessibility == Accessibility.Public) return;
            if (varSymbol.GetAttributes().Any(attr =>
                    attr.AttributeClass?.ToDisplayString() == Constants.SerializeFieldAttributeName)) return;

            diagnostic = Diagnostic.Create(Rule_1_7, varDecl.Identifier.GetLocation(), varDecl.Identifier.Text);
            context.ReportDiagnostic(diagnostic);
        }


        private void CheckSyntaxSetParameters(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDecl,
            IEnumerable<MethodDeclarationSyntax> methods, INamedTypeSymbol objectType, INamedTypeSymbol stringType)
        {
            // If implements SetParameters(object, IDictionary<string, string>)
            var model = context.SemanticModel;
            var setParamCandidates = methods.Where(method => method.Identifier.Text == "SetParameters");

            foreach (var candidate in setParamCandidates)
            {
                // If doesnt have exactly two parameter, continue
                if (candidate.ParameterList.Parameters == null ||
                    candidate.ParameterList.Parameters.Count != 2) continue;

                // If first parameter is not System.Object, continue
                if (model.GetTypeInfo(candidate.ParameterList.Parameters[0].Type).Type.SpecialType !=
                    SpecialType.System_Object) continue;

                // If second parameter is not IDictionary<string, string>, continue
                if (!Utility.IsSyntaxIDictionaryStringString(context, candidate.ParameterList.Parameters[1],
                        stringType)) continue;

                Diagnostic diagnostic =
                    Diagnostic.Create(Rule_1, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void CheckSyntaxValidateParameters(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDecl,
            IEnumerable<MethodDeclarationSyntax> methods, INamedTypeSymbol stringType)
        {
            // If implements SetParameters(object, IDictionary<string, string>)
            var setParamCandidates = methods.Where(method => method.Identifier.Text == "ValidateParameters");
            foreach (var candidate in setParamCandidates)
            {
                // If doesnt have exactly one parameter, continue
                if (candidate.ParameterList.Parameters == null ||
                    candidate.ParameterList.Parameters.Count != 1) continue;

                // If first parameter is not IDictionary<string, string>, continue
                if (!Utility.IsSyntaxIDictionaryStringString(context, candidate.ParameterList.Parameters[0],
                        stringType)) continue;

                Diagnostic diagnostic =
                    Diagnostic.Create(Rule_2, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void CheckSyntaxGetNewCustomData(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDecl,
            IEnumerable<MethodDeclarationSyntax> methods)
        {
            // If implements SetParameters(object, IDictionary<string, string>)
            var setParamCandidates = methods.Where(method => method.Identifier.Text == "GetNewCustomData");
            foreach (var candidate in setParamCandidates)
            {
                // If has parameters, continue
                if (candidate.ParameterList.Parameters != null &&
                    candidate.ParameterList.Parameters.Count != 0) continue;

                Diagnostic diagnostic =
                    Diagnostic.Create(Rule_3, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }


        private bool IsAutoParameter(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax syntax)
        {
            SemanticModel semanticModel = context.SemanticModel;

            bool hasDerivedAttribute = syntax.AttributeLists
                .SelectMany(attributeList => attributeList.Attributes)
                .Any(attribute =>
                {
                    var current = semanticModel.GetTypeInfo(attribute).Type;

                    while (current != null)
                    {
                        if (current.ToDisplayString() == Constants.AutoParameterAttributeName)
                        {
                            return true;
                        }

                        current = current.BaseType;
                    }

                    return false;
                });

            return hasDerivedAttribute;
        }

        private bool IsAutoBundleParameter(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax syntax)
        {
            SemanticModel semanticModel = context.SemanticModel;

            bool hasDerivedAttribute = syntax.AttributeLists
                .SelectMany(attributeList => attributeList.Attributes)
                .Any(attribute =>
                {
                    var current = semanticModel.GetTypeInfo(attribute).Type;

                    while (current != null)
                    {
                        if (current.Name == Constants.AutoParameterBundleAttributeName)
                        {
                            return true;
                        }

                        current = current.BaseType;
                    }

                    return false;
                });

            return hasDerivedAttribute;
        }

        private bool IsAutoParameter(ISymbol type)
        {
            var attributes = type.GetAttributes();

            foreach (var att in attributes)
            {
                var current = att.AttributeClass;

                while (current != null)
                {
                    if (current.ToDisplayString() == Constants.AutoParameterAttributeName)
                    {
                        return true;
                    }

                    current = current.BaseType;
                }
            }

            return false;
        }

        private bool IsAutoParameterBundle(ISymbol type)
        {
            var attributes = type.GetAttributes();

            foreach (var att in attributes)
            {
                var current = att.AttributeClass;

                while (current != null)
                {
                    if (current.ToDisplayString() == Constants.AutoParameterBundleAttributeName)
                    {
                        return true;
                    }

                    current = current.BaseType;
                }
            }

            return false;
        }
    }
}