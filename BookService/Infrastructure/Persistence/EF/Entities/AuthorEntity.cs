using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Library.BookService.Infrastructure.Persistence.EF.Entities
{
    [Table("author")]
    public class AuthorEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("author_id")]
        public long Id { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }

        public virtual ICollection<BookEntity> Books { get; set; }

        public AuthorEntity()
        {
            Books = new HashSet<BookEntity>();
        }

        public AuthorEntity(string name, string surname) : this()
        {
            Name = name;
            Surname = surname;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj is not AuthorEntity other) return false;
            return Name == other.Name && Surname == other.Surname;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public override string ToString()
        {
            return $"{Name} {Surname}";
        }
    }
}
