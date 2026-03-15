using Library.BookService.Infrastructure.DTO.REST.Books;

namespace Library.BookService.Infrastructure.DTO.REST.Editors
{
    public class EditorDetailResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public PagedResponse<string> Books { get; set; } 
    }
}
