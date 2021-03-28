using ErosionFinder.Data.Models;
using ErosionFinder.Dtos;
using ErosionFinder.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace ErosionFinder.Helpers
{
    internal static class ArchitecturalRuleHelper
    {
        public static IEnumerable<ArchitecturalViolationOccurrence> GetViolatingOccurrences(
            ArchitecturalRule rule,
            IDictionary<string, IEnumerable<string>> layersNamespaces,
            IEnumerable<Structure> structures)
        {
            if (rule == null)
                return Enumerable.Empty<ArchitecturalViolationOccurrence>();

            rule.CheckIfItsValid();

            var originNamespaces = layersNamespaces[rule.OriginLayer];
            var targetNamespaces = layersNamespaces[rule.TargetLayer];

            var originStructures = structures
                .Where(s => originNamespaces.Any(n => n.Equals(s.Namespace)));

            if (rule.RuleOperator == RuleOperator.NeedToRelate)
            {
                return GetNeedToRelateViolatingOccurrences(
                    targetNamespaces, originStructures, rule.RelationTypes);
            }
            else if (rule.RuleOperator == RuleOperator.CanNotRelate)
            {
                return GetCanNotRelateViolatingOccurrences(
                    targetNamespaces, originStructures, rule.RelationTypes);
            }
            else
            {
                var anotherStructures = structures
                    .Where(s => !originNamespaces.Any(n => n.Equals(s.Namespace)));

                if (rule.RuleOperator == RuleOperator.OnlyNeedToRelate)
                {
                    return GetOnlyNeedToRelateViolatingOccurrences(
                        targetNamespaces, originStructures, 
                        anotherStructures, rule.RelationTypes);
                }
                else if (rule.RuleOperator == RuleOperator.OnlyCanRelate)
                {
                    return GetOnlyCanRelateViolatingOccurrences(
                        targetNamespaces, anotherStructures, rule.RelationTypes);
                }
            }
            
            return Enumerable.Empty<ArchitecturalViolationOccurrence>();
        }

        private static IEnumerable<ArchitecturalViolationOccurrence> GetNeedToRelateViolatingOccurrences(
            IEnumerable<string> targetNamespaces, IEnumerable<Structure> originStructures,
            IEnumerable<RelationType> relationTypes)
        {
            return originStructures
                .Where(s => !s.Relations
                    .Any(r => RelationTypeIsInDefinedList(r, relationTypes) 
                        && targetNamespaces.Any(n => n.Equals(r.Target))))
                .Select(s => new ArchitecturalViolationOccurrence()
                {
                    Structure = $"{s.Namespace}.{s.Name}"
                });                    
        }

        private static IEnumerable<ArchitecturalViolationOccurrence> GetOnlyNeedToRelateViolatingOccurrences(
            IEnumerable<string> targetNamespaces, IEnumerable<Structure> originStructures,
            IEnumerable<Structure> anotherStructures, IEnumerable<RelationType> relationTypes)
        {
            var violatingOccurrences = originStructures
                .Where(s => !s.Relations
                    .Any(r => RelationTypeIsInDefinedList(r, relationTypes) 
                        && targetNamespaces.Any(n => n.Equals(r.Target))))
                .Select(s => new ArchitecturalViolationOccurrence()
                {
                    Structure = $"{s.Namespace}.{s.Name}"
                });

            var anotherViolatingOccurrences = anotherStructures
                .Select(s => 
                {
                    var notAllowedRelations = s.Relations
                        .Where(r => RelationTypeIsInDefinedList(r, relationTypes) 
                            && targetNamespaces.Any(n => n.Equals(r.Target))
                            && !s.Namespace.Equals(r.Target));

                    return GetOccurrenceByStructureAndRelations(s, notAllowedRelations);
                })
                .Where(occurrence => occurrence != null);

            return violatingOccurrences
                .Concat(anotherViolatingOccurrences);
        }

        private static IEnumerable<ArchitecturalViolationOccurrence> GetOnlyCanRelateViolatingOccurrences(
            IEnumerable<string> targetNamespaces, IEnumerable<Structure> anotherStructures,
            IEnumerable<RelationType> relationTypes)
        {
            return anotherStructures
                .Select(s => 
                {
                    var notAllowedRelations = s.Relations
                        .Where(r => RelationTypeIsInDefinedList(r, relationTypes) 
                            && targetNamespaces.Any(n => n.Equals(r.Target))
                            && !s.Namespace.Equals(r.Target));

                    return GetOccurrenceByStructureAndRelations(s, notAllowedRelations);
                })
                .Where(occurrence => occurrence != null);
        }

        private static IEnumerable<ArchitecturalViolationOccurrence> GetCanNotRelateViolatingOccurrences(
            IEnumerable<string> targetNamespaces, IEnumerable<Structure> originStructures,
            IEnumerable<RelationType> relationTypes)
        {
            return originStructures
                .Select(s => 
                {
                    var notAllowedRelations = s.Relations
                        .Where(r => RelationTypeIsInDefinedList(r, relationTypes)
                            && targetNamespaces.Any(n => n.Equals(r.Target)));

                    return GetOccurrenceByStructureAndRelations(s, notAllowedRelations);
                })
                .Where(occurrence => occurrence != null);
        }

        private static bool RelationTypeIsInDefinedList(
            Relation relation, IEnumerable<RelationType> relationTypes)
        {
            if (!relationTypes.Any())
                return true;

            return relationTypes.Any(rt => rt == relation.RelationType);
        }

        private static ArchitecturalViolationOccurrence GetOccurrenceByStructureAndRelations(
            Structure structure, IEnumerable<Relation> notAllowedRelations)
        {
            if (notAllowedRelations == null || !notAllowedRelations.Any())
                return null;

            var nonConforming = notAllowedRelations
                .Select(r => new NonConformingRelation()
                {
                    RelationType = r.RelationType,
                    Targets = r.Components
                        .Select(c => $"{r.Target}.{c}")
                });

            return new ArchitecturalViolationOccurrence()
            {
                Structure = $"{structure.Namespace}.{structure.Name}",
                NonConformingRelations = nonConforming
            };
        }
    }
}