using Library.BookService.Infrastructure.DTO.REST.Books;

namespace Library.BookService.Infrastructure.DTO.REST.Editors
{
    public class EditorRequest
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public HashSet<BookRequest> Books { get; private set; } = new();
    }
}
