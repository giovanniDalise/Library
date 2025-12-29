using Library.BookService.Infrastructure.DTO.REST.Book;

namespace Library.BookService.Infrastructure.DTO.REST.Author
{
    public class AuthorResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
