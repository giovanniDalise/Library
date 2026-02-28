using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports.Books
{
    public interface BookRepositoryPort
    {
        Task<Book> GetByIdAsync(long id);
        Task<Book> CreateAsync(Book book);
        Task<long> UpdateAsync(long id, Book book);
        Task<long> DeleteAsync(long id);
        Task<(List<Book> Items, int TotalRecords)> ReadAsync(Book searchBook, int page, int pageSize);
    }
}
