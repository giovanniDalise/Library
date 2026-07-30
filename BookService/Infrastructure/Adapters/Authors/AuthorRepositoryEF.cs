using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports.Authors;
using Library.BookService.Infrastructure.Exceptions;
using Library.BookService.Infrastructure.Persistence.EF;
using Library.BookService.Infrastructure.Persistence.EF.Entities;
using Library.BookService.Infrastructure.Persistence.EF.Mappers;
using Library.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Library.BookService.Infrastructure.Adapters.Authors
{
    public class AuthorRepositoryEF : IAuthorRepositoryPort
    {
        private readonly AuthorEntityMapper _authorMapper;
        private readonly BookDBContext _context;
        private readonly ILoggerPort _logger;

        public AuthorRepositoryEF(
            AuthorEntityMapper authorMapper,
            BookDBContext context,
            ILoggerPort logger)
        {
            _authorMapper = authorMapper;
            _context = context;
            _logger = logger;
        }

        public async Task<(List<Author> Items, int TotalRecords)> GetAuthorsAsync(Author searchAuthor, int page, int pageSize)
        {
            _logger.Info($"GetAuthorsAsync - Started | Author: {searchAuthor.Name} {searchAuthor.Surname}");

            try
            {
                int offset = (page - 1) * pageSize;

                IQueryable<AuthorEntity> query = _context.Authors;

                if (searchAuthor.Id > 0)
                {
                    query = query.Where(a => a.Id == searchAuthor.Id);
                }

                if (!string.IsNullOrEmpty(searchAuthor.Name))
                {
                    query = query.Where(a =>
                        a.Name.Contains(searchAuthor.Name) ||
                        a.Surname.Contains(searchAuthor.Name));
                }

                int total = await query.CountAsync();

                var authorEntities = await query
                    .OrderBy(a => a.Surname).ThenBy(a => a.Name)
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.Info($"GetAuthorsAsync - Completed | {authorEntities.Count} items");

                return (_authorMapper.ToDomainList(authorEntities), total);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetAuthorsAsync - Error", ex);
                throw new AuthorRepositoryEFException("Error retrieving authors", ex);
            }
        }
        public async Task<(Author author, int TotalBooks)> GetAuthorDetailAsync(long id, int page, int pageSize)
        {
            _logger.Info($"GetAuthorDetailAsync - Started | Id: {id}");
            try
            {
                var authorEntity = await _context.Authors
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (authorEntity == null)
                {
                    _logger.Warn($"GetAuthorDetailAsync - Author not found | Id: {id}");
                    return (null, 0);
                }

                int totalBooks = await _context.Books
                    .Where(b => b.Authors.Any(a => a.Id == id))
                    .CountAsync();

                int offset = (page - 1) * pageSize;

                var books = await _context.Books
                    .Where(b => b.Authors.Any(a => a.Id == id))
                    .OrderBy(b => b.Id)
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                authorEntity.Books = books;

                _logger.Info($"GetAuthorDetailAsync - Completed | Author: {authorEntity.Name}, Books: {books.Count}");

                return (_authorMapper.ToDomain(authorEntity), totalBooks);
            }
            catch (Exception e)
            {
                _logger.Error("GetAuthorDetailAsync - Error", e);
                throw new AuthorRepositoryEFException("Error retrieving authors", e);
            }
        }
        public async Task<Author> CreateAuthorAsync (Author author)
        {
            _logger.Info($"CreateAuthorAsync - Started | Creation Author:{author.Name} {author.Surname}");
            try
            {
                var authorEntity = new AuthorEntity
                {
                    Name = author.Name,
                    Surname = author.Surname
                };
                await _context.Authors.AddAsync(authorEntity);
                await _context.SaveChangesAsync();

                _logger.Info($"Author Id {authorEntity.Id} created");
                return _authorMapper.ToDomain(authorEntity);
            }
            catch(Exception e)
            {
                _logger.Error($"Error CreateAuthorAsync for author {author.Name} {author.Surname}", e);
                throw new AuthorRepositoryEFException("Error creating author:" + e.Message);
            }
        }
        public async Task<Author?> GetAuthorByIdAsync(long id)
        {
            _logger.Info($"GetAuthorByIdAsync - Started | Id: {id}");

            try
            {
                var authorEntity = await _context.Authors
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (authorEntity == null)
                {
                    return null;
                }

                _logger.Info($"GetAuthorByIdAsync - Completed | Id: {id}");
                return _authorMapper.ToDomain(authorEntity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetAuthorByIdAsync - Error | Id: {id}", ex);
                throw new AuthorRepositoryEFException("Error retrieving author by id", ex);
            }
        }

        public async Task<Author> UpdateAuthorAsync(Author author)
        {
            _logger.Info($"UpdateAuthorAsync - Started | Id: {author.Id}");
            try
            {
                var authorEntity = await _context.Authors
                    .FirstOrDefaultAsync(a => a.Id == author.Id);

                if (authorEntity == null)
                {
                    _logger.Warn($"UpdateAuthorAsync - Author not found | Id: {author.Id}");
                    throw new AuthorRepositoryEFException($"Author with id {author.Id} not found");
                }

                authorEntity.Name = author.Name;
                authorEntity.Surname = author.Surname;

                await _context.SaveChangesAsync();

                _logger.Info($"UpdateAuthorAsync - Completed | Id: {author.Id}");
                return _authorMapper.ToDomain(authorEntity);
            }
            catch (AuthorRepositoryEFException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateAuthorAsync - Error | Id: {author.Id}", ex);
                throw new AuthorRepositoryEFException("Error updating author", ex);
            }
        }
        public async Task DeleteAuthorAsync(long id)
        {
            _logger.Info($"DeleteAuthorAsync - Start | Id: {id}");

            try
            {
                var authorEntity = await _context.Authors
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (authorEntity == null)
                {
                    _logger.Warn($"DeleteAuthorAsync - Author not found | Id: {id}");
                    throw new AuthorRepositoryEFException($"Author with id {id} not found");
                }

                var hasBooks = await _context.Books
                    .AnyAsync(b => b.Authors.Any(a => a.Id == id));

                if (hasBooks)
                {
                    _logger.Warn($"DeleteAuthorAsync - Author has books | Id: {id}");
                    throw new AuthorRepositoryEFException($"Cannot delete author with id {id} because it has books associated. Please delete the books first.");
                }

                _context.Authors.Remove(authorEntity);
                await _context.SaveChangesAsync();

                _logger.Info($"DeleteAuthorAsync - Completed | Id: {id}");
            }
            catch (AuthorRepositoryEFException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteAuthorAsync - Error | Id: {id}", ex);
                throw new AuthorRepositoryEFException("Error deleting author", ex);
            }
        }
    }
}
