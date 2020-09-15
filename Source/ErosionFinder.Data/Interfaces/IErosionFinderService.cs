using ErosionFinder.Data.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ErosionFinder.Data.Interfaces
{
    public interface IErosionFinderService
    {
        /// <summary>
        /// Receives a C# solution path and returns all it's architectural violations.
        /// </summary>
        /// <param name="solutionFilePath">Solution file path</param>
        /// <returns>List of architectural violations</returns>
        Task<IEnumerable<Violation>> GetViolationsBySolutionFilePathAndConstraintsAsync(
            string solutionFilePath, ArchitecturalConstraints constraints, CancellationToken cancellationToken);
    }
}