using Library.BookService.Core.Domain.Models;
using Library.BookService.Infrastructure.DTO.REST.Author;
using Library.BookService.Infrastructure.DTO.REST.Editor;

namespace Library.BookService.Infrastructure.DTO.REST.Book
{
    public class BookRequest
    {
        public long BookId { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public EditorRequest Editor { get; set; }
        public HashSet<AuthorRequest> Authors { get; set; } = new HashSet<AuthorRequest>();
        public IFormFile? Cover { get; set; }

    }
}
