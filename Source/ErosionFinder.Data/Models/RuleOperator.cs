namespace ErosionFinder.Data.Models
{
    public enum RuleOperator
    {
        /// <summary>
        /// The relation is obligatory
        /// </summary>
        NeedToRelate = 1,

        /// <summary>
        /// The relation is obligatory only for this particular layer
        /// </summary>
        OnlyNeedToRelate = 2,

        /// <summary>
        /// The relation is optional, but only for this particular layer
        /// </summary>
        OnlyCanRelate = 3,

        /// <summary>
        /// The absence of the relation is obligatory
        /// </summary>
        CanNotRelate = 4
    }
}