namespace Library.BookService.Core.Domain.Models
{
    public class Author
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();

    }
}
