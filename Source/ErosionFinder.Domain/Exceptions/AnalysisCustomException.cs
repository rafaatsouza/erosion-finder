using ErosionFinder.Domain.Exceptions.Custom;

namespace ErosionFinder.Domain.Exceptions
{
    public class AnalysisCustomException : CustomException<AnalysisCustomError>
    {
        public AnalysisCustomException(AnalysisCustomError error) : base(error) { }
    }

    public class AnalysisCustomError : CustomError
    {
        public static AnalysisCustomError TypeNotResolved(string typeName) =>
            new AnalysisCustomError(nameof(TypeNotResolved), 
                $"Could not resolve the type '{typeName}'");

        public static AnalysisCustomError StructureTypeNotFound(string memberName) =>
            new AnalysisCustomError(nameof(StructureTypeNotFound), 
                $"Could not cast the object {memberName} into a valid structure type");

        protected AnalysisCustomError(string key, string error) : base(key, error) { }
    }
}