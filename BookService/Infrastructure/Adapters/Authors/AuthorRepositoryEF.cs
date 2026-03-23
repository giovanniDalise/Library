using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports.Authors;
using Library.BookService.Infrastructure.Exceptions;
using Library.BookService.Infrastructure.Persistence.EF;
using Library.BookService.Infrastructure.Persistence.EF.Entities;
using Library.BookService.Infrastructure.Persistence.EF.Mappers;
using Library.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Library.BookService.Infrastructure.Adapters.Authors
{
    public class AuthorRepositoryEF:IAuthorRepositoryPort
    {
        private readonly AuthorEntityMapper _authorMapper;
        private readonly BookDBContext _context;
        private readonly ILoggerPort _logger;

        public AuthorRepositoryEF(
            AuthorEntityMapper authorMapper,
            BookDBContext context,
            ILoggerPort logger)
        { 
            _authorMapper = authorMapper;
            _context = context;
            _logger = logger;
        }
    
    public async Task<(List<Author> Items, int TotalRecords)> GetAuthorsAsync (Author searchAuthor, int page, int pageSize)
        {
            _logger.Info($"GetAuthorsAsync - Started | Author: {searchAuthor.Name} {searchAuthor.Surname}");

            try
            {
                int offset = (page - 1) * pageSize;

                IQueryable<AuthorEntity> query = _context.Authors;

                if(searchAuthor.Id > 0)
                {
                    query = query.Where(a => a.Id == searchAuthor.Id);
                }

                if (!string.IsNullOrEmpty(searchAuthor.FullName))
                {
                    query = query.Where(a => a.FullName.Contains(searchAuthor.FullName));
                }

                int total = await query.CountAsync();

                var authorEntities = await query
                    .OrderBy(a => a.Surname)
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.Info($"GetAuthorsAsync - Completed | {authorEntities.Count} items");

                return (_authorMapper.ToDomainList(authorEntities), total);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetAuthorsAsync - Error", ex);
                throw new AuthorRepositoryEFException("Error retrieving authors", ex);
            }
        }
    }
}
