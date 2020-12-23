using ErosionFinder.Data.Models;
using ErosionFinder.Dtos;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ErosionFinder.Helpers.Tests
{
    public class ArchitecturalRuleHelperTest
    {
        [InlineData(RuleOperator.NeedToRelate)]
        [InlineData(RuleOperator.OnlyNeedToRelate)]
        [Theory(DisplayName = "ArchitecturalRuleHelper GetViolatingStructures - Success: NeedToRelate relation type")]
        [Trait(nameof(ArchitecturalRuleHelper.GetViolatingStructures), "Success_NeedToRelate")]
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

            var layersNamespaces = new Dictionary<string, IEnumerable<string>>()
            {
                { "Origin", new List<string>() { originStructure.Namespace } },
                { "Target", new List<string>() { targetNamespace } }
            };

            var structures = new List<Structure>() 
            {
                originStructure,
                new Structure()
                {
                    Name = targetStructureName,
                    Type = StructureType.Class,
                    Namespace = targetNamespace
                }
            };

            var violatingStructures = ArchitecturalRuleHelper.GetViolatingStructures(
                architectureRule, layersNamespaces, structures);

            Assert.NotNull(violatingStructures);
            Assert.Single(violatingStructures);
            
            Assert.Equal(originStructure.Name, 
                violatingStructures.Single().Name);
            Assert.Equal(originStructure.Type, 
                violatingStructures.Single().Type);
            Assert.Equal(originStructure.Namespace, 
                violatingStructures.Single().Namespace);
        }    

        [Fact(DisplayName = "ArchitecturalRuleHelper GetViolatingStructures - Success: OnlyCanRelate relation type")]
        [Trait(nameof(ArchitecturalRuleHelper.GetViolatingStructures), "Success_OnlyCanRelate")]
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

            var layersNamespaces = new Dictionary<string, IEnumerable<string>>()
            {
                { "Origin", new List<string>() { "Test.Origin" } },
                { "Target", new List<string>() { targetNamespace } },
                { "Another", new List<string>() { anotherStructure.Namespace } }
            };

            var structures = new List<Structure>() 
            {
                anotherStructure,
                new Structure()
                {
                    Name = targetStructureName,
                    Type = StructureType.Class,
                    Namespace = targetNamespace
                }
            };

            var violatingStructures = ArchitecturalRuleHelper.GetViolatingStructures(
                architectureRule, layersNamespaces, structures);

            Assert.NotNull(violatingStructures);
            Assert.Single(violatingStructures);
            
            Assert.Equal(anotherStructure.Name, 
                violatingStructures.Single().Name);
            Assert.Equal(anotherStructure.Type, 
                violatingStructures.Single().Type);
            Assert.Equal(anotherStructure.Namespace, 
                violatingStructures.Single().Namespace);
        }    

        [Fact(DisplayName = "ArchitecturalRuleHelper GetViolatingStructures - Success: CanNotRelate relation type")]
        [Trait(nameof(ArchitecturalRuleHelper.GetViolatingStructures), "Success_CanNotRelate")]
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

            var layersNamespaces = new Dictionary<string, IEnumerable<string>>()
            {
                { "Origin", new List<string>() { originStructure.Namespace } },
                { "Target", new List<string>() { targetNamespace } }
            };

            var structures = new List<Structure>() 
            {
                originStructure,
                new Structure()
                {
                    Name = targetStructureName,
                    Type = StructureType.Class,
                    Namespace = targetNamespace
                }
            };

            var violatingStructures = ArchitecturalRuleHelper.GetViolatingStructures(
                architectureRule, layersNamespaces, structures);

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