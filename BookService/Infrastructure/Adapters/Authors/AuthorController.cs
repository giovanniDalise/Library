using Library.BookService.Core.Application;
using Library.BookService.Core.Ports.Authors;
using Library.BookService.Infrastructure.DTO.REST;
using Library.BookService.Infrastructure.DTO.REST.Authors;
using Library.BookService.Infrastructure.DTO.REST.Editors;
using Library.BookService.Infrastructure.DTO.REST.Mappers;
using Library.Logging.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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
            [FromQuery] int pageSize = 10,
            [FromQuery] bool all = false
            )
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

                var (authors, totalRecords) = await _authorAppService.GetAuthorsAsync(
                    authorDomain,
                    all ? 1 : page,
                    all ? int.MaxValue : pageSize
                );

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
        [HttpGet("getAuthorDetail/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthorDetailResponse>> GetAuthorDetail(
        long id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            _logger.Info($"Call to GetAuthorDetail | Id:{id}");
            if (id <= 0)
            {
                _logger.Warn($"Invalid attempt with Id: {id}");
                return BadRequest(new {error = "Id must be greater than 0."});
            }
            try
            {
                var (author, totalBooks) = await _authorAppService.GetAuthorDetailAsync(id, page, pageSize);

                if(author == null)
                {
                    _logger.Warn($"Author not found | Id:{id}");
                    return BadRequest(new { error = $"Author with id {id} not found" });
                }

                return Ok(AuthorDTOMapper.ToDetailResponse(author, totalBooks));
            }
            catch (Exception ex)
            {
                _logger.Error("Error while retrieving author by id.", ex);
                return StatusCode(500, "Internal server error.");
            }
        }
        [HttpPost("AddAuthor")]
        [Authorize(Roles ="admin")]
        public async Task<ActionResult<long>> AddAuthor([FromBody] AuthorRequest request)
        {
            _logger.Info($"Attempting to add a new author: {request.Name} {request.Surname}");
            try
            {
                var authorDomain = AuthorDTOMapper.ToDomain(request);

                var createdAuthor = await _authorAppService.CreateAuthorAsync(authorDomain);
                _logger.Info($"Author successfully added with ID: {createdAuthor.Id}");

                return Ok(createdAuthor.Id);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while adding author: {request.Name} {request.Surname}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<AuthorResponse>> GetAuthorById(long id)
        {
            _logger.Info($"Call to GetAuthorById | Id: {id}");

            try
            {
                var author = await _authorAppService.GetAuthorByIdAsync(id);

                if (author == null)
                {
                    _logger.Warn($"Author not found | Id: {id}");
                    return NotFound();
                }

                var response = AuthorDTOMapper.ToResponse(author);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while retrieving editor {id}", ex);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
