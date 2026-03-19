namespace Library.BookService.Core.Domain.Models
{
    public class Book
    {
        public long? BookId { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public Editor Editor { get; set; }
        public HashSet<Author> Authors { get; set; } = new HashSet<Author>();
        public string? CoverReference { get; set; }

    }
}
