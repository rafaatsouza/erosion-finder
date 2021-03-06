﻿using ErosionFinder.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace ErosionFinder.Dtos
{
    /// <summary>
    /// Class that represents a C# structure, a component that is written in some .cs file
    /// </summary>
    internal class Structure
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
            = Enumerable.Empty<string>();

        /// <summary>
        /// List of all relations retrieved from this structure
        /// </summary>
        public IEnumerable<Relation> Relations { get; set; }
            = Enumerable.Empty<Relation>();
    }
}