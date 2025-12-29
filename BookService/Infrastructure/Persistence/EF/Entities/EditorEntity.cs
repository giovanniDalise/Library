using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Library.BookService.Infrastructure.Persistence.EF.Entities
{
    [Table("editor")]
    public class EditorEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("editor_id")]
        public long Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        // Relazione uno-a-molti con BookEntity
        public virtual ICollection<BookEntity> Books { get; set; } = new HashSet<BookEntity>();

        public EditorEntity() { }

        public EditorEntity(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;

            var editor = (EditorEntity)obj;
            return Name == editor.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
