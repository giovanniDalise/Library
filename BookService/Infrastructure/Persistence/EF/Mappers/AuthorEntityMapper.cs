using Library.BookService.Core.Domain.Models;
using Library.BookService.Infrastructure.Persistence.EF.Entities;
using Library.BookService.Infrastructure.Persistence.Interfaces;

namespace Library.BookService.Infrastructure.Persistence.EF.Mappers
{
    public class AuthorEntityMapper : IMapper<AuthorEntity, Author>
    {
        public  Author ToDomain(AuthorEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new Author
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Books = entity.Books?.Select(b => new Book
                {
                    Id = b.Id,
                    Title = b.Title
                }).ToList() ?? new List<Book>()
            };
        }

        public AuthorEntity ToEntity(Author domain)
        {
            if (domain == null)
                throw new ArgumentNullException(nameof(domain));

            return new AuthorEntity
            {
                Id = domain.Id,
                Name = domain.Name,
                Surname = domain.Surname
                // Books mapping can be added if necessary
            };

        }

        public List<Author> ToDomainList(List<AuthorEntity> entities)
        {
            return entities.Select(ToDomain).ToList();
        }

        public List<AuthorEntity> ToEntityList(List<Author> authors)
        {
            return authors.Select(ToEntity).ToList();
        }
    }
}
