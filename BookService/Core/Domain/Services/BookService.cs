using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports;
using Library.BookService.Infrastructure.DTO.REST.Book;
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

        public async Task<Book> CreateBookAsync(Book book)
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

        public async Task<(List<Book> Books, int TotalRecords)> GetBooksAsync(Book searchBook, int page, int pageSize)
        {
            return await _bookRepositoryPort.ReadAsync(searchBook, page, pageSize);
        }
    }
}
