using ErosionFinder.Dtos;
using ErosionFinder.SyntaxWalkers;
using ErosionFinder.Tests.Dtos;
using ErosionFinder.Tests.Fixture;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ErosionFinder.Tests
{
    [Collection("MSBuildCollection")]
    public class BaseListWalkerTest
    {
        [Fact]
        [Trait(nameof(BaseListWalker.VisitParameter), "Success: GetsInheritance")]
        public void BaseListWalker_VisitParameter_Success_GetsInheritance()
        {
            var programText = @"
            namespace TestCompilation
            {
                public class AnotherCodeComponent : CodeComponent
                { }

                public class CodeComponent
                { }
            }";

            var syntaxAnalysis = new SyntaxAnalysisTestComponent(programText);

            var root = syntaxAnalysis.Tree.GetCompilationUnitRoot();

            var classDeclarationNode = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>().First();
            
            var walker = new BaseListWalker(syntaxAnalysis.Model, 
                classDeclarationNode, "TestCompilation");

            var relations = walker.GetRelations(root);

            Assert.Single(relations);
            Assert.Equal(RelationType.Inheritance, 
                relations.Single().RelationType);
            Assert.Single(relations.Single().Components);
            Assert.Equal("CodeComponent", relations.Single().Components.Single());
        }
    }
}
