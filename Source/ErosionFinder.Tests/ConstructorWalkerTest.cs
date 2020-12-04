using ErosionFinder.Dtos;
using ErosionFinder.SyntaxWalkers;
using ErosionFinder.Tests.Dtos;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ErosionFinder.Tests
{
    public class ConstructorWalkerTest
    {
        public ConstructorWalkerTest()
        {
            if (!MSBuildLocator.IsRegistered)
            {
                MSBuildLocator.RegisterDefaults();
            }

            Assert.True(MSBuildLocator.IsRegistered);
        }

        [Fact]
        [Trait(nameof(ConstructorWalker.VisitParameter), "Success: GetsReceiptByConstructorArgument")]
        public void ConstructorWalker_VisitParameter_Success_GetsReceiptByConstructorArgument()
        {
            var programText = @"
            namespace HelloWorld
            {
                public class Program
                {
                    private readonly CodeComponent component;

                    public Program(CodeComponent component)
                    {
                        this.component = component;
                    }
                }

                public class CodeComponent
                {
                    public int parameter { get; set; }
                }
            }";

            var relations = GetRelationsByProgramText(programText);

            Assert.Single(relations);
            Assert.Equal(RelationType.ReceiptByConstructorArgument, 
                relations.Single().RelationType);
            Assert.Single(relations.Single().Components);
            Assert.Equal("CodeComponent", relations.Single().Components.Single());
        }

        [Fact]
        [Trait(nameof(ConstructorWalker.VisitParameter), "Success: EmptyRelations")]
        public void ConstructorWalker_VisitParameter_Success_EmptyRelations()
        {
            var programText = @"
            namespace HelloWorld
            {
                public class Component
                {
                    public int Parameter { get; set; }

                    public Component(Component component)
                    {
                        this.Parameter = component.Parameter;
                    }
                }
            }";

            var relations = GetRelationsByProgramText(programText);

            Assert.Empty(relations);
        }

        private ICollection<Relation> GetRelationsByProgramText(string programText)
        {
            var syntaxAnalysis = new SyntaxAnalysisTestComponent(programText);

            var root = syntaxAnalysis.Tree.GetCompilationUnitRoot();

            var classDeclarationNode = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>().First();
            
            var walker = new ConstructorWalker(syntaxAnalysis.Model, 
                classDeclarationNode, "HelloWorld");

            return walker.GetRelations(root);
        }
    }
}
