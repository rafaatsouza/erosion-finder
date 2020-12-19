using ErosionFinder.Dtos;
using ErosionFinder.Tests.Util;
using Xunit;

namespace ErosionFinder.SyntaxWalkers.Tests
{
    [Collection("MSBuildCollection")]
    public class ConstructorWalkerTest
    {
        [Fact(DisplayName = "ConstructorWalker VisitParameter - Success - Gets ReceiptByConstructorArgument relation type")]
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
    }
}
