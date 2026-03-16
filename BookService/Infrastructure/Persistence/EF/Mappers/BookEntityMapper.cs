using Library.BookService.Core.Domain.Models;
using Library.BookService.Infrastructure.Persistence.EF.Entities;
using Library.BookService.Infrastructure.Persistence.Interfaces;
using System.Linq;

namespace Library.BookService.Infrastructure.Persistence.EF.Mappers
{
    public class BookEntityMapper : IMapper<BookEntity, Book>
    {
        private readonly EditorEntityMapper _editorMapper = new EditorEntityMapper();

        public Book ToDomain(BookEntity entity)
        {
            return new Book
            {
                BookId = entity.BookId,
                Title = entity.Title,
                Isbn = entity.Isbn,
                Editor = _editorMapper.ToDomain(entity.Editor),
                Authors = AuthorEntityMapper.ToDomainSet(entity.Authors),
                CoverReference = entity.CoverReference
            };
        }

        public BookEntity ToEntity(Book book)
        {
            return new BookEntity
            {
                BookId = book.BookId ?? 0,
                Title = book.Title,
                Isbn = book.Isbn,
                Editor = _editorMapper.ToEntity(book.Editor),
                Authors = AuthorEntityMapper.ToEntitySet(book.Authors),
                CoverReference = book.CoverReference ?? null
            };
        }

        public List<Book> ToDomainList(List<BookEntity> entities)
        {
            return entities.Select((entity, index) => ToDomain(entity)).ToList();
        }

        public List<BookEntity> ToEntityList(List<Book> books)
        {
            var bookMapper = new BookEntityMapper(); // Crea un'istanza di BookMapper
            return books.Select(book => bookMapper.ToEntity(book)).ToList();
        }

    }
}
