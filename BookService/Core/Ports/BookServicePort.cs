using Library.BookService.Core.Domain.Models;
using Library.BookService.Infrastructure.DTO.REST.Book;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.BookService.Core.Ports
{
    public interface BookServicePort
    {
        Task<(List<Book> Books, int TotalRecords)> GetAllBooksAsync(int page, int pageSize);
        Task<Book> GetBookByIdAsync(long id);
        Task<Book> CreateBookAsync(Book book);
        Task<long> UpdateBookAsync(long id, Book book);
        Task<long> DeleteBookAsync(long id);
        Task<(List<Book> Books, int TotalRecords)> GetBooksByTextAsync(string searchText, int page, int pagesize);
        Task<List<Book>> GetBooksByObjectAsync(Book searchBook);
    }
}
