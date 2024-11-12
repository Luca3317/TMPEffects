using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using TMPEffects.AutoParameters.TMPEffects.AutoParameters.Generator;
using TMPEffects.StringLibrary;

namespace TMPEffects.AutoParameters.Generator.Generator
{
    public partial class AutoParametersGenerator
    {
        private void CreateAutoParameters(GeneratorExecutionContext context, SemanticModel model,
            TypeDeclarationSyntax type, ISymbol symbol)
        {
#if DEBUG
            context.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
                "Attempting to create AutoParameters"));
#endif

            INamedTypeSymbol typeSymbol = symbol as INamedTypeSymbol;
            if (typeSymbol == null)
            {
#if DEBUG
                context.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0], "Early return - not a type"));
#endif
                return;
            }

            bool implementsITMPAnimation = Utility.Implements(typeSymbol, Strings.ITMPAnimationName);
            bool implementsITMPCommand = Utility.Implements(typeSymbol, Strings.ITMPCommandName);

            if (!implementsITMPAnimation && !implementsITMPCommand)
            {
#if DEBUG
                context.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
                    "Early return - not ITMPAnimation nor ITMPCommand"));
#endif
                return;
            }

            // Get namespace
            var namespaceName = typeSymbol.ContainingNamespace.ToDisplayString();

            // Get type name
            var typeName = typeSymbol.Name;
            var fullTypeName = typeSymbol.ToDisplayString();

            // Get all auto parameters
            var parameters = Utility.GetAutoParametersNEW(typeSymbol);
            
            foreach (var par in parameters)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
                    "PARAMETER: " + par.FieldName + " : " + par.TypeString + " " + par.DisplayNameString));
            }
            
            var bundles = Utility.GetAutoParameterBundlesNEW(typeSymbol);
#if DEBUG
            context.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
                "AutoParameters: " + string.Join(", ", parameters.Select(n => n.DisplayNameString))));
            context.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
                "AutoParameter Bundles: " + string.Join(", ", bundles.Select(n => n.DisplayNameString))));
#endif

            // Get auto parameters storage (or null)
            var storage = Utility.GetAutoParametersStorage(typeSymbol);

#if DEBUG
            context.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
                "Storage: " + (storage == null ? "null" : storage.Name)));
            context.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0], "Got everything successfully"));
#endif

            // Prepare the type declaration
            TypeDeclarationSyntax typeDecl;
            switch (typeSymbol.TypeKind)
            {
                case TypeKind.Class:
                    typeDecl = SyntaxFactory.ClassDeclaration(typeName);
                    break;
                case TypeKind.Struct:
                    typeDecl = SyntaxFactory.StructDeclaration(typeName);
                    break;
                //case TypeKind.Record:
                default: throw new System.ArgumentException();
            }

            typeDecl = typeDecl.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

#if DEBUG
            context.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
                "Typedeclaration prepared: " + (typeDecl != null)));
#endif

            var storageDecl = CreateStorage(ref typeDecl, parameters, bundles, storage);

            List<MethodDeclarationSyntax> methods = new List<MethodDeclarationSyntax>();
#if DEBUG
            context.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
                "Successfully created storage"));
#endif

            if (implementsITMPAnimation)
            {
                methods.AddRange(CreateITMPAnimationSpecific(typeSymbol, context, parameters, bundles, storageDecl));
#if DEBUG
                context.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
                    "Successfully created animation specific"));
#endif
            }

            if (implementsITMPCommand)
            {
                methods.AddRange(CreateITMPCommandSpecific(typeSymbol, context, parameters, bundles, storageDecl));
#if DEBUG
                context.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
                    "Successfully created command specific"));
