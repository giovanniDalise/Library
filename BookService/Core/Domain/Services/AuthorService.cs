using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports.Authors;
using Library.BookService.Core.Ports.Editors;

namespace Library.BookService.Core.Domain.Services
{
    public class AuthorService :IAuthorServicePort
    {
        private readonly IAuthorRepositoryPort _authorRepositoryPort;

        public AuthorService (IAuthorRepositoryPort authorRepositoryPort)
        {
            _authorRepositoryPort = authorRepositoryPort ?? throw new ArgumentNullException(nameof(authorRepositoryPort));
        }
        public async Task<(List<Author> Authors, int TotalRecords)> GetAuthorsAsync(Author searchAuthor, int page, int pageSize)
        {
            return await _authorRepositoryPort.GetAuthorsAsync(searchAuthor, page, pageSize);
        }
    }
}
