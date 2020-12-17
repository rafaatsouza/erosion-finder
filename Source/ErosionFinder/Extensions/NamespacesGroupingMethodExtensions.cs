using ErosionFinder.Data.Models;
using ErosionFinder.Data.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace ErosionFinder.Extensions
{
    public static class NamespacesGroupingMethodExtensions
    {
        public static void CheckIfItsValid(
            this NamespacesExplicitlyGrouped explicitlyGrouped, string key)
        {
            if (explicitlyGrouped == null)
                return;

            if (explicitlyGrouped.Namespaces == null 
                || !explicitlyGrouped.Namespaces.Any())
                throw new ConstraintsException(
                    ConstraintsError.NamespaceNotFoundForLayer(key));
        }

        public static void CheckIfItsValid(
            this NamespacesRegularExpressionGrouped regexGrouped, 
            string key, IEnumerable<string> namespaces)
        {
            if (regexGrouped == null)
                return;

            var layerNamespaces = namespaces
                .Where(n => regexGrouped.NamespaceRegexPattern.IsMatch(n));

            if (!layerNamespaces.Any())
                throw new ConstraintsException(
                    ConstraintsError.NamespaceNotFoundForLayer(key));
        }
    }
}