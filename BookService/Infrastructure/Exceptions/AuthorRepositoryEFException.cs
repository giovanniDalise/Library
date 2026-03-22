using Library.BookService.Core.Exceptions;

namespace Library.BookService.Infrastructure.Exceptions
{
    public class AuthorRepositoryEFException: AuthorRepositoryException
    {
        public AuthorRepositoryEFException(string message)
            : base(message) { }

        public AuthorRepositoryEFException(string message, Exception cause)
            : base(message, cause) { }
    }
}