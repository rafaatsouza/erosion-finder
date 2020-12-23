using ErosionFinder.Data.Models;
using ErosionFinder.Data.Exceptions;
using Xunit;

namespace ErosionFinder.Extensions.Tests
{
    public class ArchitecturalRuleExtensionsTest
    {
        [Fact(DisplayName = "ArchitecturalRuleExtensions CheckIfItsValid - Error: Null reference")]
        [Trait(nameof(ArchitecturalRuleExtensions.CheckIfItsValid), "Error_NullReference")]
        public void CheckIfItsValid_ArchitecturalRule_Error_NullReference()
        {
            ArchitecturalRule rule = null;
            
            var result = Record.Exception(() => 
            {
                rule.CheckIfItsValid();
            });

            Assert.Null(result);
        }

        [Fact(DisplayName = "ArchitecturalRuleExtensions CheckIfItsValid - Error: Invalid rule")]
        [Trait(nameof(ArchitecturalRuleExtensions.CheckIfItsValid), "Error_InvalidRule")]
        public void CheckIfItsValid_ArchitecturalRule_Error_InvalidRule()
        {
            var rules = new ArchitecturalRule[3]
            {
                new ArchitecturalRule(),
                new ArchitecturalRule()
                {
                    OriginLayer = "Origin"
                },
                new ArchitecturalRule()
                {
                    OriginLayer = "Origin",
                    TargetLayer = "Target"
                }
            };

            foreach (var rule in rules)
            {
                var result = Assert.Throws<ConstraintsException>(() => 
                {
                    rule.CheckIfItsValid();
                });

                Assert.Equal(ConstraintsError.InvalidRule.Key, result.Key);   
            }
        }

        [Fact(DisplayName = "ArchitecturalRuleExtensions CheckIfItsValid - Success")]
        [Trait(nameof(ArchitecturalRuleExtensions.CheckIfItsValid), "Success")]
        public void CheckIfItsValid_ArchitecturalRule_Success()
        {
            var rule = new ArchitecturalRule()
            {
                OriginLayer = "Origin",
                TargetLayer = "Target",
                RuleOperator = RuleOperator.OnlyNeedToRelate
            };
            
            var result = Record.Exception(() => 
            {
                rule.CheckIfItsValid();
            });

            Assert.Null(result);
        }
    }
}
