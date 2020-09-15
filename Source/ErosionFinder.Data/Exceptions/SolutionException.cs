using ErosionFinder.Data.Exceptions.Base;

namespace ErosionFinder.Data.Exceptions
{
    public class SolutionException : ErosionFinderException<SolutionError>
    {
        public SolutionException(SolutionError error) : base(error) { }
    }

    public class SolutionError : ErosionFinderError
    {
        public static SolutionError SolutionFileNotFound =>
            new SolutionError(nameof(SolutionFileNotFound),
                "Informed solution was not found");

        public static SolutionError SolutionWithoutProjects =>
            new SolutionError(nameof(SolutionWithoutProjects), 
                "Informed solution does not contains any project");

        public static SolutionError SolutionWithoutCodeFiles =>
            new SolutionError(nameof(SolutionWithoutCodeFiles), 
                "Informed solution does not have any code files");

        protected SolutionError(string key, string error) : base(key, error) { }
    }
}