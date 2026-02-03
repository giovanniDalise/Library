using Library.BookService.Infrastructure.DTO.REST.Author;
using Library.BookService.Infrastructure.DTO.REST.Book;
using Library.BookService.Infrastructure.DTO.REST.Editor;
using Library.BookService.Core.Domain.Models;


namespace Library.BookService.Infrastructure.DTO.REST
{
    public static class BookDTOMapper
    {
        // Domain → DTO Response
        public static BookResponse ToResponse(Library.BookService.Core.Domain.Models.Book book)
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

        public static List<BookResponse> ToResponseList(IEnumerable<Library.BookService.Core.Domain.Models.Book> books)
        {
            return books.Select(ToResponse).ToList();
        }

        // DTO Request → Domain
        public static Library.BookService.Core.Domain.Models.Book ToDomain(BookRequest request, string? coverReference = null)
        {
            return new Library.BookService.Core.Domain.Models.Book
            {
                BookId = request.BookId,
                Title = request.Title,
                Isbn = request.Isbn,
                CoverReference = coverReference,
                Editor = request.Editor != null
                    ? new Library.BookService.Core.Domain.Models.Editor
                    {
                        Id = request.Editor.Id,
                        Name = request.Editor.Name
                    }
                    : null,
                Authors = request.Authors != null
                    ? request.Authors.Select(a => new Library.BookService.Core.Domain.Models.Author
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Surname = a.Surname
                    }).ToHashSet()
                    : new HashSet<Library.BookService.Core.Domain.Models.Author>()
            };
        }
    }
}
