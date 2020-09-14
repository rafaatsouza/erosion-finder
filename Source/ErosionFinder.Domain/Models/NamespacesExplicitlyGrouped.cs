using System.Collections.Generic;

namespace ErosionFinder.Domain.Models
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
    }
}