#endif
            }

            // Add methods to type declaration
            typeDecl = typeDecl.AddMembers(methods.Cast<MemberDeclarationSyntax>().ToArray());

            // Prepare the namespace declaration
            var namespaceDecl = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName))
                .AddMembers(typeDecl);

            var compilationUnit = SyntaxFactory.CompilationUnit();

            compilationUnit = compilationUnit.AddMembers(typeSymbol.ContainingNamespace.IsGlobalNamespace
                ? (MemberDeclarationSyntax)typeDecl
                : namespaceDecl);

            compilationUnit =
                compilationUnit.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")));
            compilationUnit =
                compilationUnit.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("UnityEngine")));

            // Prepare and add source
            var source = SourceText.From(compilationUnit.NormalizeWhitespace().ToFullString(), Encoding.UTF8);
            context.AddSource($"{fullTypeName}.autoparams.g.cs", source);
        }


        private ClassDeclarationSyntax CreateStorage(ref TypeDeclarationSyntax typeDecl,
            List<Utility.AutoParameterInfo> parameters,
            List<Utility.AutoParameterBundleInfo> bundles, INamedTypeSymbol storage)
        {
            // Add storage to new type
            ClassDeclarationSyntax storageDecl;
            if (storage == null)
            {
                storageDecl = SyntaxFactory.ClassDeclaration(Strings.DefaultStorageName)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword))
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

                foreach (var p in parameters)
                {
                    var fieldDeclaration = SyntaxFactory.FieldDeclaration(
                        SyntaxFactory.VariableDeclaration(
                            SyntaxFactory.IdentifierName(p.TypeString + (p.IsArray ? "[]" : "")),
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(p.FieldName))
                            )
                        )
                    ).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                    storageDecl = storageDecl.AddMembers(fieldDeclaration);
                }

                foreach (var b in bundles)
                {
                    var fieldDeclaration = SyntaxFactory.FieldDeclaration(
                        SyntaxFactory.VariableDeclaration(
                            SyntaxFactory.IdentifierName(b.TypeString),
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(b.FieldName))
                            )
                        )
                    ).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                    storageDecl = storageDecl.AddMembers(fieldDeclaration);
                }

                typeDecl = typeDecl.WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(storageDecl));
            }
            else
            {
                storageDecl = storage.DeclaringSyntaxReferences[0].GetSyntax() as ClassDeclarationSyntax;

                storageDecl = SyntaxFactory.ClassDeclaration(storageDecl.Identifier)
                    .WithModifiers(storageDecl.Modifiers).WithTypeParameterList(storageDecl.TypeParameterList)
                    .WithBaseList(storageDecl.BaseList).WithConstraintClauses(storageDecl.ConstraintClauses);

                foreach (var p in parameters)
                {
                    var fieldDeclaration = SyntaxFactory.FieldDeclaration(
                        SyntaxFactory.VariableDeclaration(
                            SyntaxFactory.IdentifierName(p.TypeString),
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(p.FieldName))
                            )
                        )
                    ).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                    storageDecl = storageDecl.AddMembers(fieldDeclaration);
                }

                foreach (var b in bundles)
                {
                    var fieldDeclaration = SyntaxFactory.FieldDeclaration(
                        SyntaxFactory.VariableDeclaration(
                            SyntaxFactory.IdentifierName(b.TypeString),
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(b.FieldName))
                            )
                        )
                    ).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                    storageDecl = storageDecl.AddMembers(fieldDeclaration);
                }

                typeDecl = typeDecl.WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(storageDecl));
            }

            return storageDecl;
        }


        private IEnumerable<MethodDeclarationSyntax> CreateITMPAnimationSpecific(
            INamedTypeSymbol typeSymbol, GeneratorExecutionContext context,
            List<Utility.AutoParameterInfo> parameters, List<Utility.AutoParameterBundleInfo> bundles,
            ClassDeclarationSyntax storageDecl)
        {
            // Get ValidateParameters syntax
            var validateParameters =
                CreateValidateParameters(typeSymbol, context, parameters, bundles, Strings.IAnimatorContextName,
                    Strings.IAnimatorContextKeywordDatabasePath);

            var setParameters =
                CreateAnimationSetParameters(typeSymbol, storageDecl.Identifier.Text, context, parameters, bundles);

            var getNewCustomData =
                CreateGetNewCustomData(typeSymbol, storageDecl.Identifier.Text, context, parameters, bundles);

            var animate = CreateAnimate(storageDecl.Identifier.Text);

            var partialAnimate = CreatePartialAnimate(storageDecl.Identifier.Text);

            return new List<MethodDeclarationSyntax>()
            {
                partialAnimate,
                animate,
                getNewCustomData,
                setParameters,
                validateParameters
            };
        }

        private IEnumerable<MethodDeclarationSyntax> CreateITMPCommandSpecific(
            INamedTypeSymbol typeSymbol, GeneratorExecutionContext context,
            List<Utility.AutoParameterInfo> parameters, List<Utility.AutoParameterBundleInfo> bundles,
            ClassDeclarationSyntax storageDecl)
        {
            // Get ValidateParameters syntax
            var validateParameters = CreateValidateParameters(typeSymbol, context, parameters, bundles,
                Strings.IWriterContextName, Strings.IWriterContexKeywordDatabasePath);

            var setParameters =
                CreateCommandSetParameters(typeSymbol, storageDecl.Identifier.Text, context, parameters, bundles);

            var execute = CreateExecute(storageDecl.Identifier.Text);
            var partialExecute = CreatePartialExecute(storageDecl.Identifier.Text);

            return new List<MethodDeclarationSyntax>()
            {
                partialExecute,
                execute,
                setParameters,
                validateParameters
            };
        }

        // Works for both Commands and Animations
        public static MethodDeclarationSyntax CreateValidateParameters(INamedTypeSymbol symbol,
            GeneratorExecutionContext context, List<Utility.AutoParameterInfo> parameters,
            List<Utility.AutoParameterBundleInfo> bundles,
            string contextName, string keywordsPath)
        {
            // Prepare the parameters
            var paramList = SyntaxFactory.ParameterList()
                .AddParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("parameters"))
                    .WithType(SyntaxFactory.ParseTypeName("System.Collections.Generic.IDictionary<string, string>")))
                .AddParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("context"))
                    .WithType(SyntaxFactory.ParseTypeName(contextName)));

            // Prepare the method
            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("bool"), "ValidateParameters")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword)).WithParameterList(paramList);

            // Prepare all statements
            var statements = new List<StatementSyntax>();

            // Get all hookCandidates (potential hook methods)
            var stringType = context.Compilation.GetSpecialType(SpecialType.System_String);
            var hookCandidates = symbol.GetMembers().Where(member => member.Kind == SymbolKind.Method)
                .Select(member => member as IMethodSymbol).Where(hookMethod =>
                    hookMethod.Name == "ValidateParameters_Hook" &&
                    hookMethod.ReturnType.SpecialType == SpecialType.System_Boolean);

            // Check if any of the candidates are the actual hook method
            bool present = false;
            foreach (var candidate in hookCandidates)
            {
                if (candidate.Parameters == null || candidate.Parameters.Length != 2) continue;

                // If first parameter is not IDictionary<string, string>, continue
                if (!Utility.IsSymbolIDictionaryStringString(candidate.Parameters[0], stringType)) continue;

                if (candidate.Parameters[1].Type.ToDisplayString() != contextName) continue;

                present = true;
                break;
            }

            // If there is a hook method, add its invocation
            if (present)
            {
                var arguments = new List<ArgumentSyntax>()
                {
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameters")),
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("context"))
                };
                var hc = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("ValidateParameters_Hook"),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));
                var negation = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, hc);
                var hookReturnStatement =
                    SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression));
                var hookIfStatement = SyntaxFactory.IfStatement(negation, hookReturnStatement);
                statements.Add(hookIfStatement);
            }

            // Return if parameters null; true or false depending on whether there are required parameters
            var condition = SyntaxFactory.BinaryExpression(
                SyntaxKind.EqualsExpression, SyntaxFactory.IdentifierName("parameters"),
                SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            var returnStatement = SyntaxFactory.ReturnStatement(
                SyntaxFactory.LiteralExpression(!parameters.Any(x => x.required)
                    ? SyntaxKind.TrueLiteralExpression
                    : SyntaxKind.FalseLiteralExpression));
            var ifStatement = SyntaxFactory.IfStatement(condition, returnStatement);
            statements.Add(ifStatement);

            // Add all checks
            foreach (var param in parameters)
            {
                var validationSyntax =
                    Utility.GetValidationSyntax("parameters", param, keywordsPath);
                if (validationSyntax != null) statements.Add(validationSyntax);
            }

            foreach (var bundle in bundles)
            {
                var validationSyntax =
                    Utility.GetValidationSyntax("parameters", bundle, keywordsPath);
                if (validationSyntax != null) statements.Add(validationSyntax);
            }

            // return true if passed all checks
            returnStatement =
                SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));
            statements.Add(returnStatement);

            // Add statements to method body
            return method.WithBody(SyntaxFactory.Block(statements));
        }
    }
}