using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using TMPEffects.AutoParameters.Generator;
using TMPEffects.StringLibrary;
using Diagnostic = Microsoft.CodeAnalysis.Diagnostic;
using INamedTypeSymbol = Microsoft.CodeAnalysis.INamedTypeSymbol;

namespace TMPEffects.AutoParameters.Analyzer
{
    public partial class AutoParametersAnalyzer : DiagnosticAnalyzer
    {
        private void TryAnalyzeSyntaxAutoParameters(SyntaxNodeAnalysisContext context)
        {
            TypeDeclarationSyntax typeDecl = (TypeDeclarationSyntax)context.Node;

            try
            {
                var symbol = context.SemanticModel.GetDeclaredSymbol(typeDecl);

                if (symbol.GetAttributes().Any(a =>
                        a.AttributeClass?.ToDisplayString() == Strings.AutoParametersAttributeName))
                {
                    AnalyzeSyntaxAutoParameters(context, typeDecl, symbol);
                }
            }
            catch(System.Exception e)
            {
                Diagnostic d = Diagnostic.Create(Rule___, typeDecl.GetLocation(),
                    "Failed TryAnalyzeSyntaxAutoParameters for " + typeDecl.Identifier.Text + ". " +
                    "This is most likely a bug -- feel free to open an issue or a pull request with your fix on " +
                    "https://github.com/Luca3317/TMPEffects\n" + e.Message);
                context.ReportDiagnostic(d);
            }
        }

