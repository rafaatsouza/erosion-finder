using ErosionFinder.Extensions;
using ErosionFinder.Data.Exceptions;
using ErosionFinder.Data.Models;
using ErosionFinder.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ErosionFinder.Util
{
    internal static class ViolationsMethods
    {
        private const int MaxDegreeOfParallelism = 10;

        public static IEnumerable<Violation> GetViolations(ArchitecturalConstraints constraints, 
            IEnumerable<CodeFile> codeFiles, CancellationToken cancellationToken)
        {
            constraints.CheckIfItsValid();

            var violations = new List<Violation>();
            var structures = codeFiles.SelectMany(c => c.Structures);
            var namespaces = structures.Select(s => s.Namespace).Distinct();

            var layersNamespaces = GetLayerNamespaces(constraints.Layers, namespaces);

            var lockObject = new object();

            var parallelOptions = new ParallelOptions()
            {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = MaxDegreeOfParallelism
            };

            Parallel.ForEach(constraints.Rules, parallelOptions, rule =>
            {
                var originNamespaces = layersNamespaces[rule.OriginLayer];
                var targetNamespaces = layersNamespaces[rule.TargetLayer];

                var originStructures = structures
                    .Where(s => originNamespaces.Any(n => n.Equals(s.Namespace)));

                var anotherStructures = structures
                    .Where(s => !originNamespaces.Any(n => n.Equals(s.Namespace)));

                var violatingStructures = rule.GetViolatingStructures(
                    targetNamespaces, originStructures, anotherStructures);

                if (violatingStructures.Any())
                {
                    lock (lockObject)
                    {
                        violations.Add(new Violation()
                        {
                            Rule = rule,
                            Structures = violatingStructures
                                .Select(s => s.Name).OrderBy(n => n)
                        });
                    }
                }
            });

            return violations;
        }

        private static IDictionary<string, IEnumerable<string>> GetLayerNamespaces(
            IDictionary<string, NamespacesGroupingMethod> layers, IEnumerable<string> namespaces)
        {
            var layersNamespaces = new Dictionary<string, IEnumerable<string>>();

            foreach (var layer in layers)
            {
                if (!layersNamespaces.ContainsKey(layer.Key))
                {
                    var layerNamespaces = GetNamespacesByGroupingMethod(
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

        private static IEnumerable<string> GetNamespacesByGroupingMethod(
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