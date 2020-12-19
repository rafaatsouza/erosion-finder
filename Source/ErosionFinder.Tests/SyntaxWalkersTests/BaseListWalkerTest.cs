using ErosionFinder.Dtos;
using ErosionFinder.Tests.Util;
using Xunit;

namespace ErosionFinder.SyntaxWalkers.Tests
{
    [Collection("MSBuildCollection")]
    public class BaseListWalkerTest
    {
        [Fact(DisplayName = "BaseListWalker VisitParameter - Success")]
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

            CommonAsserts.AssertSingleRelationAndSingleComponentByProgramText(
                programText, RelationType.Inheritance, "TestCompilation", "CodeComponent",
                (model, classDeclaration) => new BaseListWalker(model, classDeclaration, "TestCompilation"));
        }
    }
}
