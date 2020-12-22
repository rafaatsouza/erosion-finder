using ErosionFinder.Data.Exceptions;
using ErosionFinder.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace ErosionFinder.Helpers
{
    internal static class NamespacesGroupingMethodHelper
    {
        public static IDictionary<string, IEnumerable<string>> GetLayersNamespaces(
            IDictionary<string, NamespacesGroupingMethod> layers, IEnumerable<string> namespaces)
        {
            var layersNamespaces = new Dictionary<string, IEnumerable<string>>();

            foreach (var layer in layers)
            {
                if (!layersNamespaces.ContainsKey(layer.Key))
                {
                    var layerNamespaces = GetNamespaces(
                        layer.Value, namespaces);

                    if (layerNamespaces == null || !layerNamespaces.Any())
                    {
                        throw new ConstraintsException(
                            ConstraintsError.NamespaceNotFoundForLayer(layer.Key));
                    }

                    layersNamespaces.Add(layer.Key, layerNamespaces);
                }
            }
            
            return layersNamespaces;
        }

        private static IEnumerable<string> GetNamespaces(
            NamespacesGroupingMethod groupingMethod, IEnumerable<string> namespaces)
        {
            if (groupingMethod is NamespacesExplicitlyGrouped explicitlyGrouped)
            {
                return explicitlyGrouped.Namespaces;
            }
            else if (groupingMethod is NamespacesRegularExpressionGrouped regexGrouped)
            {
                return namespaces
                    .Where(n => regexGrouped.NamespaceRegexPattern.IsMatch(n));
            }

            return Enumerable.Empty<string>();
        }
    }    
}