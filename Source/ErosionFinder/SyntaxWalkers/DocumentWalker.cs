using ErosionFinder.Dtos;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ErosionFinder.SyntaxWalkers
{
    internal class DocumentWalker : CSharpSyntaxWalker
    {
        public ICollection<Structure> Structures { get; set; }
        private ICollection<string> Usings { get; set; }
        private SemanticModel SemanticModel { get; set; }

        public DocumentWalker()
        {
            Usings = new List<string>();
            Structures = new List<Structure>();
        }

        public async Task VisitDocumentAsync(Document document, CancellationToken cancellationToken)
        {
            var rootNode = await document.GetSyntaxRootAsync(cancellationToken);
            
            SemanticModel = await document.GetSemanticModelAsync(cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                this.Visit(rootNode);
            }
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            Usings.Add(node.Name.ToString());
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            FillStructureCollection(node, GetNamespaceIdentifier(node));
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            FillStructureCollection(node, GetNamespaceIdentifier(node));
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            FillStructureCollection(node, GetNamespaceIdentifier(node));            
        }

        private void FillStructureCollection(MemberDeclarationSyntax member, string namespaceIdentifier)
        {
            var walker = new StructureWalker(SemanticModel);

            var structure = walker.GetStructure(member, namespaceIdentifier);

            structure.References = Usings;
            structure.Namespace = namespaceIdentifier;

            Structures.Add(structure);

            SearchInsideStructures<ClassDeclarationSyntax>(member, namespaceIdentifier);
            SearchInsideStructures<InterfaceDeclarationSyntax>(member, namespaceIdentifier);
            SearchInsideStructures<EnumDeclarationSyntax>(member, namespaceIdentifier);
        }

        private string GetNamespaceIdentifier(SyntaxNode node)
            => ((NamespaceDeclarationSyntax)node.Parent).Name.ToString();

        private void SearchInsideStructures<T>(MemberDeclarationSyntax memberDeclaration, string namespaceIdentifier)
        {
            var insideStructures = memberDeclaration.DescendantNodes().OfType<T>();

            if (insideStructures.Any())
            {
                foreach (var insideStructure in insideStructures)
                {
                    if (insideStructure is MemberDeclarationSyntax insideMember)
                    {
                        FillStructureCollection(insideMember, namespaceIdentifier);
                    }
                }
            }
        }
    }
}