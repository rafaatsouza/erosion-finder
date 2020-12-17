using ErosionFinder.Dtos;
using ErosionFinder.SyntaxWalkers;
using ErosionFinder.Tests.Fixture;
using ErosionFinder.Tests.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace ErosionFinder.SyntaxWalkers.Tests
{
    [Collection("MSBuildCollection")]
    public class StructureWalkerTest
    {
        [Fact]
        [Trait(nameof(StructureWalker.GetStructure), "Success: GetsClass")]
        public void StructureWalker_GetStructure_Success_GetsClass()
        {
            var programText = @"
            namespace TestCompilation
            {
                public class Program
                { }
            }";

            CommonAssert.AssertGetStructureOfType<ClassDeclarationSyntax>(
                programText, StructureType.Class, "TestCompilation", "Program");
        }

        [Fact]
        [Trait(nameof(StructureWalker.GetStructure), "Success: GetsInterface")]
        public void StructureWalker_GetStructure_Success_GetsInterface()
        {
            var programText = @"
            namespace TestCompilation
            {
                public interface IProgram
                { }
            }";

            CommonAssert.AssertGetStructureOfType<InterfaceDeclarationSyntax>(
                programText, StructureType.Interface, "TestCompilation", "IProgram");
        }

        [Fact]
        [Trait(nameof(StructureWalker.GetStructure), "Success: GetsEnum")]
        public void StructureWalker_GetStructure_Success_GetsEnum()
        {
            var programText = @"
            namespace TestCompilation
            {
                public enum Program
                { }
            }";

            CommonAssert.AssertGetStructureOfType<EnumDeclarationSyntax>(
                programText, StructureType.Enum, "TestCompilation", "Program");
        }
    }
}