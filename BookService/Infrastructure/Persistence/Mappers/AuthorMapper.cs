using Library.BookService.Core.Domain.Models;
using Library.BookService.Infrastructure.Persistence.Entities;

namespace Library.BookService.Infrastructure.Persistence.Mappers
{
    public class AuthorMapper
    {
        public static Author ToDomain(AuthorEntity entity)
        {
            if (entity == null) return null;

            var author = new Author
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname
                // Books mapping can be added if necessary
            };

            return author;
        }

        public static AuthorEntity ToEntity(Author author)
        {
            if (author == null) return null;

            var entity = new AuthorEntity
            {
                Id = author.Id,
                Name = author.Name,
                Surname = author.Surname
                // Books mapping can be added if necessary
            };

            return entity;
        }

        public static HashSet<Author> ToDomainSet(ICollection<AuthorEntity> entities)
        {
            if (entities == null) return null;

            return entities.Select(AuthorMapper.ToDomain)
                           .ToHashSet();
        }

        public static HashSet<AuthorEntity> ToEntitySet(HashSet<Author> authors)
        {
            if (authors == null) return null;

            return authors.Select(AuthorMapper.ToEntity)
                          .ToHashSet();
        }
    }
}
