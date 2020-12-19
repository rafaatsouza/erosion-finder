using System.Collections.Generic;
using System.Linq;

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
        public IList<string> Components { get; set; }
            = new List<string>();

        /// <summary>
        /// Namespace
        /// </summary>
        public string Target { get; set; }

        public Relation(RelationType relationType, 
            string target, bool isFromSource)
        {
            RelationType = relationType;
            Target = target;
            TargetFromSource = isFromSource;
        }

        public Relation(RelationType relationType, string target, 
            bool isFromSource, params string[] components)
            : this(relationType, target, isFromSource)
        {
            Components = new List<string>(components);
        }
    }
}