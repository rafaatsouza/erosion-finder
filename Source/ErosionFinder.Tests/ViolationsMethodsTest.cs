using ErosionFinder.Data.Exceptions;
using ErosionFinder.Data.Models;
using ErosionFinder.Dtos;
using ErosionFinder.Util;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xunit;

namespace ErosionFinder.Tests
{
    public class ViolationsMethodsTest
    {
        [Fact]
        [Trait(nameof(ViolationsMethods.GetViolations), "Error_NamespaceNotFoundForRegexDefinedLayer")]
        public void GetViolations_Error_NamespaceNotFoundForRegexDefinedLayer()
        {
            var originLayerRegex = new Regex(@"(Test)(.+)(\w*(Origin([s]{1})?)\b)");
            var targetLayerRegex = new Regex(@"(Test)(.+)(\w*(Target([s]{1})?)\b)");

            var constraints = new ArchitecturalConstraints()
            {
                Rules = new List<ArchitecturalRule>()
                {
                    new ArchitecturalRule()
                    {
                        OriginLayer = "Origin",
                        TargetLayer = "Target",
                        RuleOperator = RuleOperator.NeedToRelate
                    }
                },
                Layers = new Dictionary<string, NamespacesGroupingMethod>()
                {
                    { "Origin", new NamespacesRegularExpressionGrouped(originLayerRegex) },
                    { "Target", new NamespacesRegularExpressionGrouped(targetLayerRegex) }
                }
            };

            var codeFiles = new List<CodeFile>()
            {
                GetCodeFileWithSingleClassStructure("Test", "TestNamespace", null, null)
            };

            var result = Assert.Throws<ConstraintsException>(() =>
            {
                ViolationsMethods.GetViolations(constraints, codeFiles, default);
            });

            Assert.Equal(nameof(ConstraintsError.NamespaceNotFoundForLayer), result.Key);
        }
    
        [Theory]
        [InlineData(RuleOperator.NeedToRelate)]
        [InlineData(RuleOperator.OnlyNeedToRelate)]
        [Trait(nameof(ViolationsMethods.GetViolations), "Success_NeedToRelate")]
        public void GetViolations_Success_NeedToRelate(RuleOperator ruleOperator)
        {
            var originLayer = "Origin";
            var targetLayer = "Target";

            var originNamespace = "Test.Origin";
            var targetNamespace = "Test.Target";

            var originStructureName = "OriginTestStructure";
            var targetStructureName = "TargetTestStructure";

            var constraints = new ArchitecturalConstraints()
            {
                Rules = new List<ArchitecturalRule>()
                {
                    new ArchitecturalRule()
                    {
                        OriginLayer = originLayer,
                        TargetLayer = targetLayer,
                        RuleOperator = ruleOperator
                    }
                },
                Layers = new Dictionary<string, NamespacesGroupingMethod>()
                {
                    { originLayer, GetNamespaceGroup(originNamespace) },
                    { targetLayer, GetNamespaceGroup(targetNamespace) }
                }
            };

            var codeFiles = new List<CodeFile>()
            {
                GetCodeFileWithSingleClassStructure(originStructureName, originNamespace, null, null),
                GetCodeFileWithSingleClassStructure(targetStructureName, targetNamespace, null, null)
            };

            var violations = ViolationsMethods.GetViolations(
                constraints, codeFiles, default);

            Assert.NotNull(violations);
            Assert.Single(violations);
            Assert.Single(violations.Single().Structures);
            Assert.Equal(constraints.Rules.Single().RuleOperator, violations.First().Rule.RuleOperator);
            Assert.Equal(originStructureName, violations.First().Structures.Single());

        }    

