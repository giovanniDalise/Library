namespace Library.BookService.Core.Domain.Models
{
    public class Editor
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public HashSet<Book> Books { get; private set; } = new();

        public Editor() { }


        public void SetId(long id)
        {
            Id = id;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not Editor other) return false;
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
