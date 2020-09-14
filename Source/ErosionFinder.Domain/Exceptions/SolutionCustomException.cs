﻿using ErosionFinder.Domain.Exceptions.Custom;

namespace ErosionFinder.Domain.Exceptions
{
    public class SolutionCustomException : CustomException<SolutionCustomError>
    {
        public SolutionCustomException(SolutionCustomError error) : base(error) { }
    }

    public class SolutionCustomError : CustomError
    {
        public static SolutionCustomError SolutionFileNotFound =>
            new SolutionCustomError(nameof(SolutionFileNotFound),
                "Informed solution was not found");

        public static SolutionCustomError SolutionWithoutProjects =>
            new SolutionCustomError(nameof(SolutionWithoutProjects), 
                "Informed solution does not contains any project");

        public static SolutionCustomError SolutionWithoutCodeFiles =>
            new SolutionCustomError(nameof(SolutionWithoutCodeFiles), 
                "Informed solution does not have any code files");

        protected SolutionCustomError(string key, string error) : base(key, error) { }
    }
}