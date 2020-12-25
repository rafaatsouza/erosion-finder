using ErosionFinder.Data.Exceptions;
using ErosionFinder.Data.Models;
using ErosionFinder.Dtos;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.Packaging;
using System;
using System.Collections.Generic;

namespace ErosionFinder.SyntaxWalkers
{
    internal class StructureWalker : CSharpSyntaxWalker
    {
        private readonly SemanticModel semanticModel;
        private readonly ICollection<Relation> memberRelations;

        private MemberDeclarationSyntax BaseMemberDeclaration { get; set; }
        private string BaseMemberNamespace { get; set; }

        public StructureWalker(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel ??
                throw new ArgumentNullException(nameof(semanticModel));

            memberRelations = new List<Relation>();
        }

        public Structure GetStructure(
            MemberDeclarationSyntax member, string baseMemberNamespace)
        {
            BaseMemberDeclaration = member;
            BaseMemberNamespace = baseMemberNamespace;

            var commonWalker = new CommonWalker(semanticModel, 
                member, baseMemberNamespace);

            memberRelations.AddRange(commonWalker.GetRelations(member));

            Visit(member);

            return new Structure()
            {
                Name = ((BaseTypeDeclarationSyntax)member).Identifier.ValueText,
                Type = GetStructureType(member),
                Relations = memberRelations
            };
        }

        public override void VisitConstructorDeclaration(
            ConstructorDeclarationSyntax node)
        {
            IncreaseRelations(() => new ConstructorWalker(semanticModel, 
                BaseMemberDeclaration, BaseMemberNamespace), node);
        }

        public override void VisitBaseList(BaseListSyntax node)
        {
            IncreaseRelations(() => new BaseListWalker(semanticModel, 
                BaseMemberDeclaration, BaseMemberNamespace), node);
        }

        private StructureType GetStructureType(
            MemberDeclarationSyntax member)
        {
            if (member is ClassDeclarationSyntax)
            {
                return StructureType.Class;
            }
            else if (member is InterfaceDeclarationSyntax)
            {
                return StructureType.Interface;
            }
            else if (member is EnumDeclarationSyntax)
            {
                return StructureType.Enum;
            }

            var namespaceIdentifier = ((BaseTypeDeclarationSyntax)member).Identifier.ValueText;

            throw new CodeAnalysisException(
                CodeAnalysisError.StructureTypeNotFound(namespaceIdentifier));
        }

        private void IncreaseRelations(Func<RelationsRetrieverWalker> getWalker, SyntaxNode node)
        {
            var walker = getWalker.Invoke();

            memberRelations.AddRange(walker.GetRelations(node));
        }
    }
}