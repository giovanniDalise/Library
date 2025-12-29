using Library.BookService.Core.Domain.Models;
using Library.BookService.Infrastructure.Persistence.EF.Entities;

namespace Library.BookService.Infrastructure.Persistence.EF.Mappers
{
    public class EditorMapper
    {
        public static Editor ToDomain(EditorEntity entity)
        {
            if (entity == null) return null;

            var editor = new Editor
            {
                Id = entity.Id,
                Name = entity.Name
                // Books mapping can be added if necessary
            };

            return editor;
        }

        public static EditorEntity ToEntity(Editor editor)
        {
            if (editor == null) return null;

            var entity = new EditorEntity
            {
                Id = editor.Id,
                Name = editor.Name
                // Books mapping can be added if necessary
            };

            return entity;
        }
    }
}
