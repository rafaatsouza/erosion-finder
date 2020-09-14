using System.Collections.Generic;

namespace ErosionFinder.Domain.Models
{
    /// <summary>
    /// Class that represents a C# structure, a component that is written in some .cs file
    /// </summary>
    public class Structure
    {
        /// <summary>
        /// Name of the structure
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of the structure
        /// </summary>
        public StructureType Type { get; set; }

        /// <summary>
        /// Namespace from the structure
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// List of references of the structure (the 'using' list)
        /// </summary>
        public IEnumerable<string> References { get; set; }

        /// <summary>
        /// List of all relations retrieved from this structure
        /// </summary>
        public IEnumerable<Relation> Relations { get; set; }
    }
}