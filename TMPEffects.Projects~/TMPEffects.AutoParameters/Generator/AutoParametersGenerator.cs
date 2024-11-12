using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TMPEffects.StringLibrary;

namespace TMPEffects.AutoParameters.Generator.Generator
{
    [Generator]
    public partial class AutoParametersGenerator : ISourceGenerator
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

                if (!isDecorated) continue;

                INamedTypeSymbol typeSymbol = symbol as INamedTypeSymbol;

                try
                {
                    CreateAutoParameters(context, model, typeDecl, symbol);
                }
                catch (System.Exception ex)
                {
                    Diagnostic d = Diagnostic.Create(Rule___, typeDecl.GetLocation(),
                        "Failed to create AutoParameters on " +
                        typeDecl.Identifier.Text + ". " +
                        "This is most likely a bug -- feel free to open an issue or a pull request with your fix on " +
                        "https://github.com/Luca3317/TMPEffects");
                    context.ReportDiagnostic(d);
                }
            }
        }
    }
}