using ErosionFinder.Domain.Exceptions.Base;

namespace ErosionFinder.Domain.Exceptions
{
    public class ConstraintCustomException : ErosionFinderException<ConstraintCustomError>
    {
        public ConstraintCustomException(ConstraintCustomError error) : base(error) { }
    }

    public class ConstraintCustomError : ErosionFinderError
    {
        public static ConstraintCustomError NamespaceNotFoundForLayer(string layerName) =>
            new ConstraintCustomError(nameof(NamespaceNotFoundForLayer),
                $"No namespaces were found for the informed layer '{layerName}'");

        public static ConstraintCustomError ConstraintsNullOrEmpty =>
            new ConstraintCustomError(nameof(ConstraintsNullOrEmpty),
                $"The constraints object is null or empty");

        public static ConstraintCustomError LayerOfRuleNotDefined =>
            new ConstraintCustomError(nameof(LayerOfRuleNotDefined),
                $"There are layers in the rules which are not defined");

        protected ConstraintCustomError(string key, string error) : base(key, error) { }
    }
}