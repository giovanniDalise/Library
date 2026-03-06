
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
        public static List<EditorResponse> ToResponseList(IEnumerable<Editor> editors)
        {
            return editors.Select(ToResponse).ToList();
        }
        public static Editor ToDomain(EditorRequest request)
        {
            var editor = new Editor();

            editor.SetId(request.Id);
            editor.SetName(request.Name);

            return editor;
        }
    }
}
