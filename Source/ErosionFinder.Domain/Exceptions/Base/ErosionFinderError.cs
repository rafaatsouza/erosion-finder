namespace ErosionFinder.Domain.Exceptions.Base
{
    public abstract class ErosionFinderError
    {
        public string Key { get; }
        public string Message { get; }

        public ErosionFinderError(string key, string error)
        {
            Key = key;
            Message = error;
        }
    }
}