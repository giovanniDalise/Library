using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports.Books
{
    public interface IBookRepositoryPort
    {
        Task<Book> GetBookByIdAsync(long id);
        Task<Book> CreateBookAsync(Book book);
        Task<long> UpdateBookAsync(long id, Book book);
        Task<long> DeleteBookAsync(long id);
        Task<(List<Book> Items, int TotalRecords)> GetBooksAsync(Book searchBook, int page, int pageSize);
    }
}
