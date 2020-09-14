using System.Collections.Generic;

namespace ErosionFinder.Domain.Models
{
    /// <summary>
    /// Class that represents some violation; i.e. some defined rule which was not followed
    /// </summary>
    public class Violation
    {
        /// <summary>
        /// The defined rule which was not followed
        /// </summary>
        public ArchitecturalRule Rule { get; set; }

        /// <summary>
        /// List of structures names that represent the violated rule
        /// </summary>
        public IEnumerable<string> Structures { get; set; }
    }
}