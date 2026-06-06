using Library.BookService.Core.Application;
using Library.BookService.Core.Ports.Editors;
using Library.BookService.Infrastructure.DTO.REST;
using Library.BookService.Infrastructure.DTO.REST.Books;
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
        private readonly IEditorAppServicePort _editorAppService;
        private readonly ILoggerPort _logger;
        public EditorController(IEditorAppServicePort editorAppService, ILoggerPort logger)
        {
            _editorAppService = editorAppService;
            _logger = logger;
        }

        [HttpPost("GetEditors")]
        [AllowAnonymous]
        //POST /api/authors/GetEditors?page=1&pageSize=10
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

                // sintassi per creare un oggetto anonimo per creare un oggetto al volo
                //public class ErrorResponse
                //{
                //    public string error { get; set; }
                //}
                //in modo che poi ASP.NET Core lo serializza e restituisce 400 con questo Json
                //{
                //    "error": "Page must be greater than or equal to 1."
                //}
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
        //GET /api/editors/10?page=1&pageSize=10 il ? segna l'inizio della query ed è posto dopo la route
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
        [HttpPost("AddEditor")]
        [Authorize(Roles = "admin")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<long>> AddEditor([FromForm] EditorRequest request)
        {
            _logger.Info($"Attempting to add a new editor: {request.Name}");

            try
            {
                var editorDomain = EditorDTOMapper.ToDomain(request);

                var createdEditor = await _editorAppService.CreateEditorAsync(editorDomain);
                _logger.Info($"Editor successfully added with ID: {createdEditor.Id}");

                return Ok(createdEditor.Id);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while adding editor: {request.Name}", ex);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
