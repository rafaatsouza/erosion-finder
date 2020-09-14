using ErosionFinder.Domain.Exceptions.Custom;

namespace ErosionFinder.Domain.Exceptions
{
    public class ConstraintCustomException : CustomException<ConstraintCustomError>
    {
        public ConstraintCustomException(ConstraintCustomError error) : base(error) { }
    }

    public class ConstraintCustomError : CustomError
    {
        public static ConstraintCustomError NamespaceNotFoundForLayer(string layerName) =>
            new ConstraintCustomError(nameof(NamespaceNotFoundForLayer),
                $"No namespaces were found for the informed layer '{layerName}'");

        public static ConstraintCustomError ConstraintFileNullOrEmpty =>
            new ConstraintCustomError(nameof(ConstraintFileNullOrEmpty),
                $"The constraints file is null or empty");

        public static ConstraintCustomError LayerOfRuleNotDefined =>
            new ConstraintCustomError(nameof(LayerOfRuleNotDefined),
                $"There are layers in the rules which are not defined");

        protected ConstraintCustomError(string key, string error) : base(key, error) { }
    }
}