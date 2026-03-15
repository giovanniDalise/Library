using Library.BookService.Core.Domain.Models;

namespace Library.BookService.Core.Ports.Editors
{
    public interface EditorRepositoryPort
    {
        Task<(List<Editor> Items, int TotalRecords)> ReadAsync(Editor searchEditor, int page, int pageSize);
        Task<Editor> GetByIdAsync(long id);

    }
}
