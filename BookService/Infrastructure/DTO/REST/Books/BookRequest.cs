using Library.BookService.Infrastructure.DTO.REST.Authors;
using Library.BookService.Infrastructure.DTO.REST.Editors;

namespace Library.BookService.Infrastructure.DTO.REST.Books
{
    public class BookRequest
    {
        public long BookId { get; set; }
        public string? Title { get; set; }
        public string? Isbn { get; set; }
        public EditorRequest? Editor { get; set; }
        public List<AuthorRequest> Authors { get; set; } = new List<AuthorRequest>();
        public IFormFile? Cover { get; set; }

    }
}
