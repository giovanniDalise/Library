using Library.UserService.Core.Exceptions;

namespace Library.UserService.Infrastructure.Exceptions
{
    public class UserRepositoryADOException: UserRepositoryException
    {
        public UserRepositoryADOException(string message)
             : base(message)
        {
        }

        public UserRepositoryADOException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
