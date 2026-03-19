namespace Library.BookService.Core.Domain.Models
{
    public class Editor
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<Book> Books { get; set; } = new();

    }
}
