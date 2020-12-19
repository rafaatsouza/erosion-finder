using ErosionFinder.Data.Models;
using ErosionFinder.Tests.Util;
using Xunit;

namespace ErosionFinder.SyntaxWalkers.Tests
{
    [Collection("MSBuildCollection")]
    public class CommonWalkerTest
    {
        [Fact(DisplayName = "CommonWalker VisitParameter - Success - Gets ReturnByFunction relation type")]
        [Trait(nameof(CommonWalker.VisitParameter), "Success: GetsReturnByFunction")]
        public void CommonWalker_VisitParameter_Success_GetsReturnByFunction()
        {
            var programText = @"
            namespace TestCompilation
            {
                public abstract class Program
                {
                    public virtual CodeComponent GetCodeComponent(int parameter);
                }

                public class CodeComponent
                {
                    public int parameter { get; set; }
                }
            }";

            CommonAsserts.AssertSingleRelationAndSingleComponentByProgramText(
                programText, RelationType.ReturnByFunction, "TestCompilation", "CodeComponent",
                (model, classDeclaration) => new CommonWalker(model, classDeclaration, "TestCompilation"));
        }

        [Fact(DisplayName = "CommonWalker VisitParameter - Success - Gets ReceiptByMethodArgument relation type")]
        [Trait(nameof(CommonWalker.VisitParameter), "Success: GetsReceiptByMethodArgument")]
        public void CommonWalker_VisitParameter_Success_GetsReceiptByMethodArgument()
        {
            var programText = @"
            namespace TestCompilation
            {
                public class Program
                {
                    public void Test(CodeComponent component)
                    { }
                }

                public class CodeComponent
                {
                    public int parameter { get; set; }
                }
            }";

            CommonAsserts.AssertSingleRelationAndSingleComponentByProgramText(
                programText, RelationType.ReceiptByMethodArgument, "TestCompilation", "CodeComponent",
                (model, classDeclaration) => new CommonWalker(model, classDeclaration, "TestCompilation"));
        }

        [Fact(DisplayName = "CommonWalker VisitParameter - Success - Gets Declarate relation type")]
        [Trait(nameof(CommonWalker.VisitParameter), "Success: GetsDeclarate")]
        public void CommonWalker_VisitParameter_Success_GetsDeclarate()
        {
            var programText = @"
            namespace TestCompilation
            {
                public class Program
                {
                    public void Test()
                    { 
                        CodeComponent component;
                    }
                }

                public class CodeComponent
                {
                    public int parameter { get; set; }
                }
            }";

            CommonAsserts.AssertSingleRelationAndSingleComponentByProgramText(
                programText, RelationType.Declarate, "TestCompilation", "CodeComponent",
                (model, classDeclaration) => new CommonWalker(model, classDeclaration, "TestCompilation"));
        }

        [Fact(DisplayName = "CommonWalker VisitParameter - Success - Gets Throw relation type")]
        [Trait(nameof(CommonWalker.VisitParameter), "Success: GetsThrow")]
        public void CommonWalker_VisitParameter_Success_GetsThrow()
        {
            var programText = @"
            namespace TestCompilation
            {
                public class Program
                {
                    public void Test()
                    { 
                        throw new CustomException();
                    }
                }

                public class CustomException : System.Exception
                { }
            }";

            CommonAsserts.AssertSingleRelationAndSingleComponentByProgramText(
                programText, RelationType.Throw, "TestCompilation", "CustomException",
                (model, classDeclaration) => new CommonWalker(model, classDeclaration, "TestCompilation"));
        }

        [Fact(DisplayName = "CommonWalker VisitParameter - Success - Gets Instantiate relation type")]
        [Trait(nameof(CommonWalker.VisitParameter), "Success: GetsInstantiate")]
        public void CommonWalker_VisitParameter_Success_GetsInstantiate()
        {
            var programText = @"
            namespace TestCompilation
            {
                public class Program
                {
                    public CodeComponent Component => new CodeComponent();
                }

                public class CodeComponent
                { 
                    public int Parameter { get; set; }
                }
            }";

            CommonAsserts.AssertSingleRelationAndSingleComponentByProgramText(
                programText, RelationType.Instantiate, "TestCompilation", "CodeComponent",
                (model, classDeclaration) => new CommonWalker(model, classDeclaration, "TestCompilation"));
        }

        [Fact(DisplayName = "CommonWalker VisitParameter - Success - Gets Invocate relation type")]
        [Trait(nameof(CommonWalker.VisitParameter), "Success: GetsInvocate")]
        public void CommonWalker_VisitParameter_Success_GetsInvocate()
        {
            var programText = @"
            namespace TestCompilation
            {
                public static class Program
                {
                    public static void test()
                    {
                        var c = CodeComponent.GetSum(1, 2);
                    }
                }

                public static class CodeComponent
                { 
                    public static int GetSum(int a, int b)
                    {
                        return a + b;
                    }
                }
            }";

            CommonAsserts.AssertSingleRelationAndSingleComponentByProgramText(
                programText, RelationType.Invocate, "TestCompilation", "CodeComponent",
                (model, classDeclaration) => new CommonWalker(model, classDeclaration, "TestCompilation"));
        }
    
        [Fact(DisplayName = "CommonWalker VisitParameter - Success - Empty relations")]
        [Trait(nameof(CommonWalker.VisitParameter), "Success: EmptyRelations")]
        public void CommonWalker_VisitParameter_Success_EmptyRelations()
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
                (model, classDeclaration) => new CommonWalker(model, classDeclaration, "TestCompilation"));
        }
    }
}