using System;

namespace ErosionFinder.Data.Exceptions.Base
{
    public abstract class ErosionFinderException : Exception
    {
        public string Key { get; }

        protected ErosionFinderException(string key, string message) : base(message) 
        {
            Key = key;
        }
    }

    public abstract class ErosionFinderException<T> : ErosionFinderException where T : ErosionFinderError
    {
        protected ErosionFinderException(ErosionFinderError error) : base(error.Key, error.Message) { }
    }
}