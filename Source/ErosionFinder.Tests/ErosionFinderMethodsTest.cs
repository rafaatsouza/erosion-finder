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
    }
}
