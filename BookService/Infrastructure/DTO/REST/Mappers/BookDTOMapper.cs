
using Library.BookService.Core.Domain.Models;
using Library.BookService.Infrastructure.DTO.REST.Authors;
using Library.BookService.Infrastructure.DTO.REST.Books;
using Library.BookService.Infrastructure.DTO.REST.Editors;

namespace Library.BookService.Infrastructure.DTO.REST.Mappers
{
    public static class BookDTOMapper
    {
        // Domain → DTO Response
        public static BookResponse ToResponse(Book book)
        {
            return new BookResponse
            {
                Id = book.Id,
                Title = book.Title,
                Isbn = book.Isbn,
                CoverReference = book.CoverReference,
                Editor = new EditorResponse
                {
                    Id = book.Editor.Id,
                    Name = book.Editor.Name
                },
                Authors = book.Authors.Select(a => new AuthorResponse
                {
                    Id = a.Id,
                    Name = a.Name,
                    Surname = a.Surname
                }).ToList()
            };
        }

        public static List<BookResponse> ToResponseList(IEnumerable<Book> books)
        {
            return books.Select(ToResponse).ToList();
        }

        // DTO Request → Domain
        public static Book ToDomain(BookRequest request, string? coverReference = null)
        {
            return new Book
            {
                Id = request.Id,
                Title = request.Title,
                Isbn = request.Isbn,
                CoverReference = coverReference,
                Editor = request.Editor != null
                    ? new Editor
                    {
                        Id = request.Editor.Id,
                        Name = request.Editor.Name
                    }
                    : null,
                Authors = request.Authors != null
                    ? request.Authors.Select(a => new Author
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Surname = a.Surname
                    }).ToList()
                    : new List<Author>()
            };
        }
    }
}
