using System.Collections.Generic;
using System.Linq;

namespace ErosionFinder.Data.Models
{
    /// <summary>
    /// Class that contains the conformance check output
    /// </summary>
    public class ArchitecturalConformanceCheck
    {
        /// <summary>
        /// Solution name
        /// </summary>
        public string SolutionName { get; set; }

        /// <summary>
        /// Defined architectural rules which were followed
        /// </summary>
        public IEnumerable<ArchitecturalRule> FollowedRules { get; set; }
             = Enumerable.Empty<ArchitecturalRule>();

        /// <summary>
        /// Defined architectural rules which were transgressed
        /// </summary>
        public IEnumerable<TransgressedRule> TransgressedRules  { get; set; }
             = Enumerable.Empty<TransgressedRule>();
    }    
}