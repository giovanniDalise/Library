using Library.IdentityService.Core.Exceptions;

namespace Library.IdentityService.Infrastructure.Exceptions
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
