using ErosionFinder.Data.Exceptions;
using ErosionFinder.Data.Exceptions.Base;
using ErosionFinder.Data.Models;
using ErosionFinder.Dtos;
using ErosionFinder.Util;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
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
        [Trait(nameof(ConstraintsAndViolationsMethods.GetViolations), "Success")]
        public void GetViolations_Success()
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
                        RuleOperator = RuleOperator.NeedToRelate
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
                GetClassCodeFileWithoutReferencesAndRelations(originStructureName, originNamespace),
                GetClassCodeFileWithoutReferencesAndRelations(targetStructureName, targetNamespace)
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

        private CodeFile GetClassCodeFileWithoutReferencesAndRelations(
            string name, string fullNamespace)
                => new CodeFile()
                {
                    Structures = new List<Structure>()
                    {
                        new Structure()
                        {
                            Name = name,
                            Type = StructureType.Class,
                            Namespace = fullNamespace
                        }
                    }
                };
    }
}
