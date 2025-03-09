namespace Library.AuthenticationService.Core.Exceptions
{
    public class AuthRepositoryException : Exception
    {
        public AuthRepositoryException(string message)
            : base(message)
        {
        }

        public AuthRepositoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
