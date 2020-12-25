using ErosionFinder.Data.Exceptions;
using ErosionFinder.Data.Models;
using System.Collections.Generic;
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

        [Fact(DisplayName = "ArchitecturalRuleExtensions IsSameRule - Success: NullRules")]
        [Trait(nameof(ArchitecturalRuleExtensions.IsSameRule), "Success_NullRules")]
        public void IsSameRule_ArchitecturalRule_Success_NullRules()
        {
            var rule = (ArchitecturalRule)null;

            Assert.True(rule.IsSameRule(null));
        }

        [Fact(DisplayName = "ArchitecturalRuleExtensions IsSameRule - Success: NullRule")]
        [Trait(nameof(ArchitecturalRuleExtensions.IsSameRule), "Success_NullRule")]
        public void IsSameRule_ArchitecturalRule_Success_NullRule()
        {
            var rule = new ArchitecturalRule();

            Assert.False(rule.IsSameRule(null));
        }

        [Fact(DisplayName = "ArchitecturalRuleExtensions IsSameRule - Success: DifferentRelationTypesCount")]
        [Trait(nameof(ArchitecturalRuleExtensions.IsSameRule), "Success_DifferentRelationTypesCount")]
        public void IsSameRule_ArchitecturalRule_Success_DifferentRelationTypesCount()
        {
            var rule = new ArchitecturalRule()
            {
                RelationTypes = new List<RelationType>()
                    { RelationType.Declarate }
            };

            var anotherRule = new ArchitecturalRule()
            {
                RelationTypes = new List<RelationType>()
                    { RelationType.Declarate, RelationType.Inheritance }
            };

            Assert.False(rule.IsSameRule(anotherRule));
        }

        [Fact(DisplayName = "ArchitecturalRuleExtensions IsSameRule - Success: DifferentOriginLayer")]
        [Trait(nameof(ArchitecturalRuleExtensions.IsSameRule), "Success_DifferentOriginLayer")]
        public void IsSameRule_ArchitecturalRule_Success_DifferentOriginLayer()
        {
            var rule = new ArchitecturalRule()
            {
                OriginLayer = "Origin"
            };

            var anotherRule = new ArchitecturalRule()
            {
                OriginLayer = "Test"
            };

            Assert.False(rule.IsSameRule(anotherRule));
        }

        [Fact(DisplayName = "ArchitecturalRuleExtensions IsSameRule - Success: DifferentTargetLayer")]
        [Trait(nameof(ArchitecturalRuleExtensions.IsSameRule), "Success_DifferentTargetLayer")]
        public void IsSameRule_ArchitecturalRule_Success_DifferentTargetLayer()
        {
            var rule = new ArchitecturalRule()
            {
                OriginLayer = "Origin",
                TargetLayer = "Target"
            };

            var anotherRule = new ArchitecturalRule()
            {
                OriginLayer = "Origin",
                TargetLayer = "Test"
            };

            Assert.False(rule.IsSameRule(anotherRule));
        }
    
        [Fact(DisplayName = "ArchitecturalRuleExtensions IsSameRule - Success: DifferentRuleOperator")]
        [Trait(nameof(ArchitecturalRuleExtensions.IsSameRule), "Success_DifferentRuleOperator")]
        public void IsSameRule_ArchitecturalRule_Success_DifferentRuleOperator()
        {
            var rule = new ArchitecturalRule()
            {
                OriginLayer = "Origin",
                TargetLayer = "Target",
                RuleOperator = RuleOperator.CanNotRelate
            };

            var anotherRule = new ArchitecturalRule()
            {
                OriginLayer = "Origin",
                TargetLayer = "Target",
                RuleOperator = RuleOperator.NeedToRelate
            };

            Assert.False(rule.IsSameRule(anotherRule));
        }

        [Fact(DisplayName = "ArchitecturalRuleExtensions IsSameRule - Success: DifferentRelationTypes")]
        [Trait(nameof(ArchitecturalRuleExtensions.IsSameRule), "Success_DifferentRelationTypes")]
        public void IsSameRule_ArchitecturalRule_Success_DifferentRelationTypes()
        {
            var rule = new ArchitecturalRule()
            {
                OriginLayer = "Origin",
                TargetLayer = "Target",
                RuleOperator = RuleOperator.CanNotRelate,
                RelationTypes = new List<RelationType>()
                    { RelationType.Declarate, RelationType.Indirect }
            };

            var anotherRule = new ArchitecturalRule()
            {
                OriginLayer = "Origin",
                TargetLayer = "Target",
                RuleOperator = RuleOperator.CanNotRelate,
                RelationTypes = new List<RelationType>()
                    { RelationType.Declarate, RelationType.Instantiate }
            };

            Assert.False(rule.IsSameRule(anotherRule));
        }

        [Fact(DisplayName = "ArchitecturalRuleExtensions IsSameRule - Success")]
        [Trait(nameof(ArchitecturalRuleExtensions.IsSameRule), "Success")]
        public void IsSameRule_ArchitecturalRule_Success()
        {
            var rule = new ArchitecturalRule()
            {
                OriginLayer = "Origin",
                TargetLayer = "Target",
                RuleOperator = RuleOperator.CanNotRelate,
                RelationTypes = new List<RelationType>()
                    { RelationType.Declarate, RelationType.Indirect }
            };

            var anotherRule = new ArchitecturalRule()
            {
                OriginLayer = "Origin",
                TargetLayer = "Target",
                RuleOperator = RuleOperator.CanNotRelate,
                RelationTypes = new List<RelationType>()
                    { RelationType.Declarate, RelationType.Indirect }
            };

            Assert.True(rule.IsSameRule(anotherRule));
        }
    }
}
