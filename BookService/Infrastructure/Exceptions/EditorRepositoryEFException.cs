using Library.BookService.Core.Exceptions;

namespace Library.BookService.Infrastructure.Exceptions
{
    public class EditorRepositoryEFException:EditorRepositoryException
    {
        public EditorRepositoryEFException(string message)
            : base(message) { }

        public EditorRepositoryEFException(string message, Exception cause)
            : base(message, cause) { }
    }
}
