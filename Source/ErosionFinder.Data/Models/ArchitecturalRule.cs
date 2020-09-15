namespace ErosionFinder.Data.Models
{
    /// <summary>
    /// Rule about relation between two layers
    /// </summary>
    public class ArchitecturalRule
    {
        /// <summary>
        /// Name of the origin layer 
        /// </summary>
        public string OriginLayer { get; set; }

        /// <summary>
        /// Type of rule
        /// </summary>
        public RuleOperator RuleOperator { get; set; }

        /// <summary>
        /// Name of the target layer
        /// </summary>
        public string TargetLayer { get; set; }
    }
}