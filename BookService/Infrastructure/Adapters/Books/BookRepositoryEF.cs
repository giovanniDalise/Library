using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports.Books;
using Library.BookService.Infrastructure.exceptions;
using Library.BookService.Infrastructure.Persistence.EF;
using Library.BookService.Infrastructure.Persistence.EF.Entities;
using Library.BookService.Infrastructure.Persistence.EF.Mappers;
using Library.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Library.BookService.Infrastructure.Adapters.Books
{
    public class BookRepositoryEF : IBookRepositoryPort
    {
        private readonly BookEntityMapper _bookMapper;
        private readonly BookDBContext _context;
        private readonly ILoggerPort _logger;

        public BookRepositoryEF(
            BookEntityMapper bookMapper,
            BookDBContext context,
            ILoggerPort logger)
        {
            _bookMapper = bookMapper;
            _context = context;
            _logger = logger;
        }

        public async Task<Book> GetBookDetailAsync(long id)
        {
            _logger.Info($"GetBookDetailAsync - Started | Retrieving book ID {id}");
            try
            {
                var bookEntity = await _context.Books
                                                .Include(b => b.Editor)
                                                .Include(b => b.Authors)
                                                .FirstOrDefaultAsync(b => b.Id == id);
                return bookEntity != null ? _bookMapper.ToDomain(bookEntity) : null;
            }
            catch (Exception e)
            {
                _logger.Error($"Errore GetBookDetailAsync ID {id}", e);
                throw new BookRepositoryEFException("Error getting book by id: " + e.Message);
            }
        }
        public async Task<Book> CreateBookAsync(Book book)
        {
            try
            {
                var editorEntity = await _context.Editors
                    .FirstOrDefaultAsync(e => e.Id == book.Editor.Id);
                if (editorEntity == null)
                    throw new BookRepositoryEFException($"Editor non trovato con ID {book.Editor.Id}");

                var authorEntities = await _context.Authors
                    .Where(a => book.Authors.Select(x => x.Id).Contains(a.Id))
                    .ToListAsync();
                if (authorEntities.Count != book.Authors.Count)
                    throw new BookRepositoryEFException("Uno o più autori non trovati");

                var bookEntity = new BookEntity
                {
                    Title = book.Title,
                    Isbn = book.Isbn,
                    CoverReference = book.CoverReference,
                    Editor = editorEntity,
                    Authors = authorEntities
                };

                await _context.Books.AddAsync(bookEntity);
                await _context.SaveChangesAsync();

                _logger.Info($"Book ID {bookEntity.Id} created");
                return _bookMapper.ToDomain(bookEntity);
            }
            catch (BookRepositoryEFException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.Error($"Error CreateBookAsync for book {book.Title}", e);
                throw new BookRepositoryEFException("Error creating book: " + e.Message);
            }
        }

        public async Task<long> DeleteBookAsync(long id)
        {
            _logger.Info($"DeleteBookAsync - Start | Deleting Book ID {id}");
            try
            {
                var bookEntity = await _context.Books
                    .Include(b => b.Authors) // Includi gli autori per eliminare le relazioni Many-to-Many
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (bookEntity == null)
                {
                    _logger.Warn($"Tentativo delete libro inesistente ID {id}");
                    throw new BookRepositoryEFException($"Book not found with id {id}");
                }

                // Rimuovi le relazioni nella tabella books_authors
                bookEntity.Authors.Clear();
                await _context.SaveChangesAsync();

                // Ora puoi rimuovere il libro
                _context.Books.Remove(bookEntity);
                await _context.SaveChangesAsync();

                _logger.Info($"Libro eliminato ID {id}");
                return id;
            }
            catch (Exception e)
            {
                _logger.Error($"Errore DeleteBookAsync ID {id}", e);
                throw new BookRepositoryEFException($"Error deleting book: {e.Message}", e);
            }
        }

        public async Task<long> UpdateBookAsync(long id, Book book)
        {
            _logger.Info($"UpdateBookAsync - Start | Id={id}");
            try
            {
                var existingEntity = await _context.Books
                    .Include(b => b.Authors)
                    .Include(b => b.Editor)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (existingEntity == null)
                {
                    _logger.Warn($"UpdateBookAsync - Book not found | Id={id}");
                    throw new BookRepositoryEFException("Book not found");
                }

                // Aggiorna campi base
                existingEntity.Title = book.Title;
                existingEntity.Isbn = book.Isbn;
                existingEntity.CoverReference = book.CoverReference;

                // Gestione editor — cerca per id, errore se non esiste
                var editorEntity = await _context.Editors
                    .FirstOrDefaultAsync(e => e.Id == book.Editor.Id);

                if (editorEntity == null)
                {
                    _logger.Warn($"UpdateBookAsync - Editor not found | Id={book.Editor.Id}");
                    throw new BookRepositoryEFException($"Editor non trovato con ID {book.Editor.Id}");
                }
                existingEntity.Editor = editorEntity;

                // Gestione autori — cerca per id, errore se qualcuno non esiste
                var authorEntities = await _context.Authors
                    .Where(a => book.Authors.Select(x => x.Id).Contains(a.Id))
                    .ToListAsync();

                if (authorEntities.Count != book.Authors.Count)
                {
                    _logger.Warn($"UpdateBookAsync - One or more authors not found");
                    throw new BookRepositoryEFException("Uno o più autori non trovati");
                }

                existingEntity.Authors.Clear();
                foreach (var author in authorEntities)
                {
                    existingEntity.Authors.Add(author);
                }
                await _context.SaveChangesAsync();

                _logger.Info($"UpdateBookAsync - Completed | Id={id}");
                return id;
            }
            catch (BookRepositoryEFException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.Error($"UpdateBookAsync - Error | Id={id}", e);
                throw new BookRepositoryEFException("Error updating book: " + e.Message);
            }
        }

        public async Task<(List<Book> Items, int TotalRecords)> GetBooksAsync(Book searchBook, int page, int pageSize)
        {
            _logger.Info($"GetBooksAsync - Start | Title={searchBook.Title ?? "null"} | Isbn={searchBook.Isbn ?? "null"}");

            try
            {
                int offset = (page -1) * pageSize;

                var query = _context.Books
                                    .Include(b => b.Editor)
                                    .Include(b => b.Authors)
                                    .AsQueryable();

                if (searchBook.Id > 0)
                {
                    query = query.Where(b => b.Id == searchBook.Id);
                }

                if (!string.IsNullOrEmpty(searchBook.Title))
                {
                    query = query.Where(b => b.Title.Contains(searchBook.Title));
                }

                if (!string.IsNullOrEmpty(searchBook.Isbn))
                {
                    query = query.Where(b => b.Isbn.Contains(searchBook.Isbn));
                }

                if (searchBook.Editor != null &&
                    !string.IsNullOrEmpty(searchBook.Editor.Name))
                {
                    query = query.Where(b =>
                        b.Editor != null &&
                        b.Editor.Name.Contains(searchBook.Editor.Name));
                }

                if (searchBook.Authors != null && searchBook.Authors.Any())
                {
                    var author = searchBook.Authors.First();

                    query = query.Where(b =>
                        b.Authors.Any(a =>
                            a.Name.Contains(author.Name) ||
                            a.Surname.Contains(author.Surname)
                        ));
                }

                int total = await query.CountAsync();

                var bookEntities = await query
                    .OrderBy(b =>b.Id)
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.Info($"GetBooksAsync - Completed | Results={bookEntities.Count}");

                return (_bookMapper.ToDomainList(bookEntities), total);
            }
            catch (Exception e)
            {
                _logger.Error("GetBooksAsync - Error", e);

                throw new BookRepositoryEFException("Error finding books by object: " + e.Message);
            }
        }
    }
}
