using System.Collections.Generic;

namespace ErosionFinder.Data.Models
{
    /// <summary>
    /// Class that represents the "relation with" of 
    /// the some architectural violation
    /// </summary>
    public class NonConformingRelation
    {
        /// <summary>
        /// List of not allowed related components
        /// </summary>
        public IEnumerable<string> Targets { get; set; }

        /// <summary>
        /// Type of the relation
        /// </summary>
        public RelationType RelationType { get; set; }
    }
}