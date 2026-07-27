using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports.Authors
{
    public interface IAuthorAppServicePort
    {
        Task<(List<Author> Authors, int TotalRecords)> GetAuthorsAsync(Author searchAuthor, int page, int pageSize);
        Task<(Author Author, int TotalRecords)> GetAuthorDetailAsync(long id, int page, int pageSize);
        Task<Author?> GetAuthorByIdAsync(long id);
        Task<Author> CreateAuthorAsync(Author author);
        Task<Author> UpdateAuthorAsync(Author author);
    }
}
