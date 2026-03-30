using Library.BookService.Core.Domain.Models;
using Library.BookService.Infrastructure.Persistence.EF.Entities;
using Library.BookService.Infrastructure.Persistence.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Library.BookService.Infrastructure.Persistence.EF.Mappers
{
    public class EditorEntityMapper : IMapper<EditorEntity, Editor>
    {

        public Editor ToDomain (EditorEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new Editor
            {
                Id = entity.Id,
                Name = entity.Name,
                Books = entity.Books?.Select(e => new Book
                {
                    Id = e.Id,
                    Title = e.Title
                }).ToList() ?? new List<Book>()
            };
        }
        public EditorEntity ToEntity(Editor domain)
        {
            if (domain  == null)
                throw new ArgumentNullException(nameof(domain));

            return new EditorEntity
            {
                Id = domain.Id,
                Name = domain.Name
            };
        }

        public List<Editor> ToDomainList(List<EditorEntity> entities)
        {
            return entities.Select(ToDomain).ToList();
        }
        public List<EditorEntity> ToEntityList(List<Editor> domains)
        {
            return domains.Select(ToEntity).ToList();
        }
    }
}