using ErosionFinder.Dtos;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace ErosionFinder.SyntaxWalkers
{
    internal class ConstructorWalker : RelationsRetrieverWalker
    {
        public ConstructorWalker(ILoggerFactory loggerFactory, SemanticModel semanticModel,
            SyntaxNode baseNode, string baseMemberNamespace) 
            : base(loggerFactory, semanticModel, baseNode, baseMemberNamespace) { }

        public override void VisitParameter(ParameterSyntax node)
        {
            if (ItsFromSameMember(node)
                && !(node.Parent is LambdaExpressionSyntax))
            {
                IncrementsRelationsFromExpressionAndCheckGenerics(node.Type, RelationType.ReceiptByConstructorArgument);
            }
        }
    }
}