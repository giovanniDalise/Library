using Library.BookService.Core.Ports.Editors;
using Library.BookService.Infrastructure.DTO.REST;
using Library.BookService.Infrastructure.DTO.REST.Editors;
using Library.BookService.Infrastructure.DTO.REST.Mappers;
using Library.Logging.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Library.BookService.Infrastructure.Adapters.Editors
{
    [Route("editors")]
    [ApiController]
    public class EditorController : ControllerBase
    {
        private readonly IEditorAppServicePort _editorAppService;
        private readonly ILoggerPort _logger;
        public EditorController(IEditorAppServicePort editorAppService, ILoggerPort logger)
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

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResponse<EditorDetailResponse>>> GetEditorById(
            long id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.Info($"Call to GetEditorById | Id: {id}");

            if (id <= 0)
            {
                _logger.Warn($"Invalid attempt with Id: {id}");
                return BadRequest(new { error = "Id must be greater than 0." });
            }

            try
            {
                var (editor, totalBooks) = await _editorAppService.GetEditorByIdAsync(id, page, pageSize);

                if (editor == null)
                {
                    _logger.Warn($"Editor not found | Id: {id}");
                    return NotFound(new { error = $"Editor with id {id} not found." });
                }

                return Ok(EditorDTOMapper.ToDetailResponse(editor, totalBooks));
            }
            catch (Exception ex)
            {
                _logger.Error("Error while retrieving editor by id.", ex);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
