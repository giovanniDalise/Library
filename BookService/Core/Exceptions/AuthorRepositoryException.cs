namespace Library.BookService.Core.Exceptions
{
    public class AuthorRepositoryException: Exception
    {
        public AuthorRepositoryException(string message)
            : base(message)
        {
        }

        public AuthorRepositoryException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
