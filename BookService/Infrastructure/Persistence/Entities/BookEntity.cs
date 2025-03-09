using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Library.BookService.Infrastructure.Persistence.Entities
{
    [Table("book")]
    public class BookEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("book_id")]
        public long BookId { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("ISBN")]
        public string Isbn { get; set; }

        // Relazione Many-to-One con EditorEntity
        [Column("editor")] // La colonna nel database è chiamata 'editor'
        public long? EditorId { get; set; }
        public virtual EditorEntity Editor { get; set; }

        // Relazione Many-to-Many con AuthorEntity
        public virtual ICollection<AuthorEntity> Authors { get; set; } = new HashSet<AuthorEntity>();

        public BookEntity() { }

        public BookEntity(long bookId, string title, string isbn, EditorEntity editor, IEnumerable<AuthorEntity> authors)
        {
            BookId = bookId;
            Title = title;
            Isbn = isbn;
            Editor = editor;
            Authors = authors != null ? new HashSet<AuthorEntity>(authors) : new HashSet<AuthorEntity>();
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;

            var book = (BookEntity)obj;
            return BookId == book.BookId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BookId);
        }

        public override string ToString()
        {
            return $"{Title}, {Isbn}, {Editor}, {string.Join(", ", Authors)}";
        }
    }
}
