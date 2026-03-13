
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

        public static List<EditorResponse> ToResponseList(IEnumerable<Editor> editors)
        {
            return editors.Select(ToResponse).ToList();
        }
    }
}
