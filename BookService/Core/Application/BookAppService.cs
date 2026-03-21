using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports;
using Library.BookService.Core.Ports.Books;
using Library.Logging.Abstractions;

namespace Library.BookService.Core.Application
{
    public class BookAppService : IBookAppServicePort
    {
        private readonly IBookServicePort _bookDomainService;
        private readonly IMediaStoragePort _mediaStorage;
        private readonly ILoggerPort _logger;

        public BookAppService(
            IBookServicePort bookDomainService,
            IMediaStoragePort mediaStorage,
            ILoggerPort logger)
        {
            _bookDomainService = bookDomainService;
            _mediaStorage = mediaStorage;
            _logger = logger;
        }

        public async Task<Book> CreateBookAsync(Book book, Stream? coverStream = null, string? coverFileName = null)
        {
            _logger.Info($"AppService - Creating book: {book.Title}");

            // Chiamo il dominio per creare il libro
            var createdBook = await _bookDomainService.CreateBookAsync(book);

            if (coverStream != null && coverFileName != null)
            {
                var coverUrl = await _mediaStorage.SaveAsync(coverStream, coverFileName, "image/jpeg", createdBook.Id);
                createdBook.CoverReference = coverUrl;
                // aggiorno il libro tramite dominio
                await _bookDomainService.UpdateBookAsync(createdBook.Id.Value, createdBook);
            }

            return createdBook;
        }

        public async Task<long> UpdateBookAsync(long id, Book book, Stream? newCoverStream = null, string? newCoverFileName = null)
        {
            if (newCoverStream != null && newCoverFileName != null && !string.IsNullOrWhiteSpace(book.CoverReference))
            {
                await _mediaStorage.DeleteAsync(id);
                var coverUrl = await _mediaStorage.SaveAsync(newCoverStream, newCoverFileName, "image/jpeg", book.Id);
                book.CoverReference = coverUrl;
            }

            return await _bookDomainService.UpdateBookAsync(id, book);
        }

        public async Task<long> DeleteBookAsync(long bookId)
        {
            var book = await _bookDomainService.GetBookByIdAsync(bookId);
            if (book == null) return -1;

            await _mediaStorage.DeleteAsync(bookId);

            return await _bookDomainService.DeleteBookAsync(bookId);
        }


        public async Task<(List<Book> Books, int TotalRecords)> GetBooksAsync(Book searchBook, int page, int pageSize)
        {
            return await _bookDomainService.GetBooksAsync(searchBook, page, pageSize);
        }

        public async Task<Book?> GetBookByIdAsync(long bookId)
        {
            return await _bookDomainService.GetBookByIdAsync(bookId);
        }
    }


}
