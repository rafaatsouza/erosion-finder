using ErosionFinder.Domain.Exceptions.Custom;
using ErosionFinder.Domain.Models;
using ErosionFinder.Dtos;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ErosionFinder.SyntaxWalkers
{
    internal abstract class RelationsRetrieverWalker : CSharpSyntaxWalker
    {
        protected readonly SemanticModel semanticModel;
        protected readonly ICollection<Relation> memberRelations;

        private readonly ILogger logger;
        private readonly string baseMemberName;
        private readonly string baseMemberNamespace;

        public RelationsRetrieverWalker(ILoggerFactory loggerFactory, 
            SemanticModel semanticModel, SyntaxNode baseNode, string baseMemberNamespace)
        {
            this.logger = loggerFactory?.CreateLogger<RelationsRetrieverWalker>()
                ?? throw new ArgumentNullException(nameof(RelationsRetrieverWalker));

            this.semanticModel = semanticModel
                ?? throw new ArgumentNullException(nameof(semanticModel));

            this.baseMemberName = ((BaseTypeDeclarationSyntax)baseNode).Identifier.ValueText;

            if (string.IsNullOrEmpty(this.baseMemberName))
            {
                throw new ArgumentNullException(nameof(baseNode));
            }

            this.baseMemberNamespace = !string.IsNullOrEmpty(baseMemberNamespace)
                ? baseMemberNamespace : throw new ArgumentNullException(nameof(baseMemberNamespace));

            memberRelations = new List<Relation>();
        }

        public ICollection<Relation> GetRelations(SyntaxNode node)
        {
            Visit(node);

            return memberRelations;
        }

        protected bool FindTypeInParents<T>(SyntaxNode node, out T element) where T : class
        {
            if (node is T castedElement)
            {
                element = castedElement;
                return true;
            }
            else if (node.Parent != null)
            {
                var found = FindTypeInParents<T>(node.Parent, out var foundElement);

                element = found ? foundElement : null;

                return found;
            }

            element = null;
            return false;
        }

        protected bool ItsFromSameMember(SyntaxNode node)
        {
            var findClass = FindTypeInParents<ClassDeclarationSyntax>(node, out var classDeclaration);
            var findInterface = FindTypeInParents<InterfaceDeclarationSyntax>(node, out var interfaceDeclaration);

            return (findClass && classDeclaration.Identifier.ValueText == baseMemberName)
                || (findInterface && interfaceDeclaration.Identifier.ValueText == baseMemberName);
        }

        protected void IncrementsRelationsFromExpressionAndCheckGenerics(ExpressionSyntax expression, RelationType relationType)
        {
            IncrementsRelationsFromExpression(expression, relationType);

            if (expression is GenericNameSyntax generic
                && generic.TypeArgumentList != null
                && generic.TypeArgumentList.Arguments != null)
            {
                foreach (var argument in generic.TypeArgumentList.Arguments)
                {
                    GetGenericArgumentRelationsFromExpression(argument);
                }
            }
        }

        private void IncrementsRelationsFromExpression(ExpressionSyntax expressionSyntax, RelationType relationType)
        {
            Reference reference;

            try
            {
                reference = new Reference(semanticModel, expressionSyntax);
            }
            catch (CustomException ex)
            {
                var span = expressionSyntax.SyntaxTree.GetLineSpan(expressionSyntax.Span);
                var lineNumber = span.StartLinePosition.Line + 1;

                logger.LogError("Error at structure {Structure}, line {Line} - {Message};", 
                    baseMemberName, lineNumber, ex.Message);

                return;
            }

            if (reference.IsBasicDynamicAnonymousOrReserved())
            {
                return;
            }

            if (string.Equals(reference.Namespace, baseMemberNamespace, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(reference.Name, baseMemberName, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var relationExists = memberRelations
                .Any(r => r.Target.Equals(reference.Namespace)
                    && r.RelationType == relationType);

            if (!relationExists)
            {
                memberRelations.Add(new Relation(relationType, reference.Namespace, reference.IsFromSource()));
            }

            var componentsExists = memberRelations
                .FirstOrDefault(r => r.Target.Equals(reference.Namespace)
                    && r.RelationType == relationType)
                .Components.Any(c => c.Equals(reference.Name));

            if (!componentsExists)
            {
                memberRelations
                    .FirstOrDefault(r => r.Target.Equals(reference.Namespace)
                        && r.RelationType == relationType)
                    .Components.Add(reference.Name);
            }
        }

        private void GetGenericArgumentRelationsFromExpression(ExpressionSyntax expression)
        {
            IncrementsRelationsFromExpression(expression, RelationType.Indirect);

            if (expression is GenericNameSyntax generic
                && generic.TypeArgumentList != null
                && generic.TypeArgumentList.Arguments != null)
            {
                foreach (var argument in generic.TypeArgumentList.Arguments)
                {
                    GetGenericArgumentRelationsFromExpression(argument);
                }
            }
        }
    }
}