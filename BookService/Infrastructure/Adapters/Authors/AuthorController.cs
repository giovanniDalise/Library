using Library.BookService.Core.Ports.Authors;
using Library.BookService.Infrastructure.DTO.REST;
using Library.BookService.Infrastructure.DTO.REST.Authors;
using Library.BookService.Infrastructure.DTO.REST.Mappers;
using Library.Logging.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.BookService.Infrastructure.Adapters.Authors
{
    [Microsoft.AspNetCore.Mvc.Route("authors")]
    [ApiController]
    public class AuthorController:ControllerBase
    {
        private readonly IAuthorAppServicePort _authorAppService;
        private readonly ILoggerPort _logger;

        public AuthorController (IAuthorAppServicePort authorAppService, ILoggerPort logger)
        {
            _authorAppService = authorAppService;
            _logger = logger;
        }

        [HttpPost("GetAuthors")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResponse<AuthorResponse>>> GetAuthors (
            [FromBody] AuthorRequest request,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.Info("Call to GetAuthors");

            if(page < 1)
            {
                _logger.Warn($"Invalid attempt with Page {page}");
                return BadRequest(new {error = "Page must be greater or equal to 1"});
            }

            if(pageSize < 1 || pageSize > 10)
            {
                _logger.Warn($"Invalid attempt with PageSize {pageSize}");
                return BadRequest(new { error = "PageSize must be between 1 and 10" });
            }
            try
            {
                var authorDomain = AuthorDTOMapper.ToDomain(request);

                var (authors, totalRecords) = await _authorAppService.GetAuthorsAsync(authorDomain, page, pageSize);

                var response = new PagedResponse<AuthorResponse>
                {
                    Items = AuthorDTOMapper.ToResponseList(authors),
                    TotalRecords = totalRecords
                };

                _logger.Info($"Founded {response.TotalRecords} authors");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error("Error while retrieving authors", ex);
                return StatusCode(500,"Internal server error");
            }
        }
    }
}
