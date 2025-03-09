using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports
{
    public interface BookRepositoryPort
    {
        Task<List<Book>> ReadAsync();
        Task<Book> GetByIdAsync(long id);
        Task<long> CreateAsync(Book book);
        Task<long> UpdateAsync(long id, Book book);
        Task<long> DeleteAsync(long id);
        Task<List<Book>> FindByTextAsync(string searchText);
        Task<List<Book>> FindByObjectAsync(Book searchBook);
    }
}
