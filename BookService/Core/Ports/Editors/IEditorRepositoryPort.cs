using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports.Editors
{
    public interface IEditorRepositoryPort
    {
        Task<(List<Editor> Items, int TotalRecords)> GetEditorsAsync(Editor searchEditor, int page, int pageSize);
        Task<(Editor Editor, int TotalBooks)> GetEditorByIdAsync(long id, int page, int pageSize);

    }
}
