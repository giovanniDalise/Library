using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports.Books;
using Library.BookService.Core.Ports.Editors;

namespace Library.BookService.Core.Domain.Services
{
    public class EditorService: EditorServicePort
    {
        private readonly EditorRepositoryPort _editorRepositoryPort;

        public EditorService(EditorRepositoryPort editorRepositoryPort)
        {
            _editorRepositoryPort = editorRepositoryPort ?? throw new ArgumentNullException(nameof(editorRepositoryPort));
        }
        public async Task<(List<Editor> Editors, int TotalRecords)> GetEditorsAsync(Editor searchEditor, int page, int pageSize)
        {
            return await _editorRepositoryPort.ReadAsync(searchEditor, page, pageSize);
        }
        public async Task<(Editor Editor, int TotalBooks)> GetEditorByIdAsync(long id, int page, int pageSize)
        {
            return await _editorRepositoryPort.GetByIdAsync(id, page, pageSize);
        }
    }
}
