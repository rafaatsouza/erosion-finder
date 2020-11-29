using ErosionFinder.Data.Exceptions;
using ErosionFinder.Data.Exceptions.Base;
using ErosionFinder.Data.Models;
using ErosionFinder.Dtos;
using ErosionFinder.Util;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace ErosionFinder.Tests
{
    public class ConstraintsAndViolationsMethodsTest
    {
        [Fact]
        [Trait(nameof(ConstraintsAndViolationsMethods.CheckConstraints), "Error_ConstraintNull")]
        public void CheckConstraints_Error_ConstraintNull()
        {
            var result = Assert.Throws<ConstraintsException>(() =>
            {
                ConstraintsAndViolationsMethods.CheckConstraints(null);
            });

            Assert.Equal(ConstraintsError.ConstraintsNullOrEmpty.Key, result.Key);
        }

        [Fact]
        [Trait(nameof(ConstraintsAndViolationsMethods.CheckConstraints), "Error_ConstraintEmpty")]
        public void CheckConstraints_Error_ConstraintEmpty()
        {
            var result = Assert.Throws<ConstraintsException>(() =>
            {
                ConstraintsAndViolationsMethods
                    .CheckConstraints(new ArchitecturalConstraints());
            });

            Assert.Equal(ConstraintsError.ConstraintsNullOrEmpty.Key, result.Key);
        }

        [Fact]
        [Trait(nameof(ConstraintsAndViolationsMethods.CheckConstraints), "Error_LayersNotDefined")]
        public void CheckConstraints_Error_LayersNotDefined()
        {
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
                    { "Services", new NamespacesExplicitlyGrouped() }
                }
            };

            var result = Assert.Throws<ConstraintsException>(() =>
            {
                ConstraintsAndViolationsMethods.CheckConstraints(constraints);
            });

            Assert.Equal(ConstraintsError.LayerOfRuleNotDefined.Key, result.Key);
        }

        [Fact]
        [Trait(nameof(ConstraintsAndViolationsMethods.CheckConstraints), "Error_NamespaceNotFoundForLayer")]
        public void CheckConstraints_Error_NamespaceNotFoundForLayer()
        {
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
                    { "Origin", new NamespacesExplicitlyGrouped() },
                    { "Target", new NamespacesExplicitlyGrouped() }
                }
            };

            var result = Assert.Throws<ConstraintsException>(() =>
            {
                ConstraintsAndViolationsMethods.CheckConstraints(constraints);
            });

            Assert.Equal(nameof(ConstraintsError.NamespaceNotFoundForLayer), result.Key);
        }

        [Fact]
        [Trait(nameof(ConstraintsAndViolationsMethods.GetViolations), "Error_NamespaceNotFoundForRegexDefinedLayer")]
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
                ConstraintsAndViolationsMethods.GetViolations(constraints, codeFiles, default);
            });

            Assert.Equal(nameof(ConstraintsError.NamespaceNotFoundForLayer), result.Key);
        }
    
        [Theory]
        [InlineData(RuleOperator.NeedToRelate)]
        [InlineData(RuleOperator.OnlyNeedToRelate)]
        [Trait(nameof(ConstraintsAndViolationsMethods.GetViolations), "Success_NeedToRelate")]
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

            var violations = ConstraintsAndViolationsMethods.GetViolations(
                constraints, codeFiles, default);

            Assert.NotNull(violations);
            Assert.Single(violations);
            Assert.Single(violations.Single().Structures);
            Assert.Equal(constraints.Rules.Single().RuleOperator, violations.First().Rule.RuleOperator);
            Assert.Equal(originStructureName, violations.First().Structures.Single());

        }    

        [Fact]
        [Trait(nameof(ConstraintsAndViolationsMethods.GetViolations), "Success_OnlyCanRelate")]
        public void GetViolations_Success_OnlyCanRelate()
        {
            var originLayer = "Origin";
            var targetLayer = "Target";
            var thirdLayer = "Third";

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

            var violations = ConstraintsAndViolationsMethods.GetViolations(
                constraints, codeFiles, default);

            Assert.NotNull(violations);
            Assert.Single(violations);
            Assert.Single(violations.Single().Structures);
            Assert.Equal(constraints.Rules.Single().RuleOperator, violations.First().Rule.RuleOperator);
            Assert.Equal(thirdStructureName, violations.First().Structures.Single());
        }    

        [Fact]
        [Trait(nameof(ConstraintsAndViolationsMethods.GetViolations), "Success_CanNotRelate")]
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

            var violations = ConstraintsAndViolationsMethods.GetViolations(
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
                        References = references,
                        Relations = relations
                    }
                }
            };
    }
}
