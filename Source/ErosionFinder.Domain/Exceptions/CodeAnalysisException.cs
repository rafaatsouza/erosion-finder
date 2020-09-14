using ErosionFinder.Domain.Exceptions.Base;

namespace ErosionFinder.Domain.Exceptions
{
    public class CodeAnalysisException : ErosionFinderException<CodeAnalysisError>
    {
        public CodeAnalysisException(CodeAnalysisError error) : base(error) { }
    }

    public class CodeAnalysisError : ErosionFinderError
    {
        public static CodeAnalysisError TypeNotResolved(string typeName) =>
            new CodeAnalysisError(nameof(TypeNotResolved), 
                $"Could not resolve the type '{typeName}'");

        public static CodeAnalysisError StructureTypeNotFound(string memberName) =>
            new CodeAnalysisError(nameof(StructureTypeNotFound), 
                $"Could not cast the object {memberName} into a valid structure type");

        protected CodeAnalysisError(string key, string error) : base(key, error) { }
    }
}