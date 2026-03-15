using Library.BookService.Infrastructure.DTO.REST.Books;

namespace Library.BookService.Infrastructure.DTO.REST.Editors
{
    public class EditorDetailResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<string> BookTitles { get; set; }
    }
}
