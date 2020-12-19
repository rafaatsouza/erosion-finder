using ErosionFinder.Data.Exceptions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ErosionFinder.Tests
{
    public class ErosionFinderMethodsTest
    {
        [Fact(DisplayName = "ErosionFinderMethods GetViolationsBySolutionFilePathAndConstraintsAsync - Error - Invalid solution file path")]
        [Trait(nameof(ErosionFinderMethods.GetViolationsBySolutionFilePathAndConstraintsAsync), "Error_InvalidSolutionFilePath")]
        public async Task GetViolationsBySolutionFilePathAndConstraintsAsync_Error_InvalidSolutionFilePath()
        {
            var result = await Assert.ThrowsAsync<SolutionException>(async () =>
            {
                await ErosionFinderMethods
                    .GetViolationsBySolutionFilePathAndConstraintsAsync(
                        "", null, default);
            });

            Assert.Equal(SolutionError.SolutionFileNotFound.Key, result.Key);
        }

        [Fact(DisplayName = "ErosionFinderMethods GetViolationsBySolutionFilePathAndConstraintsAsync - Error - Solution not found")]
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
    }
}
