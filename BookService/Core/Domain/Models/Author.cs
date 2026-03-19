namespace Library.BookService.Core.Domain.Models
{
    public class Author
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public HashSet<Book> Books { get; set; } = new HashSet<Book>();

    }
}
