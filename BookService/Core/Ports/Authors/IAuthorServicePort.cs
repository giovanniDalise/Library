using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports.Authors
{
    public interface IAuthorServicePort
    {
        Task<(List<Author> Authors, int TotalRecords)> GetAuthorsAsync(Author searchAuthor, int page, int pagesize);
        Task<(Author Author, int TotalBooks)> GetAuthorDetailAsync(long id, int page, int pageSize);
        Task<Author?> GetAuthorByIdAsync(long id);
        Task<Author> CreateAuthorAsync(Author author);
        Task<Author> UpdateAuthorAsync(Author author);



    }
}
