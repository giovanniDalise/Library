using Library.BookService.Infrastructure.DTO.REST.Authors;
using Library.BookService.Infrastructure.DTO.REST.Editors;

namespace Library.BookService.Infrastructure.DTO.REST.Books
{
    public class BookResponse
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public EditorResponse Editor { get; set; }
        public List<AuthorResponse> Authors { get; set; } = new List<AuthorResponse>();
        public string? CoverReference { get; set; }
    }
}
