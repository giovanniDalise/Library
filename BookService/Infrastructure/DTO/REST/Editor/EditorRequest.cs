using Library.BookService.Infrastructure.DTO.REST.Book;

namespace Library.BookService.Infrastructure.DTO.REST.Editor
{
    public class EditorRequest
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public HashSet<BookRequest> Books { get; private set; } = new();
    }
}
