using ErosionFinder.Data.Models;
using ErosionFinder.Dtos;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ErosionFinder.SyntaxWalkers
{
    internal class ConstructorWalker : RelationsRetrieverWalker
    {
        public ConstructorWalker(SemanticModel semanticModel,
            SyntaxNode baseNode, string baseMemberNamespace) 
            : base(semanticModel, baseNode, baseMemberNamespace) { }

        public override void VisitParameter(ParameterSyntax node)
        {
            if (ItsFromSameMember(node)
                && !(node.Parent is LambdaExpressionSyntax))
            {
                IncrementsRelationsFromExpressionAndCheckGenerics(node.Type, 
                    RelationType.ReceiptByConstructorArgument);
            }
        }
    }
}