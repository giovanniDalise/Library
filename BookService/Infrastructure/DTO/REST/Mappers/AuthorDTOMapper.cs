using Library.BookService.Core.Domain.Models;
using Library.BookService.Infrastructure.DTO.REST.Authors;
using NLog.Filters;

namespace Library.BookService.Infrastructure.DTO.REST.Mappers
{
    public class AuthorDTOMapper
    {
        public static AuthorResponse ToResponse (Author author)
        {
            return new AuthorResponse
            {
                Id = author.Id,
                Name = author.Name,
                Surname = author.Surname
            };
        }

        public static Author ToDomain (AuthorRequest request)
        {
            return new Author
            {
                Id = request.Id,
                Name = request.Name,
                Surname = request.Surname
            };
        }
        public static List<AuthorResponse> ToResponseList (List<Author> authors)
        {
            return authors.Select(ToResponse).ToList();
        }
    }
}
