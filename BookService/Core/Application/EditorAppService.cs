using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports;
using Library.BookService.Core.Ports.Books;
using Library.BookService.Core.Ports.Editors;
using Library.Logging.Abstractions;

namespace Library.BookService.Core.Application
{
    public class EditorAppService: EditorAppServicePort
    {
        private readonly EditorServicePort _editorDomainService;
        private readonly ILoggerPort _logger;

        public EditorAppService(
            EditorServicePort editorDomainService,
            ILoggerPort logger)
        {
            _editorDomainService = editorDomainService;
            _logger = logger;
        }

        public async Task<(List<Editor> Editors, int TotalRecords)> GetEditorsAsync(Editor searchEditor, int page, int pageSize)
        {
            return await _editorDomainService.GetEditorsAsync(searchEditor, page, pageSize);

        }
        public async Task<Editor> GetEditorByIdAsync(long id)
        {
            return await _editorDomainService.GetEditorByIdAsync(id);
        }
    }
}
