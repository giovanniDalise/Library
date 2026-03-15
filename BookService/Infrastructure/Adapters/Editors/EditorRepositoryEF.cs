using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports.Editors;
using Library.BookService.Infrastructure.Exceptions;
using Library.BookService.Infrastructure.Persistence.EF;
using Library.BookService.Infrastructure.Persistence.EF.Entities;
using Library.BookService.Infrastructure.Persistence.EF.Mappers;
using Library.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;
using NLog.Filters;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Library.BookService.Infrastructure.Adapters.Editors
{
    public class EditorRepositoryEF : EditorRepositoryPort
    {
        private readonly EditorMapper _editorMapper;
        private readonly BookDBContext _context;
        private readonly ILoggerPort _logger;

        public EditorRepositoryEF(
            EditorMapper editorMapper,
            BookDBContext context,
            ILoggerPort logger)
        {
            _editorMapper = editorMapper;
            _context = context;
            _logger = logger;
        }

        public async Task<(List<Editor> Items, int TotalRecords)> ReadAsync(Editor searchEditor, int page, int pageSize)
        {
            _logger.Info($"Read Async - Started | Editor name: {searchEditor.Name ?? "null"}");
            try
            {
                int offset = (page -1) * pageSize;

                IQueryable<EditorEntity> query = _context.Editors;

                if (searchEditor.Id > 0)
                {
                    query = query.Where(e => e.Id == searchEditor.Id);
                }

                if (!string.IsNullOrEmpty(searchEditor.Name))
                {
                    query = query.Where(e => e.Name.Contains(searchEditor.Name));
                }

                int total = await query.CountAsync();

                var editorEntities = await query
                    .OrderBy(e => e.Id)
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.Info($"ReadAsync - Completed | {editorEntities.Count} items");

                return (_editorMapper.ToDomainList(editorEntities), total);

            }
            catch (Exception e)
            {
                _logger.Error($"ReadAsynch - Error", e);
                throw new EditorRepositoryEFException("Error retrieving editors", e);
            }
        }

        public async Task<(Editor Editor, int TotalBooks)> GetByIdAsync(long id, int page, int pageSize)
        {
            _logger.Info($"GetByIdAsync - Started | Id: {id}");
            try
            {
                var editorEntity = await _context.Editors
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (editorEntity == null)
                {
                    _logger.Warn($"GetByIdAsync - Editor not found | Id: {id}");
                    return (null, 0);
                }

                // Paginazione sui libri
                int totalBooks = await _context.Books
                    .Where(b => b.EditorId == id)
                    .CountAsync();

                int offset = (page - 1) * pageSize;

                var books = await _context.Books
                    .Where(b => b.EditorId == id)
                    .OrderBy(b => b.BookId)
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                editorEntity.Books = books.ToHashSet();

                _logger.Info($"GetByIdAsync - Completed | Editor: {editorEntity.Name}, Books: {books.Count}");
                return (_editorMapper.ToDomain(editorEntity), totalBooks);
            }
            catch (Exception e)
            {
                _logger.Error($"GetByIdAsync - Error", e);
                throw new EditorRepositoryEFException("Error retrieving editor by id", e);
            }
        }
    }
}
