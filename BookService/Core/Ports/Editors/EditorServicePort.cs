using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports.Editors
{
    public interface EditorServicePort
    {
        Task<(List<Editor> Editors, int TotalRecords)> GetEditorsAsync(Editor searchEditor, int page, int pagesize);
        Task<(Editor Editor, int TotalBooks)> GetEditorByIdAsync(long id, int page, int pageSize); // ← aggiunta paginazione
    }
}
