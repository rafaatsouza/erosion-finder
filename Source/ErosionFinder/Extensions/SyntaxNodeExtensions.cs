using Microsoft.CodeAnalysis;

namespace ErosionFinder.Extensions
{
    internal static class SyntaxNodeExtensions
    {
        public static bool FindTypeInParents<T>(
            this SyntaxNode node, out T element) where T : class
        {
            if (node is T castedElement)
            {
                element = castedElement;
                return true;
            }
            else if (node.Parent != null)
            {
                var found = FindTypeInParents<T>(node.Parent, 
                    out var foundElement);

                element = found ? foundElement : null;
                return found;
            }

            element = null;
            return false;
        }
    }
}