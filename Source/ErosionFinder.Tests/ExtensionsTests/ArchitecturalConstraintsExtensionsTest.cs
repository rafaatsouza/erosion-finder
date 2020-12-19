using ErosionFinder.Data.Exceptions;
using ErosionFinder.Data.Models;
using System.Collections.Generic;
using Xunit;

namespace ErosionFinder.Extensions.Tests
{
    public class ArchitecturalConstraintsExtensionsTest
    {
        [Fact(DisplayName = "ArchitecturalConstraintsExtensions CheckIfItsValid - Error: Null reference")]
        [Trait(nameof(ArchitecturalConstraintsExtensions.CheckIfItsValid), "Error_NullReference")]
        public void CheckIfItsValid_Error_NullReference()
        {
            ArchitecturalConstraints constraints = null;

            var result = Assert.Throws<ConstraintsException>(() =>
            {
                constraints.CheckIfItsValid();
            });

            Assert.Equal(ConstraintsError.ConstraintsNullOrEmpty.Key, result.Key);
        }

        [Fact(DisplayName = "ArchitecturalConstraintsExtensions CheckIfItsValid - Error: Empty constraint")]
        [Trait(nameof(ArchitecturalConstraintsExtensions.CheckIfItsValid), "Error_ConstraintEmpty")]
        public void CheckIfItsValid_Error_ConstraintEmpty()
        {
            var constraints = new ArchitecturalConstraints();
            var result = Assert.Throws<ConstraintsException>(() =>
            {
                constraints.CheckIfItsValid();
            });

            Assert.Equal(ConstraintsError.ConstraintsNullOrEmpty.Key, result.Key);
        }

        [Fact(DisplayName = "ArchitecturalConstraintsExtensions CheckIfItsValid - Error: Layers not defined")]
        [Trait(nameof(ArchitecturalConstraintsExtensions.CheckIfItsValid), "Error_LayersNotDefined")]
        public void CheckIfItsValid_Error_LayersNotDefined()
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
                    { "Services", new NamespacesExplicitlyGrouped(new List<string>() { "Service" }) }
                }
            };

            var result = Assert.Throws<ConstraintsException>(() =>
            {
                constraints.CheckIfItsValid();
            });

            Assert.Equal(ConstraintsError.LayerOfRuleNotDefined.Key, result.Key);
        }

        [Fact(DisplayName = "ArchitecturalConstraintsExtensions CheckIfItsValid - Error: Namespace not found for layer")]
        [Trait(nameof(ArchitecturalConstraintsExtensions.CheckIfItsValid), "Error_NamespaceNotFoundForLayer")]
        public void CheckIfItsValid_Error_NamespaceNotFoundForLayer()
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
                constraints.CheckIfItsValid();
            });

            Assert.Equal(nameof(ConstraintsError.NamespaceNotFoundForLayer), result.Key);
        }
    }
}
