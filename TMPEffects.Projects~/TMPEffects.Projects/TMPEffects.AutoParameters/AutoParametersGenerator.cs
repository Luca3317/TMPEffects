using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TMPEffects.AutoParameters.Analyzers;
using TMPEffects.StringLibrary;

namespace TMPEffects.AutoParameters.Generator
{
    [Generator]
    public class AutoParametersGenerator : ISourceGenerator
    {
        public const string DiagnosticId___ = "DebuggingError2";
        private static readonly LocalizableString Title___ = "Debugerror";
        private static readonly LocalizableString MessageFormat___ = "{0}";
        private const string Category___ = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        internal static readonly DiagnosticDescriptor Rule___ = new DiagnosticDescriptor(DiagnosticId___, Title___,
            MessageFormat___, Category___, DiagnosticSeverity.Warning, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking


        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver("AutoParameters"));
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Location last = null;
            foreach (var syntaxTree in context.Compilation.SyntaxTrees)
            {
                var model = context.Compilation.GetSemanticModel(syntaxTree);
                var classes = syntaxTree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>();

                foreach (var c in classes)
                {
                    last = c.GetLocation();
                }
            }

            AttributeSyntaxReceiver receiver = context.SyntaxReceiver as AttributeSyntaxReceiver;
            if (receiver == null) return;

