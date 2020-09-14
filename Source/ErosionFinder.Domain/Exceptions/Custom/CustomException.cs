using System;

namespace ErosionFinder.Domain.Exceptions.Custom
{
    public abstract class CustomException : Exception
    {
        public string Key { get; }

        protected CustomException(string key, string message) : base(message) 
        {
            Key = key;
        }
    }

    public abstract class CustomException<T> : CustomException where T : CustomError
    {
        protected CustomException(CustomError error) : base(error.Key, error.Message) { }
    }
}