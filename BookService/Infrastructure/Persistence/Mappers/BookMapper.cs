using Library.BookService.Core.Domain.Models;
using Library.BookService.Infrastructure.Persistence.Entities;
using System.Linq;

namespace Library.BookService.Infrastructure.Persistence.Mappers
{
    public class BookMapper
    {
        public Book ToDomain(BookEntity entity)
        {
            return new Book(
                entity.BookId,
                entity.Title,
                entity.Isbn,
                EditorMapper.ToDomain(entity.Editor),
                AuthorMapper.ToDomainSet(entity.Authors)

            );
        }

        public BookEntity ToEntity(Book book)
        {
            var entity = new BookEntity
            {
                BookId = book.BookId ?? 0, 
                Title = book.Title,
                Isbn = book.Isbn,
                Editor = EditorMapper.ToEntity(book.Editor),
                Authors = AuthorMapper.ToEntitySet(book.Authors)
            };
            return entity;
        }

        public List<Book> ToDomainList(List<BookEntity> entities)
        {
            return entities.Select((entity, index) => ToDomain(entity)).ToList();
        }

        public List<BookEntity> ToEntityList(List<Book> books)
        {
            var bookMapper = new BookMapper(); // Crea un'istanza di BookMapper
            return books.Select(book => bookMapper.ToEntity(book)).ToList();
        }

    }
}
