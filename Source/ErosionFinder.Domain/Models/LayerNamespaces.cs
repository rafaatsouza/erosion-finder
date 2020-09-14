using System.Collections.Generic;

namespace ErosionFinder.Domain.Models
{
    /// <summary>
    /// Class that represents the namespaces evaluation for each layer
    /// </summary>
    public class LayerNamespaces
    {
        /// <summary>
        /// Layer name
        /// </summary>
        public string Layer { get; set; }

        /// <summary>
        /// Namespaces defined in layer
        /// </summary>
        public IEnumerable<string> Namespaces { get; set; }
    }
}