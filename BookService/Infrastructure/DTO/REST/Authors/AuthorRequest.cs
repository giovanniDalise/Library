using Library.BookService.Infrastructure.DTO.REST.Books;

namespace Library.BookService.Infrastructure.DTO.REST.Authors
{
    public class AuthorRequest
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public List<BookRequest> Books { get; set; } = new List<BookRequest>();
    }
}
