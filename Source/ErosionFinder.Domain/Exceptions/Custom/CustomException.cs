using System;

namespace ErosionFinder.Domain.Exceptions.Custom
{
    public abstract class CustomException : Exception
    {
        protected CustomException(string message) : base(message) { }
    }

    public abstract class CustomException<T> : CustomException where T : CustomError
    {
        protected CustomException(CustomError error) : base(error.Message) { }
    }
}