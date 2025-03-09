using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports;
using Library.BookService.Infrastructure.exceptions;
using Library.BookService.Infrastructure.Persistence;
using Library.BookService.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Library.BookService.Infrastructure.Adapters
{
    public class BookRepositoryEF : BookRepositoryPort
    {
        private readonly BookMapper _bookMapper;
        private readonly BookDBContext _context;

        public BookRepositoryEF(BookMapper bookMapper, BookDBContext context)
        {
            _bookMapper = bookMapper;
            _context = context;
        }

        public async Task<long> CreateAsync(Book book)
        {
            try
            {
                var bookEntity = _bookMapper.ToEntity(book);
                await _context.Books.AddAsync(bookEntity);
                await _context.SaveChangesAsync();
                return bookEntity.BookId;
            }
            catch (Exception e)
            {
                throw new BookRepositoryEFException("Error creating book: " + e.Message);
            }
        }

        public async Task<List<Book>> ReadAsync()
        {
            try
            {
                var bookEntities = await _context.Books
                                                  .Include(b => b.Editor)
                                                  .Include(b => b.Authors)
                                                  .ToListAsync();
                return _bookMapper.ToDomainList(bookEntities);
            }
            catch (Exception e)
            {
                throw new BookRepositoryEFException("Error reading books: " + e.Message);
            }
        }

        public async Task<long> DeleteAsync(long id)
        {
            try
            {
                var bookEntity = await _context.Books
                    .Include(b => b.Authors) // Includi gli autori per eliminare le relazioni Many-to-Many
                    .FirstOrDefaultAsync(b => b.BookId == id);

                if (bookEntity == null)
                {
                    throw new BookRepositoryEFException($"Book not found with id {id}");
                }

                // Rimuovi le relazioni nella tabella books_authors
                bookEntity.Authors.Clear();
                await _context.SaveChangesAsync();

                // Ora puoi rimuovere il libro
                _context.Books.Remove(bookEntity);
                await _context.SaveChangesAsync();

                return id;
            }
            catch (Exception e)
            {
                throw new BookRepositoryEFException($"Error deleting book: {e.Message}", e);
            }
        }


        public async Task<long> UpdateAsync(long id, Book book)
        {
            try
            {
                var existingEntity = await _context.Books
                    .Include(b => b.Authors)  // Se hai relazioni, le includi per evitare problemi di tracking
                    .FirstOrDefaultAsync(b => b.BookId == id);

                if (existingEntity == null)
                {
                    throw new BookRepositoryEFException("Book not found");
                }

                // Aggiorna solo i campi necessari senza creare una nuova entità
                existingEntity.Title = book.Title;
                existingEntity.Isbn = book.Isbn;
                // Se gestisci relazioni (many-to-many con Authors)
                existingEntity.Authors.Clear();  // Rimuove le associazioni esistenti
                foreach (var author in book.Authors)
                {
                    var authorEntity = await _context.Authors.FindAsync(author.Id);
                    if (authorEntity != null)
                    {
                        existingEntity.Authors.Add(authorEntity);  // Riaggiunge gli autori esistenti
                    }
                }

                await _context.SaveChangesAsync();
                return id;
            }
            catch (Exception e)
            {
                throw new BookRepositoryEFException("Error updating book: " + e.Message);
            }
        }


        public async Task<Book> GetByIdAsync(long id)
        {
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
                throw new BookRepositoryEFException("Error getting book by id: " + e.Message);
            }
        }

        public async Task<List<Book>> FindByTextAsync(string searchText)
        {
            try
            {
                var query = _context.Books
                                    .Include(b => b.Editor)
                                    .Include(b => b.Authors)
                                    .Where(b => b.Title.Contains(searchText) ||
                                                b.Isbn.Contains(searchText) ||
                                                b.Editor.Name.Contains(searchText) ||
                                                b.Authors.Any(a => a.Name.Contains(searchText) || a.Surname.Contains(searchText)))
                                    .ToListAsync();

                var result = await query;
                return _bookMapper.ToDomainList(result);
            }
            catch (Exception e)
            {
                throw new BookRepositoryEFException("Error finding books by text: " + e.Message);
            }
        }

        public async Task<List<Book>> FindByObjectAsync(Book searchBook)
        {
            try
            {
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

                var bookEntities = await query.ToListAsync();
                return _bookMapper.ToDomainList(bookEntities);
            }
            catch (Exception e)
            {
                throw new BookRepositoryEFException("Error finding books by object: " + e.Message);
            }
        }
    }
}
