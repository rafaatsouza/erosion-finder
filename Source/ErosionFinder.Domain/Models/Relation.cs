﻿using System.Collections.Generic;

namespace ErosionFinder.Domain.Models
{
    /// <summary>
    /// Class that represents a relation to some namespace
    /// </summary>
    public class Relation
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