using System.Text.RegularExpressions;

namespace ErosionFinder.Domain.Models
{
    /// <summary>
    /// Represents the grouping of the namespaces by regular expression
    /// </summary>
    public class NamespacesRegularExpressionGrouped : NamespacesGroupingMethod
    {
        /// <summary>
        /// Regular expression which will define the namespaces
        /// </summary>
        public Regex NamespaceRegexPattern { get; set; }
    }
}