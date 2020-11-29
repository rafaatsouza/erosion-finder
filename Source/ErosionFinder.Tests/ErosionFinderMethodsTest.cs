using ErosionFinder;
using ErosionFinder.Data.Exceptions;
using ErosionFinder.Data.Exceptions.Base;
using ErosionFinder.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ErosionFinder.Tests
{
    public class ErosionFinderMethodsTest
    {
        [Fact]
        [Trait(nameof(ErosionFinderMethods.GetViolationsBySolutionFilePathAndConstraintsAsync), "Error_InvalidSolutionFilePath")]
        public async Task GetViolationsBySolutionFilePathAndConstraintsAsync_Error_InvalidSolutionFilePath()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await ErosionFinderMethods
                    .GetViolationsBySolutionFilePathAndConstraintsAsync(
                        "", null, default);
            });
        }

        [Fact]
        [Trait(nameof(ErosionFinderMethods.GetViolationsBySolutionFilePathAndConstraintsAsync), "Error_SolutionNotFound")]
        public async Task GetViolationsBySolutionFilePathAndConstraintsAsync_Error_SolutionNotFound()
        {
            var testFileName = $"{Guid.NewGuid().ToString()}.sln";
            
            var result = await Assert.ThrowsAsync<SolutionException>(async () =>
            {
                await ErosionFinderMethods
                    .GetViolationsBySolutionFilePathAndConstraintsAsync(
                        testFileName, null, default);
            });

            Assert.Equal(SolutionError.SolutionFileNotFound.Key, result.Key);
        }

        [Fact]
        [Trait(nameof(ErosionFinderMethods.GetViolationsBySolutionFilePathAndConstraintsAsync), "Error_ConstraintNull")]
        public async Task GetViolationsBySolutionFilePathAndConstraintsAsync_Error_ConstraintNull()
        {
            await ExecuteTestWithSolutionTestFile(async (testFileName) => 
            {
                var result = await Assert.ThrowsAsync<ConstraintsException>(async () =>
                {
                    await ErosionFinderMethods
                        .GetViolationsBySolutionFilePathAndConstraintsAsync(
                            testFileName, null, default);
                });

                Assert.Equal(ConstraintsError.ConstraintsNullOrEmpty.Key, result.Key);
            });
        }

        [Fact]
        [Trait(nameof(ErosionFinderMethods.GetViolationsBySolutionFilePathAndConstraintsAsync), "Error_ConstraintEmpty")]
        public async Task GetViolationsBySolutionFilePathAndConstraintsAsync_Error_ConstraintEmpty()
        {
            await ExecuteTestWithSolutionTestFile(async (testFileName) => 
            {
                var constraints = new ArchitecturalConstraints();

                var result = await Assert.ThrowsAsync<ConstraintsException>(async () =>
                {
                    await ErosionFinderMethods
                        .GetViolationsBySolutionFilePathAndConstraintsAsync(
                            testFileName, constraints, default);
                });

                Assert.Equal(ConstraintsError.ConstraintsNullOrEmpty.Key, result.Key);
            });
        }

        [Fact]
        [Trait(nameof(ErosionFinderMethods.GetViolationsBySolutionFilePathAndConstraintsAsync), "Error_LayersNotDefined")]
        public async Task GetViolationsBySolutionFilePathAndConstraintsAsync_Error_LayersNotDefined()
        {
            await ExecuteTestWithSolutionTestFile(async (testFileName) => 
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

                var result = await Assert.ThrowsAsync<ConstraintsException>(async () =>
                {
                    await ErosionFinderMethods
                        .GetViolationsBySolutionFilePathAndConstraintsAsync(
                            testFileName, constraints, default);
                });

                Assert.Equal(ConstraintsError.LayerOfRuleNotDefined.Key, result.Key);
            });
        }

        [Fact]
        [Trait(nameof(ErosionFinderMethods.GetViolationsBySolutionFilePathAndConstraintsAsync), "Error_NamespaceNotFoundForLayer")]
        public async Task GetViolationsBySolutionFilePathAndConstraintsAsync_Error_NamespaceNotFoundForLayer()
        {
            await ExecuteTestWithSolutionTestFile(async (testFileName) => 
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

                var result = await Assert.ThrowsAsync<ConstraintsException>(async () =>
                {
                    await ErosionFinderMethods
                        .GetViolationsBySolutionFilePathAndConstraintsAsync(
                            testFileName, constraints, default);
                });

                Assert.Equal(nameof(ConstraintsError.NamespaceNotFoundForLayer), result.Key);
            });
        }

        private async Task ExecuteTestWithSolutionTestFile(Func<string, Task> func)
        {
            var testFileName = $"{Guid.NewGuid().ToString()}.sln";
            var fileInfo = new FileInfo(testFileName);
            
            using(var fileStream = File.Create(Path.Combine(testFileName)))
            {
                await func(testFileName);
            }

            try { fileInfo.Delete(); } catch { } 
        }
    }
}
