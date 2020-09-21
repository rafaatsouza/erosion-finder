using ErosionFinder.Dtos;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ErosionFinder.SyntaxWalkers
{
    internal class CommonWalker : RelationsRetrieverWalker
    {
        public CommonWalker(SemanticModel semanticModel,
            SyntaxNode baseNode, string baseMemberNamespace) 
            : base(semanticModel, baseNode, baseMemberNamespace) { }

        public override void VisitObjectCreationExpression(
            ObjectCreationExpressionSyntax node)
        {
            if (ItsFromSameMember(node))
            {
                if (FindTypeInParents<ThrowExpressionSyntax>(node, out _)
                    || FindTypeInParents<ThrowStatementSyntax>(node, out _))
                {
                    IncrementsRelationsFromExpressionAndCheckGenerics(
                        node.Type, RelationType.Throw);
                }
                else
                {
                    IncrementsRelationsFromExpressionAndCheckGenerics(
                        node.Type, RelationType.Instantiate);
                }

                base.VisitObjectCreationExpression(node);
            }
        }

        public override void VisitMethodDeclaration(
            MethodDeclarationSyntax node)
        {
            if (node.ReturnType != null)
            {
                IncrementsRelationsFromExpressionAndCheckGenerics(
                    node.ReturnType, RelationType.ReturnByFunction);
            }

            if (node.ParameterList != null
                && node.ParameterList.Parameters != null
                && node.ParameterList.Parameters.Any())
            {
                foreach (var parameter in node.ParameterList.Parameters)
                {
                    IncrementsRelationsFromExpressionAndCheckGenerics(
                        parameter.Type, RelationType.ReceiptByMethodArgument);
                }
            }

            base.VisitMethodDeclaration(node);
        }

        public override void VisitVariableDeclaration(
            VariableDeclarationSyntax node)
        {
            if (node.Type != null)
            {
                IncrementsRelationsFromExpressionAndCheckGenerics(
                    node.Type, RelationType.Declarate);
            }

            base.VisitVariableDeclaration(node);
        }

        public override void VisitInvocationExpression(
            InvocationExpressionSyntax node)
        {
            var expression = node.Expression;

            if (expression is MemberBindingExpressionSyntax memberBindingExpression)
            {
                if (memberBindingExpression.Name is IdentifierNameSyntax memberBindingIdentifierName)
                {
                    IncrementsRelationsFromExpressionAndCheckGenerics(
                        memberBindingIdentifierName, RelationType.Invocate);
                }
                else
                {
                    IncrementsRelationsFromExpressionAndCheckGenerics(
                        memberBindingExpression, RelationType.Invocate);
                }
            }
            else
            {
                IncrementsRelationsFromExpressionAndCheckGenerics(
                    expression, RelationType.Invocate);
            }

            base.VisitInvocationExpression(node);
        }
    }
}