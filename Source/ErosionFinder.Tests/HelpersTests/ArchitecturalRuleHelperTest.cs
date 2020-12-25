using ErosionFinder.Data.Models;
using ErosionFinder.Dtos;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ErosionFinder.Helpers.Tests
{
    public class ArchitecturalRuleHelperTest
    {
        [Fact(DisplayName = "ArchitecturalRuleHelper GetViolatingOccurrences - Success: NeedToRelate relation type")]
        [Trait(nameof(ArchitecturalRuleHelper.GetViolatingOccurrences), "Success_NeedToRelate")]
        public void GetViolatingOccurrences_Success_NeedToRelate()
        {
            var architectureRule = new ArchitecturalRule()
            {
                OriginLayer = "Origin",
                TargetLayer = "Target",
                RuleOperator = RuleOperator.NeedToRelate
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

            var violatingOccurrences = ArchitecturalRuleHelper.GetViolatingOccurrences(
                architectureRule, layersNamespaces, structures);

            Assert.NotNull(violatingOccurrences);
            Assert.Single(violatingOccurrences);
            
            Assert.Equal($"{originStructure.Namespace}.{originStructure.Name}", 
                violatingOccurrences.Single().Structure);
            Assert.Empty(violatingOccurrences.Single().NonConformingRelations);
        }   

        [Fact(DisplayName = "ArchitecturalRuleHelper GetViolatingOccurrences - Success: OnlyNeedToRelate relation type")]
        [Trait(nameof(ArchitecturalRuleHelper.GetViolatingOccurrences), "Success_OnlyNeedToRelate")]
        public void GetViolatingOccurrences_Success_OnlyNeedToRelate()
        {
            var architectureRule = new ArchitecturalRule()
            {
                OriginLayer = "Origin",
                TargetLayer = "Target",
                RuleOperator = RuleOperator.OnlyNeedToRelate
            };

            var targetNamespace = "Test.Target";            
            var targetStructureName = "TargetTestStructure";

            var originStructure = new Structure()
            {
                Name = "OriginTestStructure",
                Type = StructureType.Class,
                Namespace = "Test.Origin",
                Relations = new List<Relation>()
                {
                    new Relation(RelationType.Instantiate, 
                        targetNamespace, true, targetStructureName)
                }
            };

            var anotherStructure = new Structure()
            {
                Name = "AnotherTestStructure",
                Type = StructureType.Class,
                Namespace = "Test.Another",
                Relations = new List<Relation>()
                {
                    new Relation(RelationType.Instantiate, 
                        targetNamespace, true, targetStructureName)
                }
            };

            var layersNamespaces = new Dictionary<string, IEnumerable<string>>()
            {
                { "Origin", new List<string>() { originStructure.Namespace } },
                { "Another", new List<string>() { anotherStructure.Namespace } },
                { "Target", new List<string>() { targetNamespace } }
            };

            var structures = new List<Structure>() 
            {
                originStructure,
                anotherStructure,
                new Structure()
                {
                    Name = targetStructureName,
                    Type = StructureType.Class,
                    Namespace = targetNamespace
                }
            };

            var violatingOccurrences = ArchitecturalRuleHelper.GetViolatingOccurrences(
                architectureRule, layersNamespaces, structures);

            AssertViolatingOccurrences(violatingOccurrences, anotherStructure, 
                $"{targetNamespace}.{targetStructureName}");
        }   

        [Fact(DisplayName = "ArchitecturalRuleHelper GetViolatingOccurrences - Success: OnlyCanRelate relation type")]
        [Trait(nameof(ArchitecturalRuleHelper.GetViolatingOccurrences), "Success_OnlyCanRelate")]
        public void GetViolatingOccurrences_Success_OnlyCanRelate()
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

            var violatingOccurrences = ArchitecturalRuleHelper.GetViolatingOccurrences(
                architectureRule, layersNamespaces, structures);

            AssertViolatingOccurrences(violatingOccurrences, anotherStructure, 
                $"{targetNamespace}.{targetStructureName}");
        }    

        [Fact(DisplayName = "ArchitecturalRuleHelper GetViolatingOccurrences - Success: CanNotRelate relation type")]
        [Trait(nameof(ArchitecturalRuleHelper.GetViolatingOccurrences), "Success_CanNotRelate")]
        public void GetViolatingOccurrences_Success_CanNotRelate()
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

            var violatingOccurrences = ArchitecturalRuleHelper.GetViolatingOccurrences(
                architectureRule, layersNamespaces, structures);

            AssertViolatingOccurrences(violatingOccurrences, originStructure, 
                $"{targetNamespace}.{targetStructureName}");
        }    

        [Fact(DisplayName = "ArchitecturalRuleHelper GetViolatingOccurrences - Success: Empty")]
        [Trait(nameof(ArchitecturalRuleHelper.GetViolatingOccurrences), "Success_Empty")]
        public void GetViolatingOccurrences_Success_Empty()
        {
            var architecturalRules = new ArchitecturalRule[]
            {
                new ArchitecturalRule()
                {
                    OriginLayer = "X",
                    TargetLayer = "Y",
                    RuleOperator = RuleOperator.CanNotRelate,
                    RelationTypes = new List<RelationType>()
                        { RelationType.Declarate }
                },
                new ArchitecturalRule()
                {
                    OriginLayer = "X",
                    TargetLayer = "Z",
                    RuleOperator = RuleOperator.OnlyCanRelate,
                    RelationTypes = new List<RelationType>()
                        { RelationType.Inheritance }
                },
                new ArchitecturalRule()
                {
                    OriginLayer = "W",
                    TargetLayer = "Z",
                    RuleOperator = RuleOperator.OnlyNeedToRelate,
                    RelationTypes = new List<RelationType>()
                        { RelationType.ReceiptByConstructorArgument }
                }
            };
            
            var structures = new List<Structure>()
            {
                new Structure()
                {
                    Name = "XStructure",
                    Type = StructureType.Class,
                    Namespace = "X",
                    Relations = new List<Relation>()
                        { new Relation(RelationType.Inheritance, 
                            "Y", true, "YStructure") }
                },
                new Structure()
                {
                    Name = "YStructure",
                    Type = StructureType.Class,
                    Namespace = "Y",
                    Relations = new List<Relation>()
                        { new Relation(RelationType.Indirect, 
                            "Z", true, "ZStructure") }
                }
            };
            
            var layersNamespaces = new Dictionary<string, IEnumerable<string>>()
            {
                { "X", new List<string>() { "X" } },
                { "Y", new List<string>() { "Y" } },
                { "W", new List<string>() { "W" } },
                { "Z", new List<string>() { "Z" } }
            };

            foreach (var rule in architecturalRules)
            {
                var violatingOccurrences = ArchitecturalRuleHelper
                    .GetViolatingOccurrences(rule, layersNamespaces, structures);

                Assert.Empty(violatingOccurrences);
            }
        }
        
        private void AssertViolatingOccurrences(
            IEnumerable<ArchitecturalViolationOccurrence> violatingOccurrences,
            Structure structureWithViolation, string targetNameViolation) 
        {
            Assert.NotNull(violatingOccurrences);
            Assert.Single(violatingOccurrences);
            
            var violatingOccurrence = violatingOccurrences.Single();

            Assert.Equal($"{structureWithViolation.Namespace}.{structureWithViolation.Name}", 
                violatingOccurrence.Structure);

            Assert.Single(violatingOccurrence.NonConformingRelations);
            
            var nonConformingRelation = violatingOccurrence.NonConformingRelations.Single();

            Assert.Equal(structureWithViolation.Relations.Single().RelationType,
                nonConformingRelation.RelationType);

            Assert.Single(nonConformingRelation.Targets);
            Assert.Equal(targetNameViolation, nonConformingRelation.Targets.Single());
        }
    }
}