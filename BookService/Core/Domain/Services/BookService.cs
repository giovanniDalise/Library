using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports.Books;


namespace Library.BookService.Core.Domain.Services
{
    public class BookService : IBookServicePort
    {
        private readonly IBookRepositoryPort _bookRepositoryPort;

        public BookService(IBookRepositoryPort bookRepositoryPort)
        {
            _bookRepositoryPort = bookRepositoryPort ?? throw new ArgumentNullException(nameof(bookRepositoryPort));
        }

        public async Task<Book> GetBookByIdAsync(long id)
        {
            return await _bookRepositoryPort.GetBookByIdAsync(id);
        }


        public async Task<Book> CreateBookAsync(Book book)
        {
            return await _bookRepositoryPort.CreateBookAsync(book);
        }

        public async Task<long> UpdateBookAsync(long id, Book book)
        {
            return await _bookRepositoryPort.UpdateBookAsync(id, book);
        }

        public async Task<long> DeleteBookAsync(long id)
        {
            return await _bookRepositoryPort.DeleteBookAsync(id);
        }

        public async Task<(List<Book> Books, int TotalRecords)> GetBooksAsync(Book searchBook, int page, int pageSize)
        {
            return await _bookRepositoryPort.GetBooksAsync(searchBook, page, pageSize);
        }
    }
}
