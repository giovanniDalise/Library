using Library.BookService.Infrastructure.DTO.REST.Author;
using Library.BookService.Infrastructure.DTO.REST.Book;
using Library.BookService.Infrastructure.DTO.REST.Editor;


namespace Library.BookService.Infrastructure.DTO.REST.Mappers
{
    public static class BookDTOMapper
    {
        // Domain → DTO Response
        public static BookResponse ToResponse(Core.Domain.Models.Book book)
        {
            return new BookResponse
            {
                BookId = book.BookId,
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
                }).ToHashSet()
            };
        }

        public static List<BookResponse> ToResponseList(IEnumerable<Core.Domain.Models.Book> books)
        {
            return books.Select(ToResponse).ToList();
        }

        // DTO Request → Domain
        public static Core.Domain.Models.Book ToDomain(BookRequest request, string? coverReference = null)
        {
            return new Core.Domain.Models.Book
            {
                BookId = request.BookId,
                Title = request.Title,
                Isbn = request.Isbn,
                CoverReference = coverReference,
                Editor = request.Editor != null
                    ? new Core.Domain.Models.Editor
                    {
                        Id = request.Editor.Id,
                        Name = request.Editor.Name
                    }
                    : null,
                Authors = request.Authors != null
                    ? request.Authors.Select(a => new Core.Domain.Models.Author
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Surname = a.Surname
                    }).ToHashSet()
                    : new HashSet<Core.Domain.Models.Author>()
            };
        }
    }
}
