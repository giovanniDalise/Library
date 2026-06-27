using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports.Authors;
using Library.Logging.Abstractions;

namespace Library.BookService.Core.Application
{
    public class AuthorAppService : IAuthorAppServicePort
    {
        private readonly IAuthorServicePort _authorDomainService;
        private readonly ILoggerPort _logger;

        public AuthorAppService(IAuthorServicePort authorDomainService, ILoggerPort logger) 
        {
            _authorDomainService = authorDomainService;
            _logger = logger;
        }

        public async Task<(List<Author> Authors, int TotalRecords)> GetAuthorsAsync (Author searchAuthor, int page, int pageSize)
        {
            return await _authorDomainService.GetAuthorsAsync(searchAuthor, page, pageSize);
        }
        public async Task<(Author Author, int TotalRecords)> GetAuthorDetailAsync(long id, int page, int pageSize)
        {
            return await _authorDomainService.GetAuthorDetailAsync(id, page, pageSize);
        }
        public async Task<Author?> GetAuthorByIdAsync(long id)
        {
            return await _authorDomainService.GetAuthorByIdAsync(id);
        }
        public async Task<Author> CreateAuthorAsync(Author author)
        {
            return await _authorDomainService.CreateAuthorAsync(author);
            ;
        }
    }
}
