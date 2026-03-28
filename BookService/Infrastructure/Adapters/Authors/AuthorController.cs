using Library.BookService.Core.Ports.Authors;
using Microsoft.AspNetCore.Mvc;

namespace Library.BookService.Infrastructure.Adapters.Authors
{
    [Microsoft.AspNetCore.Mvc.Route("authors")]
    [ApiController]
    public class AuthorController:ControllerBase
    {
        private readonly IAuthorAppServicePort _authorAppService;
        private readonly ILogger _logger;

        public AuthorController (IAuthorAppServicePort authorAppService, ILogger logger)
        {
            _authorAppService = authorAppService;
            _logger = logger;
        }

    }
}
