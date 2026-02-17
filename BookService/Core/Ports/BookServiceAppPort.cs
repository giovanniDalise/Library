using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports
{
    public interface BookAppServicePort
    {
        Task<Book> CreateBookAsync(Book book, Stream? coverStream = null, string? coverFileName = null);
        Task<long> UpdateBookAsync(long id, Book book, Stream? newCoverStream = null, string? newCoverFileName = null);
        Task<long> DeleteBookAsync(long bookId);
        Task<(List<Book> Books, int TotalRecords)> GetBooksAsync(Book searchBook, int page, int pageSize);
        Task<Book?> GetBookByIdAsync(long bookId);
    }
}
