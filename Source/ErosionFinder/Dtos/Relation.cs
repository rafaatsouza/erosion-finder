using ErosionFinder.Data.Models;
using System.Collections.Generic;

namespace ErosionFinder.Dtos
{
    /// <summary>
    /// Class that represents a relation to some namespace
    /// </summary>
    internal class Relation
    {
        /// <summary>
        /// Type of the relation
        /// </summary>
        public RelationType RelationType { get; set; }

        /// <summary>
        /// Flag that indicates if the relation target 
        /// is in the source code
        /// </summary>
        public bool TargetFromSource { get; set; }

        /// <summary>
        /// List of components that "represents" this relation
        /// </summary>
        public ICollection<string> Components { get; set; }

        /// <summary>
        /// Namespace
        /// </summary>
        public string Target { get; set; }

        public Relation(RelationType relationType, string target, bool isFromSource)
        {
            RelationType = relationType;
            Target = target;
            TargetFromSource = isFromSource;
            Components = new List<string>();
        }
    }
}