namespace Library.BookService.Core.Exceptions
{
    public class EditorRepositoryException:Exception
    {
        public EditorRepositoryException(string message)
            : base(message)
        {
        }

        public EditorRepositoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

