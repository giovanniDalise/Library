namespace Library.BookService.Core.Domain.Models
{
    public class Author
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public HashSet<Book> Books { get; set; } = new HashSet<Book>();

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj is not Author other) return false;
            return Name == other.Name && Surname == other.Surname;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Surname);
        }

        public override string ToString()
        {
            return $"{Name} {Surname}";
        }
    }
}
