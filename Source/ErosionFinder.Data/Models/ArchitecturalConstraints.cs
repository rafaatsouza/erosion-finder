using System.Collections.Generic;

namespace ErosionFinder.Data.Models
{
    /// <summary>
    /// Set of architectural layers and rules that defines the constraints
    /// </summary>
    public class ArchitecturalConstraints
    {
        /// <summary>
        /// A layer is just a group of namespaces, 
        /// which can be grouped by many ways. 
        /// Besides the group of namespaces, a layer also has a name.
        /// </summary>
        public IDictionary<string, NamespacesGroupingMethod> Layers { get; set; }

        /// <summary>
        /// Architectural planned rules
        /// </summary>
        public IEnumerable<ArchitecturalRule> Rules { get; set; }
    }
}