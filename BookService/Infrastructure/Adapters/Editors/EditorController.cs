using Library.BookService.Core.Ports.Editors;
using Library.BookService.Infrastructure.DTO.REST;
using Library.BookService.Infrastructure.DTO.REST.Editors;
using Library.BookService.Infrastructure.DTO.REST.Mappers;
using Library.Logging.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.BookService.Infrastructure.Adapters.Editors
{
    [Route("editors")]
    [ApiController]
    public class EditorController : ControllerBase
    {
        private readonly EditorAppServicePort _editorAppService;
        private readonly ILoggerPort _logger;
        public EditorController(EditorAppServicePort editorAppService, ILoggerPort logger)
        {
            _editorAppService = editorAppService;
            _logger = logger;
        }

        [HttpPost("GetEditors")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResponse<EditorResponse>>> GetEditors(
            [FromBody] EditorRequest request,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
            )
        {
            _logger.Info($"Call to GetEditors");

            if (page < 1)
            {
                _logger.Warn($"Invalid attempt with Page: {page}");
                return BadRequest(new { error = "Page must be greater than or equal to 1." });
            }

            if (pageSize < 1 || pageSize > 10)
            {
                _logger.Warn($"Invalid attempt with PageSize: {pageSize}");
                return BadRequest(new { error = "PageSize must be between 1 and 10." });
            }

            try
            {
                var editorDomain = EditorDTOMapper.ToDomain(request);

                var (editors, totalRecords) = await _editorAppService.GetEditorsAsync(editorDomain, page, pageSize);

                var response = new PagedResponse<EditorResponse>
                {
                    Items = EditorDTOMapper.ToResponseList(editors),
                    Page = page,
                    PageSize = pageSize,
                    TotalRecords = totalRecords
                };
                _logger.Info($"Founded {response.TotalRecords} editors");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error("Error while retrieving editors.", ex);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
