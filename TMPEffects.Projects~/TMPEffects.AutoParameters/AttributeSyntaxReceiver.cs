using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TMPEffects.AutoParameters.Generator
{
    public class AttributeSyntaxReceiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> TypeDeclarations { get; private set; }

        public readonly string attributeName;

        public string lastfail;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            TypeDeclarationSyntax typeDecl;
            if ((typeDecl = syntaxNode as TypeDeclarationSyntax) == null || syntaxNode is InterfaceDeclarationSyntax)
                return;

            if (typeDecl.AttributeLists.Count == 0)
                return;

            if (typeDecl.AttributeLists.Any(a => a.Attributes.Any(b => attributeName == b.Name.ToFullString())))
            {
                TypeDeclarations.Add(typeDecl);
            }
        }

        public AttributeSyntaxReceiver(string attributeName)
        {
            TypeDeclarations = new List<TypeDeclarationSyntax>();
            this.attributeName = attributeName;
        }
    }
}