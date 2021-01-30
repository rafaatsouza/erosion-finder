using System.Collections.Generic;
using System.Linq;

namespace ErosionFinder.Data.Models
{
    /// <summary>
    /// Class that represents the occurrence of 
    /// the violation of some architectural rule
    /// </summary>
    public class ArchitecturalViolationOccurrence
    {
        /// <summary>
        /// Rule's origin structure name, containing namespace
        /// </summary>
        public string Structure { get; set; }

        /// <summary>
        /// List of the architectural non-conforming relations that composes 
        /// this violation; It will be empty if it refers to some "need to relate" violations
        /// </summary>
        public IEnumerable<NonConformingRelation> NonConformingRelations { get; set; }
            = Enumerable.Empty<NonConformingRelation>();
    }
}