using Library.BookService.Core.Exceptions;

namespace Library.BookService.Infrastructure.exceptions
{
 public class BookRepositoryEFException : BookRepositoryException
    {
        public BookRepositoryEFException(string message)
            : base(message) { }

        public BookRepositoryEFException(string message, Exception cause)
            : base(message, cause) { }
    }
}
