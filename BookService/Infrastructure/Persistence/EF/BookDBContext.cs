using Library.BookService.Infrastructure.Persistence.EF.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Library.BookService.Infrastructure.Persistence.EF
{
    public class BookDBContext : DbContext
    {
        public DbSet<BookEntity> Books { get; set; }
        public DbSet<EditorEntity> Editors { get; set; }
        public DbSet<AuthorEntity> Authors { get; set; }

        public BookDBContext(DbContextOptions<BookDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura la relazione Many-to-One tra BookEntity e EditorEntity
            modelBuilder.Entity<BookEntity>()
                .HasOne(book => book.Editor)
                .WithMany(editor => editor.Books)
                .HasForeignKey(book => book.EditorId);

            // Configura la relazione Many-to-Many tra BookEntity e AuthorEntity
            modelBuilder.Entity<BookEntity>()
                .HasMany(book => book.Authors)
                .WithMany(author => author.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "books_authors",
                    j => j.HasOne<AuthorEntity>()
                          .WithMany()
                          .HasForeignKey("author")
                          .OnDelete(DeleteBehavior.Restrict), // Impedisce il cascading delete automatico
                    j => j.HasOne<BookEntity>()
                          .WithMany()
                          .HasForeignKey("book")
                          .OnDelete(DeleteBehavior.Restrict) // Impedisce il cascading delete automatico
                );

            // Configura eventuali vincoli unici o indici
            modelBuilder.Entity<BookEntity>()
                .HasIndex(book => book.Isbn)
                .IsUnique();

            modelBuilder.Entity<AuthorEntity>()
                .HasIndex(author => author.Name)
                .IsUnique(false);
        }

        /// <summary>
        /// Metodo per rimuovere manualmente le relazioni nella tabella books_authors prima di eliminare un libro
        /// </summary>
        public void RemoveBookWithRelationships(int bookId)
        {
            var book = Books.Include(b => b.Authors).FirstOrDefault(b => b.BookId == bookId);

            if (book != null)
            {
                book.Authors.Clear(); // Rimuove i riferimenti nella tabella books_authors
                SaveChanges();

                Books.Remove(book);
                SaveChanges();
            }
        }
    }
}
