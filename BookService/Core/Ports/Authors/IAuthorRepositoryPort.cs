using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports.Authors
{
    public interface IAuthorRepositoryPort
    {
        Task<(List<Author> Items, int TotalRecords)> GetAuthorsAsync(Author searchAuthor, int page, int pageSize);

    }
}
