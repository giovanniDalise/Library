using Library.BookService.Core.Domain.Models;
using Library.BookService.Infrastructure.DTO.REST.Book;

namespace Library.BookService.Core.Ports
{
    public interface BookRepositoryPort
    {
        Task<(List<Book> Items, int TotalRecords)> ReadAsync(int page, int pageSize);
        Task<Book> GetByIdAsync(long id);
        Task<Book> CreateAsync(Book book);
        Task<long> UpdateAsync(long id, Book book);
        Task<long> DeleteAsync(long id);
        Task<(List<Book> Items, int TotalRecords)> FindByTextAsync(string searchText, int page, int pageSize);
        Task<(List<Book> Items, int TotalRecords)> FindByObjectAsync(Book searchBook, int page, int pageSize);
    }
}
