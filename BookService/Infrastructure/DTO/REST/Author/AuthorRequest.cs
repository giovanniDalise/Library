using Library.BookService.Infrastructure.DTO.REST.Book;

namespace Library.BookService.Infrastructure.DTO.REST.Author
{
    public class AuthorRequest
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public HashSet<BookRequest> Books { get; set; } = new HashSet<BookRequest>();
    }
}
