namespace Library.IdentityService.Core.Exceptions
{
    public class ProfileImageStorageException : Exception
    {
        public ProfileImageStorageException(string message) : base(message) { }

        public ProfileImageStorageException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
