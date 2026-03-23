using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports.Authors
{
    public interface IAuthorServicePort
    {
        Task<(List<Author> Authors, int TotalRecords)> GetAuthorsAsync(Author searchAuthor, int page, int pagesize);
    }
}
