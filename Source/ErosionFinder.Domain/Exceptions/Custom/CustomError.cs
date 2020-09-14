namespace ErosionFinder.Domain.Exceptions.Custom
{
    public abstract class CustomError
    {
        public string Key { get; }
        public string Message { get; }

        public CustomError(string key, string error)
        {
            Key = key;
            Message = error;
        }
    }
}