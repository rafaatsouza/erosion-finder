using ErosionFinder.Data.Models;
using ErosionFinder.Dtos;
using ErosionFinder.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace ErosionFinder.Helpers
{
    internal static class ArchitecturalRuleHelper
    {
        public static IEnumerable<Structure> GetViolatingStructures(
            ArchitecturalRule rule,
            IDictionary<string, IEnumerable<string>> layersNamespaces,
            IEnumerable<Structure> structures)
        {
            if (rule == null)
                return Enumerable.Empty<Structure>();

            rule.CheckIfItsValid();

            var originNamespaces = layersNamespaces[rule.OriginLayer];
            var targetNamespaces = layersNamespaces[rule.TargetLayer];

            var originStructures = structures
                .Where(s => originNamespaces.Any(n => n.Equals(s.Namespace)));

            if (rule.RuleOperator == RuleOperator.NeedToRelate)
            {
                return GetNeedToRelateViolatingStructures(
                    targetNamespaces, originStructures);
            }
            else if (rule.RuleOperator == RuleOperator.CanNotRelate)
            {
                return GetCanNotRelateViolatingStructures(
                    targetNamespaces, originStructures);
            }
            else
            {
                var anotherStructures = structures
                    .Where(s => !originNamespaces.Any(n => n.Equals(s.Namespace)));

                if (rule.RuleOperator == RuleOperator.OnlyNeedToRelate)
                {
                    return GetOnlyNeedToRelateViolatingStructures(
                        targetNamespaces, originStructures, anotherStructures);
                }
                else if (rule.RuleOperator == RuleOperator.OnlyCanRelate)
                {
                    return GetOnlyCanRelateViolatingStructures(
                        targetNamespaces, anotherStructures);
                }
            }
            
            return Enumerable.Empty<Structure>();
        }

        private static IEnumerable<Structure> GetNeedToRelateViolatingStructures(
            IEnumerable<string> targetNamespaces, IEnumerable<Structure> originStructures)
        {
            return originStructures
                .Where(s => !s.Relations
                    .Any(r => targetNamespaces.Any(n => n.Equals(r.Target))));
        }

        private static IEnumerable<Structure> GetOnlyNeedToRelateViolatingStructures(
            IEnumerable<string> targetNamespaces, IEnumerable<Structure> originStructures,
            IEnumerable<Structure> anotherStructures)
        {
            var violatingStructures = originStructures
                .Where(s => !s.Relations
                    .Any(r => targetNamespaces.Any(n => n.Equals(r.Target))));

            var anotherViolatingStructures = anotherStructures
                .Where(s => s.Relations
                    .Any(r => targetNamespaces.Any(n => n.Equals(r.Target))));

            return violatingStructures.Concat(anotherViolatingStructures);
        }

        private static IEnumerable<Structure> GetOnlyCanRelateViolatingStructures(
            IEnumerable<string> targetNamespaces, IEnumerable<Structure> anotherStructures)
        {
            return anotherStructures
                .Where(s => s.Relations
                    .Any(r => targetNamespaces.Any(n => n.Equals(r.Target))));
        }

        private static IEnumerable<Structure> GetCanNotRelateViolatingStructures(
            IEnumerable<string> targetNamespaces, IEnumerable<Structure> originStructures)
        {
            return originStructures
                .Where(s => s.Relations
                    .Any(r => targetNamespaces.Any(n => n.Equals(r.Target))));
        }
    }
}