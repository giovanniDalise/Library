using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports
{
    public interface BookRepositoryPort
    {
        Task<List<Book>> ReadAsync(int page, int pageSize);
        Task<int> CountAsync();
        Task<Book> GetByIdAsync(long id);
        Task<Book> CreateAsync(Book book);
        Task<long> UpdateAsync(long id, Book book);
        Task<long> DeleteAsync(long id);
        Task<List<Book>> FindByTextAsync(string searchText);
        Task<List<Book>> FindByObjectAsync(Book searchBook);
    }
}
