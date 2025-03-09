namespace Library.BookService.Core.Exceptions
{
    public class BookRepositoryException : Exception
    {
        public BookRepositoryException(string message)
            : base(message)
        {
        }

        public BookRepositoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
