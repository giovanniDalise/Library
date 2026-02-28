using Library.BookService.Infrastructure.DTO.REST.Book;
using Library.BookService.Infrastructure.DTO.REST.Editor;

namespace Library.BookService.Infrastructure.DTO.REST.Mappers
{
    public class EditorDTOMapper
    {
        public static EditorResponse ToResponse(Core.Domain.Models.Editor editor)
        {
            return new EditorResponse
            {
                Id = editor.Id,
                Name = editor.Name
            };
        }
        public static List<EditorResponse> ToResponseList(IEnumerable<Core.Domain.Models.Editor> editors)
        {
            return editors.Select(ToResponse).ToList();
        }
        public static Core.Domain.Models.Editor ToDomain(EditorRequest request)
        {
            var editor = new Core.Domain.Models.Editor();

            editor.SetId(request.Id);
            editor.SetName(request.Name);

            return editor;
        }
    }
}
