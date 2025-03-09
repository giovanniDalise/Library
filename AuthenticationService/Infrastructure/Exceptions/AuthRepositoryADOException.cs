using Library.AuthenticationService.Core.Exceptions;

namespace Library.AuthenticationService.Infrastructure.Exceptions
{
    public class AuthRepositoryADOException : AuthRepositoryException
    {
        public AuthRepositoryADOException(string message)
            : base(message)
        {
        }
        
        public AuthRepositoryADOException(string message, Exception innerException)
            : base(message, innerException) 
        {
        }
    }
}
