using ErosionFinder.Data.Models;
using ErosionFinder.Data.Exceptions;
using ErosionFinder.Dtos;
using System.Collections.Generic;
using System.Linq;
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

        [InlineData(RuleOperator.NeedToRelate)]
        [InlineData(RuleOperator.OnlyNeedToRelate)]
        [Theory(DisplayName = "ArchitecturalRuleExtensions GetViolatingStructures - Success: NeedToRelate relation type")]
        [Trait(nameof(ArchitecturalRuleExtensions.GetViolatingStructures), "Success_NeedToRelate")]
        public void GetViolatingStructures_Success_NeedToRelate(RuleOperator ruleOperator)
        {
            var architectureRule = new ArchitecturalRule()
            {
                OriginLayer = "Origin",
                TargetLayer = "Target",
                RuleOperator = ruleOperator
            };

            var targetNamespace = "Test.Target";            
            var targetStructureName = "TargetTestStructure";
            
            var originStructure = new Structure()
            {
                Name = "OriginTestStructure",
                Type = StructureType.Class,
                Namespace = "Test.Origin"
            };

            var targetStructure = new Structure()
            {
                Name = targetStructureName,
                Type = StructureType.Class,
                Namespace = targetNamespace
            };

            var violatingStructures = architectureRule.GetViolatingStructures(
                new List<string>() { targetNamespace }, 
                new List<Structure>() { originStructure }, 
                new List<Structure>() { targetStructure });

            Assert.NotNull(violatingStructures);
            Assert.Single(violatingStructures);
            
            Assert.Equal(originStructure.Name, 
                violatingStructures.Single().Name);
            Assert.Equal(originStructure.Type, 
                violatingStructures.Single().Type);
            Assert.Equal(originStructure.Namespace, 
                violatingStructures.Single().Namespace);
        }    

        [Fact(DisplayName = "ArchitecturalRuleExtensions GetViolatingStructures - Success: OnlyCanRelate relation type")]
        [Trait(nameof(ArchitecturalRuleExtensions.GetViolatingStructures), "Success_OnlyCanRelate")]
        public void GetViolatingStructures_Success_OnlyCanRelate()
        {
            var architectureRule = new ArchitecturalRule()
            {
                OriginLayer = "Origin",
                TargetLayer = "Target",
                RuleOperator = RuleOperator.OnlyCanRelate
            };

            var targetNamespace = "Test.Target";            
            var targetStructureName = "TargetTestStructure";
            
            var anotherStructure = new Structure()
            {
                Name = "AnotherTestStructure",
                Type = StructureType.Class,
                Namespace = "Test.Another",
                References = new List<string>() { { targetNamespace } },
                Relations = new List<Relation>()
                    { new Relation(RelationType.Inheritance, 
                        targetNamespace, true, targetStructureName) }
            };

            var targetStructure = new Structure()
            {
                Name = targetStructureName,
                Type = StructureType.Class,
                Namespace = targetNamespace
            };

            var violatingStructures = architectureRule.GetViolatingStructures(
                new List<string>() { targetNamespace }, 
                new List<Structure>() { }, 
                new List<Structure>() { anotherStructure, targetStructure });

            Assert.NotNull(violatingStructures);
            Assert.Single(violatingStructures);
            
            Assert.Equal(anotherStructure.Name, 
                violatingStructures.Single().Name);
            Assert.Equal(anotherStructure.Type, 
                violatingStructures.Single().Type);
            Assert.Equal(anotherStructure.Namespace, 
                violatingStructures.Single().Namespace);
        }    

        [Fact(DisplayName = "ArchitecturalRuleExtensions GetViolatingStructures - Success: CanNotRelate relation type")]
        [Trait(nameof(ArchitecturalRuleExtensions.GetViolatingStructures), "Success_CanNotRelate")]
        public void GetViolatingStructures_Success_CanNotRelate()
        {
            var architectureRule = new ArchitecturalRule()
            {
                OriginLayer = "Origin",
                TargetLayer = "Target",
                RuleOperator = RuleOperator.CanNotRelate
            };

            var targetNamespace = "Test.Target";            
            var targetStructureName = "TargetTestStructure";
            
            var originStructure = new Structure()
            {
                Name = "OriginTestStructure",
                Type = StructureType.Class,
                Namespace = "Test.Origin",
                References = new List<string>() { { targetNamespace } },
                Relations = new List<Relation>()
                    { new Relation(RelationType.Inheritance, 
                        targetNamespace, true, targetStructureName) }
            };

            var targetStructure = new Structure()
            {
                Name = targetStructureName,
                Type = StructureType.Class,
                Namespace = targetNamespace
            };

            var violatingStructures = architectureRule.GetViolatingStructures(
                new List<string>() { targetNamespace }, 
                new List<Structure>() { originStructure }, 
                new List<Structure>() { targetStructure });

            Assert.NotNull(violatingStructures);
            Assert.Single(violatingStructures);
            
            Assert.Equal(originStructure.Name, 
                violatingStructures.Single().Name);
            Assert.Equal(originStructure.Type, 
                violatingStructures.Single().Type);
            Assert.Equal(originStructure.Namespace, 
                violatingStructures.Single().Namespace);
        }    
    }
}
