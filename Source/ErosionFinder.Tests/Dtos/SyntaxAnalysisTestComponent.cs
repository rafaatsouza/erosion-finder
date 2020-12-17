using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ErosionFinder.Tests.Dtos
{
    internal class SyntaxAnalysisTestComponent
    {
        public SyntaxTree Tree { get; }

        public SemanticModel Model { get; }

        public SyntaxAnalysisTestComponent(string programText)
        {
            Tree = CSharpSyntaxTree.ParseText(programText);

            var compilation = CSharpCompilation.Create("TestCompilation")
                .AddReferences(
                    MetadataReference.CreateFromFile(
                    typeof(object).Assembly.Location))
                .AddSyntaxTrees(Tree);

            Model = compilation.GetSemanticModel(Tree);
        }
    }
}