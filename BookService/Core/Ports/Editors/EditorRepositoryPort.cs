using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports.Editors
{
    public interface EditorRepositoryPort
    {
        Task<(List<Editor> Items, int TotalRecords)> ReadAsync(Editor searchEditor, int page, int pageSize);
        Task<(Editor Editor, int TotalBooks)> GetByIdAsync(long id, int page, int pageSize);

    }
}
