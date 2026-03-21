
using Library.BookService.Core.Domain.Models;
using Library.BookService.Infrastructure.DTO.REST.Editors;

namespace Library.BookService.Infrastructure.DTO.REST.Mappers
{
    public class EditorDTOMapper
    {
        public static EditorResponse ToResponse(Editor editor)
        {
            return new EditorResponse
            {
                Id = editor.Id,
                Name = editor.Name
            };
        }

        public static Editor ToDomain(EditorRequest request)
        {
            return new Editor
            {
                Id = request.Id,
                Name = request.Name
            };  
        }

        public static EditorDetailResponse ToDetailResponse(Editor editor, int totalBooks)
        {
            return new EditorDetailResponse
            {
                Id = editor.Id,
                Name = editor.Name,
                Books = new PagedResponse<string>
                {
                    Items = editor.Books?.Select(b => b.Title).ToList() ?? new List<string>(),
                    TotalRecords = totalBooks
                }
            };
        }

        public static List<EditorResponse> ToResponseList(List<Editor> editors)
        {
            return editors.Select(ToResponse).ToList();
        }
    }
}
