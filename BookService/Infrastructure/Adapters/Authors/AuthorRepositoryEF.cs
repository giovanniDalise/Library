using Library.BookService.Core.Ports.Authors;
using Library.BookService.Infrastructure.Persistence.EF;
using Library.BookService.Infrastructure.Persistence.EF.Mappers;
using Library.Logging.Abstractions;

namespace Library.BookService.Infrastructure.Adapters.Authors
{
    public class AuthorRepositoryEF:AuthorRepositoryPort
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

    }
}
