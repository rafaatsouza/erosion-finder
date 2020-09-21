using ErosionFinder.Dtos;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ErosionFinder.SyntaxWalkers
{
    internal class BaseListWalker : RelationsRetrieverWalker
    {
        public BaseListWalker(SemanticModel semanticModel, 
            SyntaxNode baseNode, string baseMemberNamespace) 
            : base(semanticModel, baseNode, baseMemberNamespace) { }

        public override void VisitSimpleBaseType(
            SimpleBaseTypeSyntax node)
        {
            if (ItsFromSameMember(node))
            {
                IncrementsRelationsFromExpressionAndCheckGenerics(
                    node.Type, RelationType.Inheritance);
            }
        }
    }
}