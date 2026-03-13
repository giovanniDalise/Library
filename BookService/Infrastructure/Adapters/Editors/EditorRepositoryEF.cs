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

                IQueryable<EditorEntity> query = _context.Editors.Include(e => e.Books);

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
    }
}