            foreach (var typeDecl in receiver.TypeDeclarations)
            {
                SemanticModel model = context.Compilation.GetSemanticModel(typeDecl.SyntaxTree);
                ISymbol symbol = model.GetDeclaredSymbol(typeDecl);

                // Check whether attribute actually is the correct attribute
                bool isDecorated = false;
                foreach (var attributeData in symbol.GetAttributes())
                {
                    var attClass = attributeData.AttributeClass;

                    if (attClass.ToDisplayString() == Strings.AutoParametersAttributeName)
                    {
                        isDecorated = true;
                        break;
                    }
                }

                if (isDecorated)
                {
                    try
                    {
                        CreateAutoParameters(context, model, typeDecl, symbol);
                    }
                    catch (System.Exception ex)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule___, typeDecl.GetLocation(),
                            "Fucked it up " + ex.Message));
                    }
                }
            }
        }


        private void CreateAutoParameters(GeneratorExecutionContext context, SemanticModel model,
            TypeDeclarationSyntax syntax, ISymbol symbol)
        {
            INamedTypeSymbol typeSymbol = symbol as INamedTypeSymbol;
            if (typeSymbol == null) return;

            // Get namespace
            var namespaceName = typeSymbol.ContainingNamespace.ToDisplayString();

            // Get type name
            var typeName = typeSymbol.Name;
            var fullTypeName = typeSymbol.ToDisplayString();

            // Get all auto parameters
            var parameters = GetAutoParameters(typeSymbol);
            var bundles = GetAutoParameterBundles(typeSymbol);
            parameters.AddRange(bundles);

            // Get auto parameters storage (or null)
            var storage = GetAutoParametersStorage(typeSymbol);

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
                            SyntaxFactory.IdentifierName(p.Item1.Type.ToDisplayString()),
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(p.Item1.Name))
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
                            SyntaxFactory.IdentifierName(p.Item1.Type.ToDisplayString()),
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(p.Item1.Name))
                            )
                        )
                    ).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                    storageDecl = storageDecl.AddMembers(fieldDeclaration);
                }

                typeDecl = typeDecl.WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(storageDecl));
            }

            // Get ValidateParameters syntax
            var validateParameters = CreateValidateParameters(typeSymbol, context, parameters);
            var setParameters = CreateSetParameters(typeSymbol, storageDecl.Identifier.Text, context, parameters);
            var getNewCustomData = CreateGetNewCustomData(typeSymbol, storageDecl.Identifier.Text, context, parameters);
            var animate = CreateAnimate(storageDecl.Identifier.Text);
            var partialAnimate = CreatePartialAnimate(storageDecl.Identifier.Text);

            // Add methods to type declaration
            typeDecl = typeDecl.AddMembers(validateParameters)
                .AddMembers(setParameters)
                .AddMembers(getNewCustomData)
                .AddMembers(animate)
                .AddMembers(partialAnimate);

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


        private ClassDeclarationSyntax CreateAutoParameterStorage(ISymbol symbol, TypeDeclarationSyntax typeDecl,
            GeneratorExecutionContext context)
        {
            var storage = SyntaxFactory.ClassDeclaration(Strings.DefaultStorageName);
            return storage;
        }


        private MethodDeclarationSyntax CreateValidateParameters(INamedTypeSymbol symbol,
            GeneratorExecutionContext context, List<(IFieldSymbol, AttributeData)> parameters)
        {
            // Prepare the parameters
            var paramList = SyntaxFactory.ParameterList().AddParameters(SyntaxFactory
                .Parameter(SyntaxFactory.Identifier("parameters"))
                .WithType(SyntaxFactory.ParseTypeName("System.Collections.Generic.IDictionary<string, string>")));

            // Prepare the method
            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("bool"), "ValidateParameters")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword)).WithParameterList(paramList);

            // Prepare all statements
            var statements = new List<StatementSyntax>();

            // Check if there are any required parameters
            bool hasRequired = false;
            foreach (var param in parameters)
            {
                if (param.Item2.ConstructorArguments.Length == 0)
                    continue;

                var arg0 = param.Item2.ConstructorArguments[0];
                if (arg0.Type.SpecialType == SpecialType.System_Boolean && (bool)arg0.Value)
                {
                    hasRequired = true;
                    break;
                }
            }

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
                if (candidate.Parameters == null || candidate.Parameters.Length != 1) continue;

                // If first parameter is not IDictionary<string, string>, continue
                if (!Utility.IsSymbolIDictionaryStringString(candidate.Parameters[0], stringType)) continue;

                present = true;
                break;
            }

            // If there is a hook method, add its invocation
            if (present)
            {
                var arguments = new List<ArgumentSyntax>()
                    { SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameters")) };
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
                SyntaxFactory.LiteralExpression(!hasRequired
                    ? SyntaxKind.TrueLiteralExpression
                    : SyntaxKind.FalseLiteralExpression));
            var ifStatement = SyntaxFactory.IfStatement(condition, returnStatement);
            statements.Add(ifStatement);

            // Add all checks
            foreach (var param in parameters)
            {
                contextt = context;
                var validationSyntax = Utility.GetValidationSyntax("parameters", param.Item1, param.Item2);
                if (validationSyntax != null) statements.Add(validationSyntax);
            }

            // return true if passed all checks
            returnStatement =
                SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));
            statements.Add(returnStatement);

            // Add statements to method body
            return method.WithBody(SyntaxFactory.Block(statements));
        }

        public static GeneratorExecutionContext contextt;

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

        private MethodDeclarationSyntax CreateSetParameters(INamedTypeSymbol symbol, string storageName,
            GeneratorExecutionContext context, List<(IFieldSymbol, AttributeData)> parameters)
        {
            // Prepare the parameters
            var paramList = SyntaxFactory.ParameterList()
                .AddParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("customData"))
                    .WithType(SyntaxFactory.ParseTypeName("object"))).AddParameters(SyntaxFactory
                    .Parameter(SyntaxFactory.Identifier("parameters"))
                    .WithType(SyntaxFactory.ParseTypeName("System.Collections.Generic.IDictionary<string, string>")));

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
                var setParameterSyntax = Utility.GetSetParameterSyntax("parameters", "d", param.Item1, param.Item2);
                if (setParameterSyntax != null) statements.Add(setParameterSyntax);
            }

            var stringType = context.Compilation.GetSpecialType(SpecialType.System_String);
            var hookCandidates = symbol.GetMembers().Where(member => member.Kind == SymbolKind.Method)
                .Select(member => member as IMethodSymbol).Where(hookMethod => hookMethod.Name == "SetParameters_Hook");
            bool present = false;
            foreach (var candidate in hookCandidates)
            {
                if (candidate.Parameters == null || candidate.Parameters.Length != 2) continue;

                // If first parameter is not object, continue
                if (candidate.Parameters[0].Type.SpecialType != SpecialType.System_Object) continue;

                // If second parameter is not IDictionary<string, string>, continue
                if (!Utility.IsSymbolIDictionaryStringString(candidate.Parameters[1], stringType)) continue;

                present = true;
                break;
            }

            // Call hook
            if (present)
            {
                var arguments = new List<ArgumentSyntax>()
                {
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("customData")),
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameters"))
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
            // Prepare the method
            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("object"), "GetNewCustomData")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));

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
                if (candidate.Parameters == null || candidate.Parameters.Length != 1) continue;

                if (candidate.Parameters[0].Type.SpecialType != SpecialType.System_Object) continue;

                present = true;
                break;
            }

            // Call hook
            if (present)
            {
                var arguments = new List<ArgumentSyntax>()
                    { SyntaxFactory.Argument(SyntaxFactory.IdentifierName("d")) };
                var hc = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("GetNewCustomData_Hook"),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));
                statements.Add(SyntaxFactory.ExpressionStatement(hc));
            }

            statements.Add(SyntaxFactory.ParseStatement("return d;"));

            // Add statements to method body
            return method.WithBody(SyntaxFactory.Block(statements));
        }


        private List<(IFieldSymbol, AttributeData)> GetAutoParameters(INamedTypeSymbol typeSymbol)
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

        private List<(IFieldSymbol, AttributeData)> GetAutoParameterBundles(INamedTypeSymbol typeSymbol)
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


        private INamedTypeSymbol GetAutoParametersStorage(INamedTypeSymbol typeSymbol)
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
    }
}