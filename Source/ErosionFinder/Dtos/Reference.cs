using ErosionFinder.Domain.Exceptions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace ErosionFinder.Dtos
{
    internal class Reference
    {
        public string Name { get; }
        public string Namespace { get; }
        
        private bool Reserved { get; set; }        
        private bool ExplicitlyDynamic { get; set; }
        private bool Anonymous { get; set; }

        private ISymbol Symbol { get; }

        private const string ReservedIdentifiers = "nameof"; //separated by comma

        public Reference(SemanticModel semanticModel, ExpressionSyntax expressionSyntax)
        {
            Symbol = GetSymbol(semanticModel, expressionSyntax);

            if (Symbol == null && !Reserved && !ExplicitlyDynamic && !Anonymous)
                throw new AnalysisCustomException(
                    AnalysisCustomError.TypeNotResolved(expressionSyntax.ToString()));
            else if (Reserved || ExplicitlyDynamic || Anonymous)
                return;

            Name = Symbol.Name;
            Namespace = Symbol.ContainingNamespace.ToString();

            if (Symbol.Kind == SymbolKind.Field
                || Symbol.Kind == SymbolKind.Local
                || Symbol.Kind == SymbolKind.Property
                || Symbol.Kind == SymbolKind.Parameter)
            {
                var typeInfo = semanticModel.GetTypeInfo(expressionSyntax);

                if (typeInfo.ConvertedType == null)
                    throw new AnalysisCustomException(
                        AnalysisCustomError.TypeNotResolved(expressionSyntax.ToString()));

                Symbol = typeInfo.ConvertedType.ContainingSymbol;

                if (Symbol is IArrayTypeSymbol arrayTypeSymbol)
                    Symbol = arrayTypeSymbol.ElementType;

                Name = typeInfo.Type.Name;
                Namespace = typeInfo.ConvertedType.ContainingNamespace.ToString();
            }
            else if (Symbol.Kind == SymbolKind.Method)
            {
                var refSymbol = Symbol;

                while (refSymbol.Kind == SymbolKind.Method && refSymbol.ContainingSymbol != null)
                {
                    refSymbol = refSymbol.ContainingSymbol;
                }

                Name = refSymbol.Name;
            }
        }

        /// <summary>
        /// Checks if reference it's some dynamic type, or anonymous type,
        /// or derives from 'System' namespace or it's some reserved keyword
        /// </summary>
        /// <returns>True/False indicating if is dynamic, anonymous or from 'System'</returns>
        public bool IsBasicDynamicAnonymousOrReserved()
        {
            if (Reserved || Anonymous || ExplicitlyDynamic 
                || Symbol.Kind == SymbolKind.DynamicType 
                || Symbol.Kind == SymbolKind.TypeParameter)
                return true;

            return Namespace.StartsWith("System", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Checks if reference its defined in source code
        /// </summary>
        /// <returns>True/False indicating if is defined in source code</returns>
        public bool IsFromSource()
            => Symbol.Locations
                    .Any(l => l.Kind == LocationKind.SourceFile
                        || l.Kind == LocationKind.XmlFile);

        /// <summary>
        /// Gets symbol from some node
        /// </summary>
        /// <param name="semanticModel">Semantic model</param>
        /// <param name="node">Syntax node</param>
        /// <returns>Symbol from node</returns>
        private ISymbol GetSymbol(SemanticModel semanticModel, SyntaxNode node)
        {
            var typeInfo = semanticModel.GetTypeInfo(node);
            var operation = semanticModel.GetOperation(node);

            ExplicitlyDynamic = (typeInfo.Type != null && typeInfo.ConvertedType.TypeKind == TypeKind.Dynamic)
                || (operation != null && operation.Kind == OperationKind.DynamicMemberReference);

            Anonymous = typeInfo.ConvertedType != null && typeInfo.ConvertedType.IsAnonymousType;

            var symbolInfo = semanticModel.GetSymbolInfo(node);
            
            if (symbolInfo.Symbol == null)
            {
                if (node is IdentifierNameSyntax identifier)
                    Reserved = IdentifierIsReserved(symbolInfo, identifier);

                return null;
            }

            if (symbolInfo.Symbol is IArrayTypeSymbol arrayTypeSymbol)
                return arrayTypeSymbol.ElementType;

            return symbolInfo.Symbol;
        }

        /// <summary>
        /// Checks if some identifier actually is some reserved word
        /// </summary>
        /// <param name="symbolInfo">Identifier's symbol info</param>
        /// <param name="identifier">Identifier</param>
        /// <returns>True/False indicating if identifier is reserved word</returns>
        private bool IdentifierIsReserved(SymbolInfo symbolInfo, IdentifierNameSyntax identifier)
        {
            if (symbolInfo.Symbol == null
                && symbolInfo.CandidateSymbols.IsEmpty
                && symbolInfo.CandidateReason == CandidateReason.None)
            {
                return identifier != null
                    && identifier.Identifier.Kind() == SyntaxKind.IdentifierToken
                    && ReservedIdentifiers.Split(',').Any(r => string.Equals(r, identifier.Identifier.Text));
            }

            return false;
        }
    }
}