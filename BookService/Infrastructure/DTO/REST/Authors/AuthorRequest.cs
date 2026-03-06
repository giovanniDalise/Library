using Library.BookService.Infrastructure.DTO.REST.Books;

namespace Library.BookService.Infrastructure.DTO.REST.Authors
{
    public class AuthorRequest
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public HashSet<BookRequest> Books { get; set; } = new HashSet<BookRequest>();
    }
}
