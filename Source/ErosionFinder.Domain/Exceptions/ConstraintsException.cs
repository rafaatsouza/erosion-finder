using ErosionFinder.Domain.Exceptions.Base;

namespace ErosionFinder.Domain.Exceptions
{
    public class ConstraintsException : ErosionFinderException<ConstraintsError>
    {
        public ConstraintsException(ConstraintsError error) : base(error) { }
    }

    public class ConstraintsError : ErosionFinderError
    {
        public static ConstraintsError NamespaceNotFoundForLayer(string layerName) =>
            new ConstraintsError(nameof(NamespaceNotFoundForLayer),
                $"No namespaces were found for the informed layer '{layerName}'");

        public static ConstraintsError ConstraintsNullOrEmpty =>
            new ConstraintsError(nameof(ConstraintsNullOrEmpty),
                $"The constraints object is null or empty");

        public static ConstraintsError LayerOfRuleNotDefined =>
            new ConstraintsError(nameof(LayerOfRuleNotDefined),
                $"There are layers in the rules which are not defined");

        protected ConstraintsError(string key, string error) : base(key, error) { }
    }
}