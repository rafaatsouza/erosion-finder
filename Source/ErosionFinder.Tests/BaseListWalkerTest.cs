using ErosionFinder.Data.Models;
using ErosionFinder.Dtos;
using ErosionFinder.SyntaxWalkers;
using ErosionFinder.Tests.Fixture;
using ErosionFinder.Tests.Util;
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

            CommonAssert.AssertSingleRelationAndSingleComponentByProgramText(
                programText, RelationType.Inheritance, "TestCompilation", "CodeComponent",
                (model, classDeclaration) => new BaseListWalker(model, classDeclaration, "TestCompilation"));
        }
    }
}
