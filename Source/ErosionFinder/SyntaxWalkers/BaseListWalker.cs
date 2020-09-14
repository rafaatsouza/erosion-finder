using ErosionFinder.Dtos;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace ErosionFinder.SyntaxWalkers
{
    internal class BaseListWalker : RelationsRetrieverWalker
    {
        public BaseListWalker(ILoggerFactory loggerFactory, SemanticModel semanticModel, 
            SyntaxNode baseNode, string baseMemberNamespace) 
            : base(loggerFactory, semanticModel, baseNode, baseMemberNamespace) { }

        public override void VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
            if (ItsFromSameMember(node))
            {
                IncrementsRelationsFromExpressionAndCheckGenerics(node.Type, RelationType.Inheritance);
            }
        }
    }
}