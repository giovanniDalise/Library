using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.BookService.Core.Domain.Services
{
    public class BookService : BookServicePort
    {
        private readonly BookRepositoryPort _bookRepositoryPort;

        public BookService(BookRepositoryPort bookRepositoryPort)
        {
            _bookRepositoryPort = bookRepositoryPort ?? throw new ArgumentNullException(nameof(bookRepositoryPort));
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _bookRepositoryPort.ReadAsync();
        }

        public async Task<Book> GetBookByIdAsync(long id)
        {
            return await _bookRepositoryPort.GetByIdAsync(id);
        }

        public async Task<long> CreateBookAsync(Book book)
        {
            return await _bookRepositoryPort.CreateAsync(book);
        }

        public async Task<long> UpdateBookAsync(long id, Book book)
        {
            return await _bookRepositoryPort.UpdateAsync(id, book);
        }

        public async Task<long> DeleteBookAsync(long id)
        {
            return await _bookRepositoryPort.DeleteAsync(id);
        }

        public async Task<List<Book>> GetBooksByTextAsync(string searchText)
        {
            return await _bookRepositoryPort.FindByTextAsync(searchText);
        }

        public async Task<List<Book>> GetBooksByObjectAsync(Book searchBook)
        {
            return await _bookRepositoryPort.FindByObjectAsync(searchBook);
        }
    }
}
