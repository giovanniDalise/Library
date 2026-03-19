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
        public virtual ICollection<BookEntity> Books { get; set; } = new List<BookEntity>();

        public EditorEntity() { }

        public EditorEntity(string name)
        {
            Name = name;
        }
    }
}
