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
    }
}