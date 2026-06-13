using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports.Editors
{
    public interface IEditorServicePort
    {
        Task<(List<Editor> Editors, int TotalRecords)> GetEditorsAsync(Editor searchEditor, int page, int pagesize);
        Task<(Editor Editor, int TotalBooks)> GetEditorDetailAsync(long id, int page, int pageSize);
        Task<Editor?> GetEditorByIdAsync(long id);

        Task<Editor> CreateEditorAsync(Editor editor);

    }
}
