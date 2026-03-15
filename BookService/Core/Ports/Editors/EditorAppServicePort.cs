using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports.Editors
{
    public interface EditorAppServicePort
    {
        Task<(List<Editor> Editors, int TotalRecords)> GetEditorsAsync(Editor searchEditor, int page, int pageSize);
        Task<(Editor Editor, int TotalBooks)> GetEditorByIdAsync(long id, int page, int pageSize);

    }
}
