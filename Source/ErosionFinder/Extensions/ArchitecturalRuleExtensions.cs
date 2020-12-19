using ErosionFinder.Data.Models;
using ErosionFinder.Data.Exceptions;
using System.Collections.Generic;
using ErosionFinder.Dtos;
using System.Linq;

namespace ErosionFinder.Extensions
{
    internal static class ArchitecturalRuleExtensions
    {
        public static void CheckIfItsValid(this ArchitecturalRule rule)
        {
            if (rule == null)
                return;

           if (string.IsNullOrEmpty(rule.OriginLayer)
            || string.IsNullOrEmpty(rule.TargetLayer)
            || rule.RuleOperator == default(RuleOperator)) 
            {
                throw new ConstraintsException(
                    ConstraintsError.InvalidRule);
            }
        }

        public static IEnumerable<Structure> GetViolatingStructures(
            this ArchitecturalRule rule, IEnumerable<string> targetNamespaces, 
            IEnumerable<Structure> originStructures, IEnumerable<Structure> anotherStructures)
        {
            if (rule == null)
                return Enumerable.Empty<Structure>();

            rule.CheckIfItsValid();

            switch (rule.RuleOperator)
            {
                case (RuleOperator.NeedToRelate):
                    return GetNeedToRelateViolatingStructures(
                        targetNamespaces, originStructures);

                case (RuleOperator.OnlyNeedToRelate):
                    return GetOnlyNeedToRelateViolatingStructures(
                        targetNamespaces, originStructures, anotherStructures);

                case (RuleOperator.OnlyCanRelate):
                    return GetOnlyCanRelateViolatingStructures(
                        targetNamespaces, anotherStructures);

                case (RuleOperator.CanNotRelate):
                    return GetCanNotRelateViolatingStructures(
                        targetNamespaces, originStructures);
            }

            return Enumerable.Empty<Structure>();
        }

        private static IEnumerable<Structure> GetNeedToRelateViolatingStructures(
            IEnumerable<string> targetNamespaces, IEnumerable<Structure> originStructures)
            => originStructures
                .Where(s => !s.Relations
                    .Any(r => targetNamespaces.Any(n => n.Equals(r.Target))));

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
            => anotherStructures
                .Where(s => s.Relations
                    .Any(r => targetNamespaces.Any(n => n.Equals(r.Target))));

        private static IEnumerable<Structure> GetCanNotRelateViolatingStructures(
            IEnumerable<string> targetNamespaces, IEnumerable<Structure> originStructures)
            => originStructures
                .Where(s => s.Relations
                    .Any(r => targetNamespaces.Any(n => n.Equals(r.Target))));
    }
}