using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using TMPEffects.StringLibrary;

namespace TMPEffects.ParameterUtilityGenerator
{
    public class AttributeSyntaxReceiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> TypeDeclarations { get; private set; }

        private readonly string[] attributeNames;

        public string lastfail;


        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            TypeDeclarationSyntax typeDecl;
            if ((typeDecl = syntaxNode as TypeDeclarationSyntax) == null || syntaxNode is InterfaceDeclarationSyntax)
                return;

            if (typeDecl.AttributeLists.Count == 0)
                return;

            if (!typeDecl.AttributeLists.Any(a =>
                    a.Attributes.Any(b => attributeNames.Contains(b.Name.ToFullString()))))
                return;

            TypeDeclarations.Add(typeDecl);
        }

        public AttributeSyntaxReceiver(params string[] attributeName)
        {
            TypeDeclarations = new List<TypeDeclarationSyntax>();
            this.attributeNames = attributeName;
        }
    }

    [Generator]
    public class ParameterUtilityGenerator : ISourceGenerator
    {
        public const string DiagnosticId___ = "DebuggingError2";
        private static readonly LocalizableString Title___ = "Debugerror";
        private static readonly LocalizableString MessageFormat___ = "{0}";
        private const string Category___ = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor Rule___ = new DiagnosticDescriptor(DiagnosticId___, Title___,
            MessageFormat___, Category___, DiagnosticSeverity.Warning, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public const string GenerateParameterTypeAttributeName = "TMPEffects.Parameters.GenerateParameterUtilityAttribute";
        
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver("GenerateParameterUtility"));
        }

        public void Execute(GeneratorExecutionContext context)
        {
            AttributeSyntaxReceiver receiver = context.SyntaxReceiver as AttributeSyntaxReceiver;
            if (receiver == null) return;
            
            foreach (var typeDecl in receiver.TypeDeclarations)
            {
                SemanticModel model = context.Compilation.GetSemanticModel(typeDecl.SyntaxTree);
                ISymbol symbol = ModelExtensions.GetDeclaredSymbol(model, typeDecl);

                // Check whether attribute actually is the correct attribute
                foreach (var attributeData in symbol.GetAttributes())
                {
                    var attClass = attributeData.AttributeClass;

                    try
                    {
                        if (attClass.ToDisplayString() == GenerateParameterTypeAttributeName)
                        {
                            foreach (var type in Strings.SupportedTypesList)
                            {
                                CreateTypeParameter(context, model, typeDecl, symbol, type.Item1, type.Item2);
                            }
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule___, typeDecl.GetLocation(),
                            "ParameterUtility fucked it up " + e.Message));
                    }
                }
            }
        }

        public static INamedTypeSymbol stringType;

        private void CreateTypeParameter(GeneratorExecutionContext context, SemanticModel model,
            TypeDeclarationSyntax syntax, ISymbol symbol, string genTypeName, string genDisplayName)
        {
            INamedTypeSymbol typeSymbol = symbol as INamedTypeSymbol;
            if (typeSymbol == null) return;

            stringType = context.Compilation.GetSpecialType(SpecialType.System_String);

            // Get type name
            var typeName = typeSymbol.Name;
            var fullTypeName = typeSymbol.ToDisplayString();

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

            (string type, string displayName) values = (genTypeName, genDisplayName);

            exc = context;
            bool addHasParameter = !ImplementsHasParameter(typeSymbol, values);
            bool addHasNonParameter = !ImplementsHasNonParameter(typeSymbol, values);
            bool addTryGetParameter = !ImplementsTryGetParameter(typeSymbol, values);

            string code =
                @"using System;
using System.Collections.Generic;

namespace TMPEffects.Parameters
{
    // <auto-generated />
    // This class is auto-generated; changing it might break [AutoParameters] and existing animations + commands
    public static partial class ParameterUtility
    {";

            if (addHasParameter)
            {
                string name = values.type.Substring(values.type.LastIndexOf('.') + 1);
                string fullName = values.type;

                code += $@" 
        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is of type {name} (=> can be converted to {name}).<br />
        /// </summary>
        /// <param name=""parameters"">The parameters to check.</param>
        /// <param name=""name"">The name to check.</param>
        /// <param name=""aliases"">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is of type {name}, false otherwise.</returns>
        public static bool Has{values.displayName}Parameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {{
            return TryGet{values.displayName}Parameter(out {fullName} _, parameters, name, aliases);
        }}
";
            }

            if (addHasNonParameter)
            {
                string name = values.type.Substring(values.type.LastIndexOf('.') + 1);
                string fullName = values.type;

                code += $@" 
        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is not of type {name} (=> can not be converted to {name}).<br />
        /// </summary>
        /// <param name=""parameters"">The parameters to check.</param>
        /// <param name=""name"">The name to check.</param>
        /// <param name=""aliases"">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is not of type {name}, false otherwise.</returns>
        public static bool HasNon{values.displayName}Parameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {{
            if (!ParameterDefined(parameters, name, aliases)) return false;
            return !TryGet{values.displayName}Parameter(out {fullName} _, parameters, name, aliases);
        }}
";
            }

            if (addTryGetParameter)
            {
                string name = values.type.Substring(values.type.LastIndexOf('.') + 1);
                string fullName = values.type;
                
                code += $@"
        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is of type {name} (=> can be converted to {name}).<br />
        /// </summary>
        /// <param name=""value"">Set to the value of the parameter if successful.</param>
        /// <param name=""parameters"">The parameters to check.</param>
        /// <param name=""name"">The name to check.</param>
        /// <param name=""aliases"">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is of type {name}, false otherwise.</returns>
        public static bool TryGet{values.displayName}Parameter(out {fullName} value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {{
            value = default;
            if (!TryGetDefinedParameter(out string parameterName, parameters, name, aliases)) return false;
            return ParameterParsing.StringTo{values.displayName}(parameters[parameterName], out value);
        }}
";
            }

            code += @"
    }
}";

            // Prepare and add source
            var source = SourceText.From(code, Encoding.UTF8);
            context.AddSource($"{typeName}.{values.displayName}.g.cs", source);
        }

        private static (INamedTypeSymbol type, string displayName) GetAttributeValues(AttributeData att)
        {
            INamedTypeSymbol type = att.ConstructorArguments[0].Value as INamedTypeSymbol;
            string displayName = att.ConstructorArguments[1].Value as string;

            return (type, displayName);
        }

        private bool ImplementsTryGetParameter(INamedTypeSymbol symbol,
            (string type, string displayName) values)
        {
            return symbol.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(method => method.Name == $"TryGet{values.displayName}Parameter")
                .Where(method => method.Parameters.Length == 4)
                .Where(method => method.Parameters[0].RefKind == RefKind.Out)
                .Where(method => method.Parameters[0].Type.ToDisplayString() == values.type)
                .Where(method => IsSymbolIDictionaryStringString(method.Parameters[1]))
                .Where(method => SymbolEqualityComparer.Default.Equals(method.Parameters[2].Type, stringType))
                .Any(method => method.Parameters[3].IsParams &&
                               SymbolEqualityComparer.Default.Equals(
                                   (method.Parameters[3].Type as IArrayTypeSymbol).ElementType, stringType));
        }

        private static GeneratorExecutionContext exc;

        private bool ImplementsHasParameter(INamedTypeSymbol symbol, (string type, string displayName) values)
        {
            // exc.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
            //     "Step 0_0"));
            //
            // var methods = symbol.GetMembers().OfType<IMethodSymbol>().ToList();
            // exc.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
            //     "Step 0_1"));
            //
            // methods = methods.Where(method => method.DeclaredAccessibility == Accessibility.Public && method.IsStatic).ToList();
            // exc.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
            //     "Step 0_2"));
            //
            // methods = methods.Where(method => method.Name == $"Has{values.displayName}Parameter").ToList();
            // exc.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
            //     "Step 0_3"));
            //
            // methods = methods.Where(method => method.Parameters.Length == 3).ToList();
            // exc.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
            //     "Step 0_4"));
            //
            // methods = methods.Where(method => IsSymbolIDictionaryStringString(method.Parameters[0])).ToList();
            // exc.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
            //     "Step 0_5"));
            //
            // methods = methods.Where(method =>
            //     SymbolEqualityComparer.Default.Equals(method.Parameters[1].Type, stringType)).ToList();
            // exc.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
            //     "Step 0_6"));
            //
            // methods = methods.Where(method => method.Parameters[2].IsParams &&
            //                                             SymbolEqualityComparer.Default.Equals(
            //                                                 (method.Parameters[2].Type as IArrayTypeSymbol).ElementType, stringType)).ToList();
            // exc.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
            //     "Step 0_7; Is methods null: " + (methods == null)));

            // try
            // {
            //     bool toreturn = methods.Any();
            //     exc.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
            //         "Step 0_8 DONE! w " + toreturn));
            //     return toreturn;
            // }
            // catch (Exception e)
            // {
            //     exc.ReportDiagnostic(Diagnostic.Create(Rule___, symbol.Locations[0],
            //         "Step 0_8 FUCKED! w "));
            //     return false;
            // }

            return symbol.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(method => method.DeclaredAccessibility == Accessibility.Public && method.IsStatic)
                .Where(method => method.Name == $"Has{values.displayName}Parameter")
                .Where(method => method.Parameters.Length == 3)
                .Where(method => IsSymbolIDictionaryStringString(method.Parameters[0]))
                .Where(method => SymbolEqualityComparer.Default.Equals(method.Parameters[1].Type, stringType))
                .Any(method => method.Parameters[2].IsParams &&
                               SymbolEqualityComparer.Default.Equals(
                                   (method.Parameters[2].Type as IArrayTypeSymbol).ElementType, stringType));
        }

        private bool ImplementsHasNonParameter(INamedTypeSymbol symbol,
            (string type, string displayName) values)
        {
            return symbol.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(method => method.DeclaredAccessibility == Accessibility.Public && method.IsStatic)
                .Where(method => method.Name == $"HasNon{values.displayName}Parameter")
                .Where(method => method.Parameters.Length == 3)
                .Where(method => IsSymbolIDictionaryStringString(method.Parameters[0]))
                .Where(method => SymbolEqualityComparer.Default.Equals(method.Parameters[1].Type, stringType))
                .Any(method => method.Parameters[2].IsParams &&
                               SymbolEqualityComparer.Default.Equals(
                                   (method.Parameters[2].Type as IArrayTypeSymbol).ElementType, stringType));
        }


        public static bool IsSymbolIDictionaryStringString(IParameterSymbol param)
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
    }
}