        private void AnalyzeSyntaxAutoParameters(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDecl,
            INamedTypeSymbol symbol)
        {
            Diagnostic diagnostic;

            // TODO Report here if on a class that does not implement ITMPCommand or ITMPAnimation (and return)
            bool implementsITMPAnimation = Utility.Implements(symbol, Strings.ITMPAnimationName);
            bool implementsITMPCommand = Utility.Implements(symbol, Strings.ITMPCommandName);

            if (!implementsITMPAnimation && !implementsITMPCommand)
            {
                // TODO Report that this is not valid and return
                Diagnostic d = Diagnostic.Create(Rule___, typeDecl.GetLocation(),
                    "Doesnt implement either; TODO make own rule for this!");
                context.ReportDiagnostic(d);
                return;
            }

            // Get all methods
            var methods = typeDecl.Members.Where(member => member.Kind() == SyntaxKind.MethodDeclaration)
                .Select(method => method as MethodDeclarationSyntax);

            // If multiple storages are defined, report
            var nestedtypes = typeDecl.Members.OfType<BaseTypeDeclarationSyntax>();
            int count = 0;
            string firstFoundName = "";
            foreach (var type in nestedtypes)
            {
                INamedTypeSymbol typeSymbol = context.SemanticModel.GetDeclaredSymbol(type);
                foreach (var att in typeSymbol.GetAttributes())
                {
                    if (att.AttributeClass.ToDisplayString() == Strings.AutoParametersStorageAttributeName)
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

            if (count == 0) firstFoundName = typeDecl.Identifier.Text + "." + Strings.DefaultStorageName;

            // ITMPAnimation specific checks
            if (implementsITMPAnimation)
                ITMPAnimationSpecificChecks(context, typeDecl, methods, firstFoundName);

            // ITMPCommand specific checks
            if (implementsITMPCommand)
                ITMPCommandSpecificChecks(context, typeDecl, methods, firstFoundName);
        }

        #region ITMPAnimation-specific

        private void ITMPAnimationSpecificChecks(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDecl,
            IEnumerable<MethodDeclarationSyntax> methods, string storageName)
        {
            var stringType = context.Compilation.GetSpecialType(SpecialType.System_String);

            // If one of the methods to generate is implemented, report
            CheckSyntaxValidateParameters(context, typeDecl, methods, stringType, Strings.IAnimatorContextName);
            CheckSyntaxSetParameters(context, typeDecl, methods, stringType);
            CheckSyntaxGetNewCustomData(context, typeDecl, methods);
            // TODO Probably Check and Report if main Animate implemented? same for execute command

            // If no partial Animate is implemented, report
            // (and return, no need to check the method if not implemented)
            if (!CheckPartialAnimate(context, typeDecl, methods, storageName))
                return;
            
            // If any autoparameters / bundles are referenced in Animate(), report
            var animateCandidates = typeDecl.Members.Where(member => member.Kind() == SyntaxKind.MethodDeclaration)
                .Select(member => member as MethodDeclarationSyntax)
                .Where(method => method.Identifier.Text == "Animate").ToList();

            MethodDeclarationSyntax animate = null;
            foreach (var candidate in animateCandidates)
            {
                var candidateSymbol = context.SemanticModel.GetDeclaredSymbol(candidate);
                if (candidateSymbol.Parameters == null || candidateSymbol.Parameters.Length != 3) continue;
                if (candidateSymbol.Parameters[0].Type.ToDisplayString() != Strings.CharDataName) continue;
                if (candidateSymbol.Parameters[1].Type.ToDisplayString() != storageName) continue;
                if (candidateSymbol.Parameters[2].Type.ToDisplayString() != Strings.IAnimationContextName) continue;
                animate = candidate;
                break;
            }

            if (animate != null)
                CheckAutoParameterReferenced(animate, context, typeDecl,
                    GetStorageName(typeDecl.Identifier.Text, typeDecl.Members.OfType<TypeDeclarationSyntax>(),
                        context));
        }

        // If implements SetParameters(object, IDictionary<string, string>, IAnimationContext)
        private void CheckSyntaxSetParameters(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDecl,
            IEnumerable<MethodDeclarationSyntax> methods, INamedTypeSymbol stringType)
        {
            var model = context.SemanticModel;
            var setParamCandidates = methods.Where(method => method.Identifier.Text == "SetParameters");

            foreach (var candidate in setParamCandidates)
            {
                // If doesnt have exactly three parameter, continue
                if (candidate.ParameterList.Parameters == null ||
                    candidate.ParameterList.Parameters.Count != 3) continue;

                // If first parameter is not System.Object, continue
                if (model.GetTypeInfo(candidate.ParameterList.Parameters[0].Type).Type.SpecialType !=
                    SpecialType.System_Object) continue;

                // If second parameter is not IDictionary<string, string>, continue
                if (!Utility.IsSyntaxIDictionaryStringString(context, candidate.ParameterList.Parameters[1],
                        stringType)) continue;

                var type = ModelExtensions
                    .GetTypeInfo(context.SemanticModel, candidate.ParameterList.Parameters[2].Type)
                    .Type as INamedTypeSymbol;
                if (type.ToDisplayString() != Strings.IAnimationContextName) continue;

                Diagnostic diagnostic =
                    Diagnostic.Create(Rule_1, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }

        // Check whether implements GetNewCustomData(IAnimationContext)
        private void CheckSyntaxGetNewCustomData(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDecl,
            IEnumerable<MethodDeclarationSyntax> methods)
        {
            var setParamCandidates = methods.Where(method => method.Identifier.Text == "GetNewCustomData");
            foreach (var candidate in setParamCandidates)
            {
                // If has parameters, continue
                if (candidate.ParameterList.Parameters.Count != 1) continue;

                var type = ModelExtensions
                    .GetTypeInfo(context.SemanticModel, candidate.ParameterList.Parameters[0].Type)
                    .Type as INamedTypeSymbol;
                if (type.ToDisplayString() != Strings.IAnimationContextName) continue;

                Diagnostic diagnostic =
                    Diagnostic.Create(Rule_3, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }

        // Check whether implements private partial void Animate(CharData, AutoParametersData, IAnimationContext)
        private bool CheckPartialAnimate(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDecl,
            IEnumerable<MethodDeclarationSyntax> methods, string storageDeclName)
        {
            var model = context.SemanticModel;
            var setParamCandidates = methods.Where(method => method.Identifier.Text == "Animate");

            Diagnostic diagnostic;
            foreach (var candidate in setParamCandidates)
            {
                // If doesnt have exactly three parameter, continue
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
                if (type == null || type.ToDisplayString() != Strings.CharDataName) continue;

                // If second parameter is not the storage, continue
                expressionSyntax = candidate.ParameterList.Parameters[1].Type;
                if (expressionSyntax == null) continue;
                type = context.SemanticModel.GetTypeInfo(expressionSyntax).Type as INamedTypeSymbol;

                if (type == null || type.ToDisplayString() != storageDeclName) continue;

                // If third parameter is not the animationcontext, continue
                expressionSyntax = candidate.ParameterList.Parameters[2].Type;
                if (expressionSyntax == null) continue;
                type = context.SemanticModel.GetTypeInfo(expressionSyntax).Type as INamedTypeSymbol;
                if (type == null || type.ToDisplayString() != Strings.IAnimationContextName) continue;

                return true;
            }

            diagnostic =
                Diagnostic.Create(Rule_5, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
            context.ReportDiagnostic(diagnostic);
            return false;
        }

        // Check whether implements private partial void Animate(CharData, AutoParametersData, IAnimationContext)
        private bool CheckPartialExecute(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDecl,
            IEnumerable<MethodDeclarationSyntax> methods, string storageDeclName, INamedTypeSymbol stringType)
        {
            var model = context.SemanticModel;
            var setParamCandidates = methods.Where(method => method.Identifier.Text == "ExecuteCommand");

            Diagnostic diagnostic;
            foreach (var candidate in setParamCandidates)
            {
                // If doesnt have exactly three parameter, continue
                if (candidate.ParameterList.Parameters == null ||
                    candidate.ParameterList.Parameters.Count != 3) continue;

                // If has no body, continue
                if (candidate.Body == null) continue;

                // If isnt private partial, continue
                bool isPartial = candidate.Modifiers.Any(SyntaxKind.PartialKeyword);
                // bool isPrivate = candidate.Modifiers.Any(SyntaxKind.PrivateKeyword);
                if (!isPartial /*|| !isPrivate*/) continue;

                // If first parameter is not IDict<string,string>, continue
                if (!Utility.IsSyntaxIDictionaryStringString(context, candidate.ParameterList.Parameters[0],
                        stringType)) continue;

                // If second parameter is not the storage, continue
                var expressionSyntax = candidate.ParameterList.Parameters[1].Type;
                if (expressionSyntax == null) continue;
                var type = context.SemanticModel.GetTypeInfo(expressionSyntax).Type as INamedTypeSymbol;
                if (type == null || type.ToDisplayString() != storageDeclName) continue;

                // If third parameter is not the commandcontext, continue
                expressionSyntax = candidate.ParameterList.Parameters[2].Type;
                if (expressionSyntax == null) continue;
                type = context.SemanticModel.GetTypeInfo(expressionSyntax).Type as INamedTypeSymbol;
                if (type == null || type.ToDisplayString() != Strings.ICommandContextName) continue;

                return true;
            }

            // TODO Own rule for this and other command specific shit
            diagnostic =
                Diagnostic.Create(Rule_5, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
            context.ReportDiagnostic(diagnostic);
            return false;
        }

        #endregion

        #region ITMPCommand-specific

        private void ITMPCommandSpecificChecks(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDecl,
            IEnumerable<MethodDeclarationSyntax> methods, string storageName)
        {
            var objectType = context.Compilation.GetSpecialType(SpecialType.System_Object);
            var stringType = context.Compilation.GetSpecialType(SpecialType.System_String);

            CheckSyntaxValidateParameters(context, typeDecl, methods, stringType, Strings.IWriterContextName);
            CheckSyntaxSetParameters(context, typeDecl, methods, stringType);

            // If no partial Animate is implemented, report
            // (and return, no need to check the method if not implemented)
            if (!CheckPartialExecute(context, typeDecl, methods, storageName, stringType))
                return;

            // If any autoparameters / bundles are referenced in ExecuteCommand(), report
            var executeCandidates = typeDecl.Members.Where(member => member.Kind() == SyntaxKind.MethodDeclaration)
                .Select(member => member as MethodDeclarationSyntax)
                .Where(method => method.Identifier.Text == "ExecuteCommand").ToList();

            MethodDeclarationSyntax execute = null;
            foreach (var candidate in executeCandidates)
            {
                var candidateSymbol = context.SemanticModel.GetDeclaredSymbol(candidate);
                if (candidateSymbol.Parameters == null || candidateSymbol.Parameters.Length != 3) continue;

                var l = candidateSymbol.Parameters[0].Type;
                if (!Utility.IsSyntaxIDictionaryStringString(context, candidate.ParameterList.Parameters[0],
                        stringType)) continue;
                if (candidateSymbol.Parameters[1].Type.ToDisplayString() != storageName) continue;
                if (candidateSymbol.Parameters[2].Type.ToDisplayString() != Strings.ICommandContextName) continue;

                execute = candidate;
                break;
            }

            if (execute != null)
                CheckAutoParameterReferenced(execute, context, typeDecl,
                    GetStorageName(typeDecl.Identifier.Text, typeDecl.Members.OfType<TypeDeclarationSyntax>(),
                        context));
        }

        // If implements SetParameters(IDictionary<string, string>, ICommandContext)
        private void CheckCommandSyntaxSetParameters(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDecl,
            IEnumerable<MethodDeclarationSyntax> methods, INamedTypeSymbol objectType, INamedTypeSymbol stringType)
        {
            var model = context.SemanticModel;
            var setParamCandidates = methods.Where(method => method.Identifier.Text == "SetParameters");

            foreach (var candidate in setParamCandidates)
            {
                // If doesnt have exactly two parameter, continue
                if (candidate.ParameterList.Parameters == null ||
                    candidate.ParameterList.Parameters.Count != 2) continue;

                // If first parameter is not IDictionary<string, string>, continue
                if (!Utility.IsSyntaxIDictionaryStringString(context, candidate.ParameterList.Parameters[0],
                        stringType)) continue;

                var type = ModelExtensions
                    .GetTypeInfo(context.SemanticModel, candidate.ParameterList.Parameters[1].Type)
                    .Type as INamedTypeSymbol;
                if (type.ToDisplayString() != Strings.ICommandContextName) continue;

                Diagnostic diagnostic =
                    Diagnostic.Create(Rule_1, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }

        #endregion

        // Check whether implements ValidateParameters(IDictionary<string, string>, ContextName)
        private void CheckSyntaxValidateParameters(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDecl,
            IEnumerable<MethodDeclarationSyntax> methods, INamedTypeSymbol stringType, string contextName)
        {
            var setParamCandidates = methods.Where(method => method.Identifier.Text == "ValidateParameters");

            // Check each candidates signature
            foreach (var candidate in setParamCandidates)
            {
                if (candidate.ParameterList.Parameters.Count != 2) continue;

                if (!Utility.IsSyntaxIDictionaryStringString(context, candidate.ParameterList.Parameters[0],
                        stringType)) continue;

                var type = ModelExtensions
                    .GetTypeInfo(context.SemanticModel, candidate.ParameterList.Parameters[1].Type)
                    .Type as INamedTypeSymbol;
                if (type.ToDisplayString() != contextName) continue;

                Diagnostic diagnostic =
                    Diagnostic.Create(Rule_2, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
                break;
            }
        }

        private void CheckAutoParameterReferenced(MethodDeclarationSyntax methodToCheck,
            SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDecl,
            string storageDeclName)
        {
            var autoParameters = typeDecl.Members.OfType<FieldDeclarationSyntax>()
                .Where(field => Utility.IsAutoParameter(context, field)).ToList();
            
            var autoParameterSymbols = autoParameters.SelectMany(field => field.Declaration.Variables)
                .Select(variable => context.SemanticModel.GetDeclaredSymbol(variable));
            
            var identifierNodes = methodToCheck.DescendantNodes().OfType<IdentifierNameSyntax>();

            foreach (var identifier in identifierNodes)
            {
                ISymbol identifierSymbol = context.SemanticModel.GetSymbolInfo(identifier).Symbol;

                if (autoParameterSymbols.Contains(identifierSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule_4, identifier.GetLocation(), identifierSymbol.Name,
                        identifierSymbol.Name));
                }
            }
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
                    if (att.AttributeClass.ToDisplayString() == Strings.AutoParametersStorageAttributeName)
                    {
                        return typeSymbol.ToDisplayString();
                    }
                }
            }

            return enclosingTypeName + "." + Strings.DefaultStorageName;
        }
    }
}