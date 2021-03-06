﻿using ErosionFinder.Data.Models;
using ErosionFinder.Data.Exceptions.Base;
using ErosionFinder.Dtos;
using ErosionFinder.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace ErosionFinder.SyntaxWalkers
{
    internal abstract class RelationsRetrieverWalker : CSharpSyntaxWalker
    {
        protected readonly SemanticModel semanticModel;
        protected readonly ICollection<Relation> memberRelations;

        private readonly string baseMemberName;
        private readonly string baseMemberNamespace;

        public RelationsRetrieverWalker(SemanticModel semanticModel, 
            SyntaxNode baseNode, string baseMemberNamespace)
        {
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

        protected bool ItsFromSameMember(SyntaxNode node)
        {
            var foundClass = node.TryFindTypeInParents<ClassDeclarationSyntax>(
                out var classDeclaration);
            var foundInterface = node.TryFindTypeInParents<InterfaceDeclarationSyntax>(
                out var interfaceDeclaration);

            return (foundClass && classDeclaration.Identifier.ValueText == baseMemberName)
                || (foundInterface && interfaceDeclaration.Identifier.ValueText == baseMemberName);
        }

        protected void IncrementsRelationsFromExpressionAndCheckGenerics(
            ExpressionSyntax expression, RelationType relationType)
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

        private void IncrementsRelationsFromExpression(
            ExpressionSyntax expressionSyntax, RelationType relationType)
        {
            Reference reference;

            try
            {
                reference = new Reference(semanticModel, expressionSyntax);
            }
            catch (ErosionFinderException ex)
            {
                var span = expressionSyntax.SyntaxTree.GetLineSpan(expressionSyntax.Span);
                var lineNumber = span.StartLinePosition.Line + 1;

                Trace.Fail(string.Format("Error at structure {0}, line {1} - {2};", 
                    baseMemberName, lineNumber, ex.Message));

                return;
            }

            if (reference.IsBasicDynamicAnonymousOrReserved())
            {
                return;
            }

            var sameNamespace = string.Equals(reference.Namespace, 
                baseMemberNamespace, StringComparison.InvariantCultureIgnoreCase);
            var sameName = string.Equals(reference.Name, 
                baseMemberName, StringComparison.InvariantCultureIgnoreCase);

            if (sameNamespace && sameName)
            {
                return;
            }

            var relationExists = memberRelations
                .Any(r => r.Target.Equals(reference.Namespace)
                    && r.RelationType == relationType);

            if (!relationExists)
            {
                memberRelations.Add(new Relation(relationType, 
                    reference.Namespace, reference.IsFromSource()));
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

        private void GetGenericArgumentRelationsFromExpression(
            ExpressionSyntax expression)
        {
            IncrementsRelationsFromExpression(
                expression, RelationType.Indirect);

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