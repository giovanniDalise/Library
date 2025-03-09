using System.Collections.Generic;
using System.Threading.Tasks;
using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports
{
    public interface BookServicePort
    {
        Task<List<Book>> GetAllBooksAsync();
        Task<Book> GetBookByIdAsync(long id);
        Task<long> CreateBookAsync(Book book);
        Task<long> UpdateBookAsync(long id, Book book);
        Task<long> DeleteBookAsync(long id);
        Task<List<Book>> GetBooksByTextAsync(string searchText);
        Task<List<Book>> GetBooksByObjectAsync(Book searchBook);
    }
}
