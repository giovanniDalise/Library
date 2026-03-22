namespace Library.BookService.Core.Domain.Models
{
    public class Author
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string? SortName { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();

    }
}
