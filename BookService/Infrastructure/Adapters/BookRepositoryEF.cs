using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports;
using Library.BookService.Infrastructure.DTO.REST.Book;
using Library.BookService.Infrastructure.exceptions;
using Library.BookService.Infrastructure.Persistence.EF;
using Library.BookService.Infrastructure.Persistence.EF.Entities;
using Library.BookService.Infrastructure.Persistence.EF.Mappers;
using Library.Logging.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;

namespace Library.BookService.Infrastructure.Adapters
{
    public class BookRepositoryEF : BookRepositoryPort
    {
        private readonly BookMapper _bookMapper;
        private readonly BookDBContext _context;
        private readonly ILoggerPort _logger;

        public BookRepositoryEF(
            BookMapper bookMapper,
            BookDBContext context,
            ILoggerPort logger)
        {
            _bookMapper = bookMapper;
            _context = context;
            _logger = logger;
        }
        public async Task<Book> CreateAsync(Book book)
        {
            _logger.Info($"[Repository] Creazione libro: {book.Title}");
            try
            {
                // Controlla se l'editor esiste
                var editorEntity = await _context.Editors
                    .FirstOrDefaultAsync(e => e.Name == book.Editor.Name);

                // Se non esiste, crealo
                if (editorEntity == null)
                {
                    _logger.Debug($"Editor non trovato, creazione: {book.Editor.Name}");
                    editorEntity = new EditorEntity { Name = book.Editor.Name };
                    await _context.Editors.AddAsync(editorEntity);
                    await _context.SaveChangesAsync(); // salva subito per avere l'ID
                }

                // Gestione autori
                var authorEntities = new List<AuthorEntity>();
                foreach (var author in book.Authors)
                {
                    var authorEntity = await _context.Authors
                        .FirstOrDefaultAsync(a => a.Name == author.Name && a.Surname == author.Surname);

                    if (authorEntity == null)
                    {
                        _logger.Debug($"Autore non trovato, creazione: {author.Name} {author.Surname}");
                        // Se non esiste, crealo
                        authorEntity = new AuthorEntity { Name = author.Name, Surname = author.Surname };
                        await _context.Authors.AddAsync(authorEntity);
                        await _context.SaveChangesAsync(); // salva subito per avere l'ID
                    }

                    authorEntities.Add(authorEntity);
                }

                // Crea la BookEntity con editor e autori esistenti o appena creati
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

                _logger.Info($"Libro creato con ID {bookEntity.BookId}");
                return _bookMapper.ToDomain(bookEntity);
            }
            catch (Exception e)
            {
                _logger.Error($"Errore durante CreateAsync per libro {book.Title}", e);
                throw new BookRepositoryEFException("Error creating book: " + e.Message);
            }
        }
        public async Task<long> DeleteAsync(long id)
        {
            _logger.Info($"[Repository] Eliminazione libro ID {id}");
            try
            {
                var bookEntity = await _context.Books
                    .Include(b => b.Authors) // Includi gli autori per eliminare le relazioni Many-to-Many
                    .FirstOrDefaultAsync(b => b.BookId == id);

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
                _logger.Error($"Errore DeleteAsync ID {id}", e);
                throw new BookRepositoryEFException($"Error deleting book: {e.Message}", e);
            }
        }

        public async Task<long> UpdateAsync(long id, Book book)
        {
            _logger.Info($"UpdateAsync - Start | BookId={id}");
            try
            {
                var existingEntity = await _context.Books
                    .Include(b => b.Authors)
                    .Include(b => b.Editor)
                    .FirstOrDefaultAsync(b => b.BookId == id);

                if (existingEntity == null) 
                {
                    _logger.Warn($"UpdateAsync - Book not found | BookId={id}");
                    throw new BookRepositoryEFException("Book not found");
                }

                // Aggiorna campi base
                existingEntity.Title = book.Title;
                existingEntity.Isbn = book.Isbn;
                existingEntity.CoverReference = book.CoverReference;

                // Gestione editor
                var editorEntity = await _context.Editors
                    .FirstOrDefaultAsync(e => e.Name == book.Editor.Name);

                if (editorEntity == null)
                {
                    _logger.Info($"UpdateAsync - Creating new editor | Name={book.Editor.Name}");

                    editorEntity = new EditorEntity { Name = book.Editor.Name };
                    await _context.Editors.AddAsync(editorEntity);
                    await _context.SaveChangesAsync(); // salva subito per avere l'ID
                }
                existingEntity.Editor = editorEntity;

                _logger.Info($"UpdateAsync - Updating authors | Count={book.Authors.Count}");

                // Gestione autori
                existingEntity.Authors.Clear(); // rimuove le associazioni esistenti
                foreach (var author in book.Authors)
                {
                    var authorEntity = await _context.Authors
                        .FirstOrDefaultAsync(a => a.Name == author.Name && a.Surname == author.Surname);

                    if (authorEntity == null)
                    {
                        _logger.Info($"UpdateAsync - Creating new author | {author.Name} {author.Surname}");
                        authorEntity = new AuthorEntity { Name = author.Name, Surname = author.Surname };
                        await _context.Authors.AddAsync(authorEntity);
                        await _context.SaveChangesAsync(); // salva subito per avere l'ID
                    }

                    existingEntity.Authors.Add(authorEntity);
                }

                await _context.SaveChangesAsync();

                _logger.Info($"UpdateAsync - Completed | BookId={id}");
                return id;
            }
            catch (Exception e)
            {
                _logger.Error($"UpdateAsync - Error | BookId={id}", e);
                throw new BookRepositoryEFException("Error updating book: " + e.Message);
            }
        }

        public async Task<Book> GetByIdAsync(long id)
        {
            _logger.Info($"[Repository] Recupero libro ID {id}");
            try
            {
                var bookEntity = await _context.Books
                                                .Include(b => b.Editor)
                                                .Include(b => b.Authors)
                                                .FirstOrDefaultAsync(b => b.BookId == id);
                return bookEntity != null ? _bookMapper.ToDomain(bookEntity) : null;
            }
            catch (Exception e)
            {
                _logger.Error($"Errore GetByIdAsync ID {id}", e);
                throw new BookRepositoryEFException("Error getting book by id: " + e.Message);
            }
        }

        public async Task<(List<Book> Items, int TotalRecords)> ReadAsync(Book searchBook, int page, int pageSize)
        {
            _logger.Info($"ReadAsync - Start | Title={searchBook.Title ?? "null"} | Isbn={searchBook.Isbn ?? "null"}");

            try
            {
                int offset = (page -1) * pageSize;

                var query = _context.Books
                                    .Include(b => b.Editor)
                                    .Include(b => b.Authors)
                                    .AsQueryable();

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
                            (string.IsNullOrEmpty(author.Name) || a.Name.Contains(author.Name)) &&
                            (string.IsNullOrEmpty(author.Surname) || a.Surname.Contains(author.Surname))
                        ));
                }

                int total = await query.CountAsync();

                var bookEntities = await query
                    .OrderBy(b =>b.BookId)
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.Info($"ReadAsync - Completed | Results={bookEntities.Count}");

                return (_bookMapper.ToDomainList(bookEntities), total);
            }
            catch (Exception e)
            {
                _logger.Error("ReadAsync - Error", e);

                throw new BookRepositoryEFException("Error finding books by object: " + e.Message);
            }
        }
    }
}
