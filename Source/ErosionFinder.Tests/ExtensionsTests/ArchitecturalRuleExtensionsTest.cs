using ErosionFinder.Extensions;
using ErosionFinder.Data.Exceptions;
using ErosionFinder.Data.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;

namespace ErosionFinder.Extensions.Tests
{
    public class ArchitecturalRuleExtensionsTest
    {
        [Fact]
        [Trait(nameof(ArchitecturalRuleExtensions.CheckIfItsValid), "ArchitecturalRule_Error_NullReference")]
        public void CheckIfItsValid_ArchitecturalRule_Error_NullReference()
        {
            ArchitecturalRule rule = null;
            
            var result = Record.Exception(() => 
            {
                rule.CheckIfItsValid();
            });

            Assert.Null(result);
        }

        [Fact]
        [Trait(nameof(ArchitecturalRuleExtensions.CheckIfItsValid), "ArchitecturalRule_Error_InvalidRule")]
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

        [Fact]
        [Trait(nameof(ArchitecturalRuleExtensions.CheckIfItsValid), "ArchitecturalRule_Success")]
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
