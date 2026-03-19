namespace Library.BookService.Core.Domain.Models
{
    public class Editor
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public HashSet<Book> Books { get; set; } = new();

    }
}
