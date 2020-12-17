using ErosionFinder.Data.Models;
using ErosionFinder.Dtos;
using ErosionFinder.SyntaxWalkers;
using ErosionFinder.Tests.Fixture;
using ErosionFinder.Tests.Util;
using Xunit;

namespace ErosionFinder.SyntaxWalkers.Tests
{
    [Collection("MSBuildCollection")]
    public class ConstructorWalkerTest
    {
        [Fact]
        [Trait(nameof(ConstructorWalker.VisitParameter), "Success: GetsReceiptByConstructorArgument")]
        public void ConstructorWalker_VisitParameter_Success_GetsReceiptByConstructorArgument()
        {
            var programText = @"
            namespace TestCompilation
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

            CommonAsserts.AssertSingleRelationAndSingleComponentByProgramText(
                programText, RelationType.ReceiptByConstructorArgument, "TestCompilation", "CodeComponent",
                (model, classDeclaration) => new ConstructorWalker(model, classDeclaration, "TestCompilation"));
        }

        [Fact]
        [Trait(nameof(ConstructorWalker.VisitParameter), "Success: EmptyRelations")]
        public void ConstructorWalker_VisitParameter_Success_EmptyRelations()
        {
            var programText = @"
            namespace TestCompilation
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

            CommonAsserts.AssertEmptyRelationByProgramText(
                programText, 
                (model, classDeclaration) => new ConstructorWalker(model, classDeclaration, "TestCompilation"));
        }
    }
}
