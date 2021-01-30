using ErosionFinder.Data.Exceptions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ErosionFinder.Tests
{
    public class ErosionFinderMethodsTest
    {
        [Fact(DisplayName = "ErosionFinderMethods CheckArchitecturalConformanceAsync - Error - Invalid solution file path")]
        [Trait(nameof(ErosionFinderMethods.CheckArchitecturalConformanceAsync), "Error_InvalidSolutionFilePath")]
        public async Task CheckArchitecturalConformanceAsync_Error_InvalidSolutionFilePath()
        {
            var result = await Assert.ThrowsAsync<SolutionException>(async () =>
            {
                await ErosionFinderMethods
                    .CheckArchitecturalConformanceAsync("", null, default);
            });

            Assert.Equal(SolutionError.SolutionFileNotFound.Key, result.Key);
        }

        [Fact(DisplayName = "ErosionFinderMethods CheckArchitecturalConformanceAsync - Error - Solution not found")]
        [Trait(nameof(ErosionFinderMethods.CheckArchitecturalConformanceAsync), "Error_SolutionNotFound")]
        public async Task CheckArchitecturalConformanceAsync_Error_SolutionNotFound()
        {
            var testFileName = $"{Guid.NewGuid().ToString()}.sln";
            
            var result = await Assert.ThrowsAsync<SolutionException>(async () =>
            {
                await ErosionFinderMethods
                    .CheckArchitecturalConformanceAsync(testFileName, null, default);
            });

            Assert.Equal(SolutionError.SolutionFileNotFound.Key, result.Key);
        }
    }
}
