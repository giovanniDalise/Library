namespace Library.BookService.Infrastructure.DTO.REST.Books
{
    public class BookRequest
    {
        public string? Title { get; set; }
        public string? Isbn { get; set; }
        public long EditorId { get; set; }
        public List<long> AuthorIds { get; set; } = new List<long>();
        public IFormFile? Cover { get; set; }
        public string? CoverReference { get; set; }
    }
}