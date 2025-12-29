using Library.BookService.Core.Domain.Models;
using Library.BookService.Infrastructure.DTO.REST.Author;
using Library.BookService.Infrastructure.DTO.REST.Editor;

namespace Library.BookService.Infrastructure.DTO.REST.Book
{
    public class BookResponse
    {
        public long? BookId { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public EditorResponse Editor { get; set; }
        public HashSet<AuthorResponse> Authors { get; set; } = new HashSet<AuthorResponse>();
        public string? CoverReference { get; set; }
    }
}
