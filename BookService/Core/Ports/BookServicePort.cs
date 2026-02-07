using Library.BookService.Core.Domain.Models;
using Library.BookService.Infrastructure.DTO.REST.Book;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.BookService.Core.Ports
{
    public interface BookServicePort
    {
        Task<Book> CreateBookAsync(Book book);
        Task<long> UpdateBookAsync(long id, Book book);
        Task<long> DeleteBookAsync(long id);
        Task<(List<Book> Books, int TotalRecords)> GetBooksAsync(Book searchBook, int page, int pageSize);
    }
}
