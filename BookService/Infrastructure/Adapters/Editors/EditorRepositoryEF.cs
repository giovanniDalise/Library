using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports.Editors;
using Library.BookService.Infrastructure.Exceptions;
using Library.BookService.Infrastructure.Persistence.EF;
using Library.BookService.Infrastructure.Persistence.EF.Entities;
using Library.BookService.Infrastructure.Persistence.EF.Mappers;
using Library.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;
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
            _logger.Info($"ReadAsync - Start | Title={searchEditor.Name ?? "null"}");

            try
            {
                int offset = (page - 1) * pageSize;

                //IQueryable rispetto IEnumerable permette di non caricare subito i dati in 
                //memoria e lavorare sulla query che viene eseguita solo quando serve con il ToListAsync()
                IQueryable<EditorEntity> query = _context.Editors.Include(e => e.Books);

                if (searchEditor.Id > 0)
                {
                    query = query.Where(e => e.Id == searchEditor.Id);
                }

                if (!string.IsNullOrEmpty(searchEditor.Name))
                {
                    query = query.Where(b => b.Name.Contains(searchEditor.Name));
                }

                int total = await query.CountAsync();

                var editorEntities = await query
                    .OrderBy(e => e.Id)
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.Info($"ReadAsync - Completed | Results={editorEntities.Count}");

                return (_editorMapper.ToDomainList(editorEntities), total);
            }
            catch (Exception e)
            {
                _logger.Error("ReadAsync - Error", e);

                throw new EditorRepositoryEFException("Error finding books by object: " + e.Message);
            }
        }
    }
}
