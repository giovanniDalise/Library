using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports.Authors
{
    public interface IAuthorAppServicePort
    {
        Task<(List<Author> Authors, int TotalRecords)> GetAuthorsAsync(Author searchAuthor, int page, int pageSize);
        Task<(Author Author, int TotalRecords)> GetAuthorByIdAsync(long id, int page, int pageSize);

    }
}
