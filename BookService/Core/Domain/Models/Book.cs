using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Domain.Models
{
    public class Book
    {
        public long? BookId { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public Editor Editor { get; set; }
        public HashSet<Author> Authors { get; set; } = new HashSet<Author>();

        public Book() { }

        public Book(long? bookId, string title, string isbn, Editor editor, HashSet<Author> authors)
        {
            BookId = bookId;
            Title = title;
            Isbn = isbn;
            Editor = editor;
            Authors = authors ?? new HashSet<Author>();
        }

        public override bool Equals(object obj)
        {
            if (obj is Book book)
            {
                return BookId == book.BookId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return BookId.GetHashCode();
        }

        public override string ToString()
        {
            return $"Book{{ Title='{Title}', ISBN='{Isbn}', Editor={Editor}, Authors={string.Join(", ", Authors)} }}";
        }
    }
}
