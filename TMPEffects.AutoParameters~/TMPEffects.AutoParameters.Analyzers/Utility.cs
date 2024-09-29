using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace TMPEffects.AutoParameters.Analyzers
{
    public static class Utility
    {
        public static bool IsSyntaxIDictionaryStringString(SyntaxNodeAnalysisContext context, ParameterSyntax param, INamedTypeSymbol stringType)
        {
            var type = context.SemanticModel.GetTypeInfo(param.Type).Type as INamedTypeSymbol;
            if (type == null) return false;
            if (!type.IsGenericType) return false;
            if (type.ConstructedFrom.ToDisplayString() != "System.Collections.Generic.IDictionary<TKey, TValue>") return false;
            if (type.TypeArguments.Length != 2) return false;
            if (!SymbolEqualityComparer.Default.Equals(stringType, type.TypeArguments[0])) return false;
            if (!SymbolEqualityComparer.Default.Equals(stringType, type.TypeArguments[1])) return false;

            return true;
        }

        public static bool IsSymbolIDictionaryStringString(IParameterSymbol param, INamedTypeSymbol stringType)
        {
            if (!(param.Type is INamedTypeSymbol type)) return false;
            if (!type.IsGenericType) return false;
            if (type.ConstructedFrom.ToDisplayString() != "System.Collections.Generic.IDictionary<TKey, TValue>") return false;
            if (type.TypeArguments.Length != 2) return false;
            if (!SymbolEqualityComparer.Default.Equals(stringType, type.TypeArguments[0])) return false;
            if (!SymbolEqualityComparer.Default.Equals(stringType, type.TypeArguments[1])) return false;

            return true;
        }
    }
}
