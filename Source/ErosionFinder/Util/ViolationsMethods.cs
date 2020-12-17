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
            var violations = new List<Violation>();
            var structures = codeFiles.SelectMany(c => c.Structures);
            var namespaces = structures.Select(s => s.Namespace).Distinct();

            var layersNamespaces = GetNamespacesForEachLayer(constraints, namespaces).ToList();

            var lockObject = new object();

            var parallelOptions = new ParallelOptions()
            {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = MaxDegreeOfParallelism
            };

            Parallel.ForEach(constraints.Rules, parallelOptions, rule =>
            {
                var originNamespaces = layersNamespaces
                    .Single(l => l.Layer.Equals(rule.OriginLayer))
                    .Namespaces;

                var targetNamespaces = layersNamespaces
                    .Single(l => l.Layer.Equals(rule.TargetLayer))
                    .Namespaces;

                var originStructures = structures
                    .Where(s => originNamespaces.Any(n => n.Equals(s.Namespace)));

                var anotherStructures = structures
                    .Where(s => !originNamespaces.Any(n => n.Equals(s.Namespace)));

                IEnumerable<Structure> violatingStructures = null;

                if (rule.RuleOperator == RuleOperator.NeedToRelate
                    || rule.RuleOperator == RuleOperator.OnlyNeedToRelate)
                {
                    violatingStructures = originStructures
                        .Where(s => !(s.Relations?.Any(r => targetNamespaces.Any(n => n.Equals(r.Target))) ?? false));

                    if (rule.RuleOperator == RuleOperator.OnlyNeedToRelate)
                    {
                        violatingStructures = violatingStructures
                            .Concat(anotherStructures.
                                Where(s => (s.Relations?.Any(r => targetNamespaces.Any(n => n.Equals(r.Target)))) ?? false));
                    }
                }
                else if (rule.RuleOperator == RuleOperator.OnlyCanRelate)
                {
                    violatingStructures = anotherStructures.
                        Where(s => (s.Relations?.Any(r => targetNamespaces.Any(n => n.Equals(r.Target))) ?? false));
                }
                else if (rule.RuleOperator == RuleOperator.CanNotRelate)
                {
                    violatingStructures = originStructures
                        .Where(s => (s.Relations?.Any(r => targetNamespaces.Any(n => n.Equals(r.Target))) ?? false));
                }

                if (violatingStructures?.Any() ?? false)
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

        private static IEnumerable<LayerNamespaces> GetNamespacesForEachLayer(
            ArchitecturalConstraints constraints, IEnumerable<string> namespaces)
        {
            foreach (var layer in constraints.Layers)
            {
                var namespaceGrouppingMethod = layer.Value;

                IEnumerable<string> layerNamespaces = null;

                if (namespaceGrouppingMethod is NamespacesExplicitlyGrouped explicitlyGrouped)
                {
                    layerNamespaces = explicitlyGrouped.Namespaces;

                }
                else if (namespaceGrouppingMethod is NamespacesRegularExpressionGrouped regexGrouped)
                {
                    layerNamespaces = namespaces
                        .Where(n => regexGrouped.NamespaceRegexPattern.IsMatch(n));
                }

                if (layerNamespaces == null || !layerNamespaces.Any())
                {
                    throw new ConstraintsException(
                        ConstraintsError.NamespaceNotFoundForLayer(layer.Key));
                }

                yield return new LayerNamespaces()
                {
                    Layer = layer.Key,
                    Namespaces = layerNamespaces
                };
            }
        }
    }
}