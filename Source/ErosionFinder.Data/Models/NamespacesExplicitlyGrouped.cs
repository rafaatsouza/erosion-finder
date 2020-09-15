using System.Collections.Generic;

namespace ErosionFinder.Data.Models
{
    /// <summary>
    /// Represents the grouping by listing all the namespaces explicitly
    /// </summary>
    public class NamespacesExplicitlyGrouped : NamespacesGroupingMethod
    {
        /// <summary>
        /// List of namespaces
        /// </summary>
        public IEnumerable<string> Namespaces { get; set; }

        public NamespacesExplicitlyGrouped() { }

        public NamespacesExplicitlyGrouped(IEnumerable<string> namespaces)
        {
            Namespaces = namespaces;
        }
    }
}