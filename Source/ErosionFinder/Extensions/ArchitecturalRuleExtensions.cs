using System;
using System.Linq;
using System.Runtime.CompilerServices;
using ErosionFinder.Data.Exceptions;
using ErosionFinder.Data.Models;

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

        public static bool IsSameRule(this ArchitecturalRule mainRule, 
            ArchitecturalRule targetRule)
        {
            if (mainRule == null && targetRule == null)
                return true;

            if (mainRule == null || targetRule == null)
                return false;

            if (mainRule.RelationTypes.Count() != targetRule.RelationTypes.Count())
                return false;

            return TextEquals(mainRule.OriginLayer, targetRule.OriginLayer)
                && TextEquals(mainRule.TargetLayer, targetRule.TargetLayer)
                && mainRule.RuleOperator == targetRule.RuleOperator
                && !mainRule.RelationTypes
                    .Any(rt => !targetRule.RelationTypes.Any(rtt => rtt == rt));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TextEquals(string text, string anotherText)
            => string.Equals(text, anotherText, StringComparison.InvariantCultureIgnoreCase);
    }
}