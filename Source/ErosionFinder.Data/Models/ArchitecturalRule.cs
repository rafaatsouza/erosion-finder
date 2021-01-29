using System.Collections.Generic;
using System.Linq;

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
        
        /// <summary>
        /// List of relation types which 
        /// this rule should be applied to (optional)
        /// </summary>
        public IEnumerable<RelationType> RelationTypes { get; set; }
            = Enumerable.Empty<RelationType>();

        public ArchitecturalRule(string origin, string target, 
            RuleOperator ruleOperator, params RelationType[] types)
        {
            OriginLayer = origin;
            TargetLayer = target;
            RuleOperator = ruleOperator;

            if (types != null && types.Any())
            {
                RelationTypes = types.ToList();
            }
        }
    }
}