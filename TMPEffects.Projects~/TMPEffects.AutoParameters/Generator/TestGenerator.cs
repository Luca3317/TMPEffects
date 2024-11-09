using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using TMPEffects.AutoParameters.Analyzer;
using TMPEffects.AutoParameters.TMPEffects.AutoParameters.Generator;
using TMPEffects.StringLibrary;

namespace TMPEffects.AutoParameters.Generator.Generator
{
    [Generator]
    public class TestGenerator : ISourceGenerator
    {
        public const string DiagnosticId___ = "DebuggingError3";
        private static readonly LocalizableString Title___ = "Debugerror";
        private static readonly LocalizableString MessageFormat___ = "{0}";
        private const string Category___ = "Usage";
#pragma warning disable RS2008 // Enable analyzer release tracking
        internal static readonly DiagnosticDescriptor Rule___ = new DiagnosticDescriptor(DiagnosticId___, Title___,
            MessageFormat___, Category___, DiagnosticSeverity.Warning, isEnabledByDefault: true);
#pragma warning restore RS2008 // Enable analyzer release tracking

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver("TestAttribute"));
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
                ISymbol symbol = ModelExtensions.GetDeclaredSymbol(model, typeDecl);

                // Check whether attribute actually is the correct attribute
                bool isDecorated = false;
                foreach (var attributeData in symbol.GetAttributes())
                {
                    var attClass = attributeData.AttributeClass;

                    if (attClass.ToDisplayString() == "TMPEffects.TestAttribute")
                    {
                        isDecorated = true;
                        break;
                    }
                }

                if (!isDecorated) continue;

                INamedTypeSymbol typeSymbol = symbol as INamedTypeSymbol;

                try
                {            
                    string code = @"
using System;
using UnityEngine;
using TMPEffects.SerializedCollections;

namespace TMPEffects.Databases
{{
    public partial interface ITMPKeywordDatabase
    {{
        public bool TryGetDouble(string str, out double result);
    }}


    public partial class TMPKeywordDatabase
    {{
        public bool TryGetDouble(string str, out double result)
        {{
            return doubleKeywords.TryGetValue(str, out result);
        }}

        [SerializeField, SerializedDictionary(keyName: ""Keyword"", valueName: ""Double""))]
        private SerializedDictionary<string, double> doubleKeywords;
    }}

    public partial class TMPSceneKeywordDatabase
    {{
        public bool TryGetDouble(string str, out double result)
        {{
            return doubleKeywords.TryGetValue(str, out result);
        }}

        [SerializeField, SerializedDictionary(keyName: ""Keyword"", valueName: ""Double""))]
        private SerializedDictionary<string, double> doubleKeywords;
    }}
}}";
                    
                    context.AddSource($"ITMPKeywordDatabase.double.g.cs", code);
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