        [Fact]
        [Trait(nameof(ViolationsMethods.GetViolations), "Success_OnlyCanRelate")]
        public void GetViolations_Success_OnlyCanRelate()
        {
            var originLayer = "Origin";
            var targetLayer = "Target";
            
            var originNamespace = "Test.Origin";
            var targetNamespace = "Test.Target";
            var thirdNamespace = "Test.Third";

            var originStructureName = "OriginTestStructure";
            var targetStructureName = "TargetTestStructure";
            var thirdStructureName = "ThirdTestStructure";

            var constraints = new ArchitecturalConstraints()
            {
                Rules = new List<ArchitecturalRule>()
                {
                    new ArchitecturalRule()
                    {
                        OriginLayer = originLayer,
                        TargetLayer = targetLayer,
                        RuleOperator = RuleOperator.OnlyCanRelate
                    }
                },
                Layers = new Dictionary<string, NamespacesGroupingMethod>()
                {
                    { originLayer, GetNamespaceGroup(originNamespace) },
                    { targetLayer, GetNamespaceGroup(targetNamespace) }
                }
            };

            var thirdCodeFile = GetCodeFileWithSingleClassStructure(
                thirdStructureName, 
                thirdNamespace, 
                new List<string>() { targetNamespace },
                new List<Relation>()
                {
                    new Relation(RelationType.Inheritance, targetNamespace, true)
                });

            var codeFiles = new List<CodeFile>()
            {
                GetCodeFileWithSingleClassStructure(originStructureName, originNamespace, null, null),
                GetCodeFileWithSingleClassStructure(targetStructureName, targetNamespace, null, null),
                thirdCodeFile
            };

            var violations = ViolationsMethods.GetViolations(
                constraints, codeFiles, default);

            Assert.NotNull(violations);
            Assert.Single(violations);
            Assert.Single(violations.Single().Structures);
            Assert.Equal(constraints.Rules.Single().RuleOperator, violations.First().Rule.RuleOperator);
            Assert.Equal(thirdStructureName, violations.First().Structures.Single());
        }    

        [Fact]
        [Trait(nameof(ViolationsMethods.GetViolations), "Success_CanNotRelate")]
        public void GetViolations_Success_CanNotRelate()
        {
            var originLayer = "Origin";
            var targetLayer = "Target";
            
            var originNamespace = "Test.Origin";
            var targetNamespace = "Test.Target";
            
            var originStructureName = "OriginTestStructure";
            var targetStructureName = "TargetTestStructure";
            
            var constraints = new ArchitecturalConstraints()
            {
                Rules = new List<ArchitecturalRule>()
                {
                    new ArchitecturalRule()
                    {
                        OriginLayer = originLayer,
                        TargetLayer = targetLayer,
                        RuleOperator = RuleOperator.CanNotRelate
                    }
                },
                Layers = new Dictionary<string, NamespacesGroupingMethod>()
                {
                    { originLayer, GetNamespaceGroup(originNamespace) },
                    { targetLayer, GetNamespaceGroup(targetNamespace) }
                }
            };

            var firstCodeFile = GetCodeFileWithSingleClassStructure(
                originStructureName, 
                originNamespace, 
                new List<string>() { targetNamespace },
                new List<Relation>()
                {
                    new Relation(RelationType.Inheritance, targetNamespace, true)
                });

            var codeFiles = new List<CodeFile>()
            {
                firstCodeFile,
                GetCodeFileWithSingleClassStructure(targetStructureName, targetNamespace, null, null)
            };

            var violations = ViolationsMethods.GetViolations(
                constraints, codeFiles, default);

            Assert.NotNull(violations);
            Assert.Single(violations);
            Assert.Single(violations.Single().Structures);
            Assert.Equal(constraints.Rules.Single().RuleOperator, violations.First().Rule.RuleOperator);
            Assert.Equal(originStructureName, violations.First().Structures.Single());
        }    

        private NamespacesExplicitlyGrouped GetNamespaceGroup(string fullNamespace)
            => new NamespacesExplicitlyGrouped(new List<string>() { fullNamespace });

        private CodeFile GetCodeFileWithSingleClassStructure(string name, string fullNamespace, 
            IEnumerable<string> references, IEnumerable<Relation> relations)
            => new CodeFile()
            {
                Structures = new List<Structure>()
                {
                    new Structure()
                    {
                        Name = name,
                        Type = StructureType.Class,
                        Namespace = fullNamespace,
                        References = references ?? Enumerable.Empty<string>(),
                        Relations = relations ?? Enumerable.Empty<Relation>()
                    }
                }
            };
    }
}
