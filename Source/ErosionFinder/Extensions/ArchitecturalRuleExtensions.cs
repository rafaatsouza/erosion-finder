using ErosionFinder.Data.Exceptions;
using ErosionFinder.Data.Models;
using ErosionFinder.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace ErosionFinder.Extensions
{
    internal static class ArchitecturalRuleExtensions
    {
        public static void CheckIfItsValid(
            this ArchitecturalRule rule)
        {
            if (rule != null)
            {
                var invalidRule = string.IsNullOrEmpty(rule.OriginLayer) 
                    || string.IsNullOrEmpty(rule.TargetLayer) 
                    || rule.RuleOperator == default(RuleOperator);

                if (invalidRule)
                {
                    throw new ConstraintsException(
                        ConstraintsError.InvalidRule);
                }
            }
        }

        public static IEnumerable<Structure> GetViolatingStructures(
            this ArchitecturalRule rule, IEnumerable<string> targetNamespaces, 
            IEnumerable<Structure> originStructures, IEnumerable<Structure> anotherStructures)
        {
            if (rule == null)
                return Enumerable.Empty<Structure>();

            rule.CheckIfItsValid();

            var violatingStructures = GetViolatingStructuresByNamespaces(
                rule.RuleOperator, targetNamespaces, originStructures, anotherStructures);

            if (rule.HasParticularRelationTypes)
            {
                //TODO: Filter violating by particular relation types
            }

            return violatingStructures;
        }

        private static IEnumerable<Structure> GetViolatingStructuresByNamespaces(
            RuleOperator ruleOperator, IEnumerable<string> targetNamespaces, 
            IEnumerable<Structure> originStructures, IEnumerable<Structure> anotherStructures)
        {
            if (ruleOperator == RuleOperator.NeedToRelate)
            {
                return GetNeedToRelateViolatingStructures(
                    targetNamespaces, originStructures);
            }
            else if (ruleOperator == RuleOperator.OnlyNeedToRelate)
            {
                return GetOnlyNeedToRelateViolatingStructures(
                    targetNamespaces, originStructures, anotherStructures);
            }
            else if (ruleOperator == RuleOperator.OnlyCanRelate)
            {
                return GetOnlyCanRelateViolatingStructures(
                    targetNamespaces, anotherStructures);
            }
            else if (ruleOperator == RuleOperator.CanNotRelate)
            {
                return GetCanNotRelateViolatingStructures(
                    targetNamespaces, originStructures);
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