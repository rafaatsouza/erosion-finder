using ErosionFinder.Data.Models;
using ErosionFinder.Data.Exceptions;

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
    }
}