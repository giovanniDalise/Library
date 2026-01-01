using Library.BookService.Core.Domain.Models;
using Library.BookService.Infrastructure.Persistence.EF.Entities;
using Library.BookService.Infrastructure.Persistence.Interfaces;
using System.Linq;

namespace Library.BookService.Infrastructure.Persistence.EF.Mappers
{
    public class BookMapper : IMapper<BookEntity, Book>
    {
        public Book ToDomain(BookEntity entity)
        {
            return new Book
            {
                BookId = entity.BookId,
                Title = entity.Title,
                Isbn = entity.Isbn,
                Editor = EditorMapper.ToDomain(entity.Editor),
                Authors = AuthorMapper.ToDomainSet(entity.Authors),
                CoverReference = entity.CoverReference
            };
        }


        public BookEntity ToEntity(Book book)
        {
            var entity = new BookEntity
            {
                BookId = book.BookId ?? 0,
                Title = book.Title,
                Isbn = book.Isbn,
                Editor = EditorMapper.ToEntity(book.Editor),
                Authors = AuthorMapper.ToEntitySet(book.Authors),
                CoverReference = book.CoverReference ?? null
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
