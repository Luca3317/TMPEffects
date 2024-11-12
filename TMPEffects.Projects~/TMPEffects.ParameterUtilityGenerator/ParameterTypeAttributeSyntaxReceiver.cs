using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TMPEffects.AutoParameters.Generator
{
    public class ParameterTypeAttributeSyntaxReceiver : ISyntaxReceiver
    {
        public List<AttributeSyntax> AttributeSyntaxes { get; private set; }

        public readonly string attributeName;

        public string lastfail;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            TypeDeclarationSyntax typeDecl;
            if ((typeDecl = syntaxNode as TypeDeclarationSyntax) == null)
                return;

            if (typeDecl.AttributeLists.Count == 0)
                return;

            AttributeSyntaxes.AddRange(
                typeDecl.AttributeLists.SelectMany(list => list.Attributes)
                    .Where(attr => attr.Name.ToFullString() == attributeName)
            );
        }

        public ParameterTypeAttributeSyntaxReceiver(string attributeName)
        {
            AttributeSyntaxes = new List<AttributeSyntax>();
            this.attributeName = attributeName;
        }
    }
}