using ErosionFinder.Dtos;
using ErosionFinder.SyntaxWalkers;
using ErosionFinder.Tests.Dtos;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ErosionFinder.Tests.Util
{
    internal static class CommonAssert
    {
        public static void AssertSingleRelationAndSingleComponentByProgramText(
            string programText, RelationType relationType, string target, string component,
            Func<SemanticModel, ClassDeclarationSyntax, RelationsRetrieverWalker> getWalker)
        {
            var relations = GetRelations(programText, getWalker);

            Assert.Single(relations);

            var relation = relations.Single();

            Assert.Equal(relationType, relation.RelationType);
            Assert.Equal(target, relation.Target);
            Assert.Single(relation.Components);
            Assert.Equal(component, relation.Components.Single());
        }

        public static void AssertEmptyRelationByProgramText(
            string programText, 
            Func<SemanticModel, ClassDeclarationSyntax, RelationsRetrieverWalker> getWalker)
        {
            var relations = GetRelations(programText, getWalker);

            Assert.Empty(relations);
        }

        private static ICollection<Relation> GetRelations(string programText,
            Func<SemanticModel, ClassDeclarationSyntax, RelationsRetrieverWalker> getWalker)
        {
            var syntaxAnalysis = new SyntaxAnalysisTestComponent(programText);

            var root = syntaxAnalysis.Tree.GetCompilationUnitRoot();

            var classDeclarationNode = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>().First();
            
            var walker = getWalker(syntaxAnalysis.Model, 
                classDeclarationNode);

            return walker.GetRelations(root);
        }
    }
}