using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports.Authors;

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
        public async Task<(Author Author, int TotalBooks)> GetAuthorDetailAsync(long id, int page, int pageSize)
        {
            return await _authorRepositoryPort.GetAuthorDetailAsync(id, page, pageSize);
        }
        public async Task<Author> CreateAuthorAsync(Author author)
        {
            return await _authorRepositoryPort.CreateAuthorAsync(author);
        }
        public async Task<Author?> GetAuthorByIdAsync(long id)
        {
            return await _authorRepositoryPort.GetAuthorByIdAsync(id);
        }
        public async Task<Author> UpdateAuthorAsync(Author author)
        {
            return await _authorRepositoryPort.UpdateAuthorAsync(author);
        }
        public async Task DeleteAuthorAsync(long id)
        {
            await _authorRepositoryPort.DeleteAuthorAsync(id);
        }
    }
}
