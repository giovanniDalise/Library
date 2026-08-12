using Library.BookService.Core.Ports.Editors;
using Library.BookService.Infrastructure.DTO.REST;
using Library.BookService.Infrastructure.DTO.REST.Editors;
using Library.BookService.Infrastructure.DTO.REST.Mappers;
using Library.BookService.Infrastructure.Exceptions;
using Library.BookService.Infrastructure.REST.Common;
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
            [FromQuery] int pageSize = 10,
            [FromQuery] bool all = false
            )
        {
            _logger.Info($"Call to GetEditors");
            //questi specifici controlli sembrano inutili dato che l'utente dal fe non potrà mai
            //selezionare il numero della pagina o il pageSize ma resta comunque un controllo necessario.
            //[AllowAnonymous] + [HttpPost("GetEditors")] con page/pageSize letti da query string.
            //Questo significa che chiunque, senza nemmeno autenticarsi, può chiamare
            //con un client HTTP qualsiasi(Postman, curl, uno script):
            //POST / editors / GetEditors ? page = -1 & pageSize = 999999
            //Senza il controllo BE, un pageSize = 999999 forzerebbe una query che
            //tenta di caricare 999999 editor in un colpo solo — nella migliore
            //delle ipotesi un endpoint lento, nella peggiore un vettore per
            //denial-of - service(basta ripetere la richiesta poche volte in parallelo
            //per stressare il DB).

            var validationError = PaginationValidator.Validate(page, pageSize);
            if (validationError != null)
            {
                _logger.Warn($"Invalid pagination | Page={page} | PageSize={pageSize}");
                return validationError;
            }

            try
            {
                var editorDomain = EditorDTOMapper.ToDomain(request);

                var (editors, totalRecords) = await _editorAppService.GetEditorsAsync(editorDomain,
                    all ? 1 : page,
                    all ? int.MaxValue : pageSize
                );

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

        [HttpGet("getEditorDetail/{id}")]
        [AllowAnonymous]
        //GET /api/editors/10?page=1&pageSize=10 il ? segna l'inizio della query ed è posto dopo la route
        public async Task<ActionResult<PagedResponse<EditorDetailResponse>>> GetEditorDetail(
            long id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.Info($"Call to GetEditorDetail | Id: {id}");

            var validationError = PaginationValidator.Validate(page, pageSize);
            if (validationError != null)
            {
                _logger.Warn($"Invalid pagination | Page={page} | PageSize={pageSize}");
                return validationError;
            }
            if (id <= 0)
            {
                _logger.Warn($"Invalid attempt with Id: {id}");
                return BadRequest(new { error = "Id must be greater than 0." });
                //  BadRequest Codice 400
                // sintassi per creare un oggetto anonimo per creare un oggetto al volo
                //public class ErrorResponse
                //{
                //    public string error { get; set; }
                //}
                //in modo che poi ASP.NET Core lo serializza e restituisce 400 con questo Json
                //{
                //    "error": "Id must be greater than 0."
                //}
            }

            try
            {
                var (editor, totalBooks) = await _editorAppService.GetEditorDetailAsync(id, page, pageSize);

                if (editor == null)
                {
                    _logger.Warn($"Editor not found | Id: {id}");
                    return NotFound(new { error = $"Editor with id {id} not found." });
                    // Not Found Codice 404
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
        public async Task<ActionResult<long>> AddEditor([FromBody] EditorRequest request)
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

        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<EditorResponse>> GetEditorById(long id)
        {
            _logger.Info($"Call to GetEditorById | Id: {id}");

            try
            {
                var editor = await _editorAppService.GetEditorByIdAsync(id);

                if (editor == null)
                {
                    _logger.Warn($"Editor not found | Id: {id}");
                    return NotFound();
                }

                var response = EditorDTOMapper.ToResponse(editor);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while retrieving editor {id}", ex);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateEditor(long id, [FromBody] EditorRequest request)
        {
            _logger.Info($"Attempting to update editor | Id: {id}");

            try
            {
                var editorDomain = EditorDTOMapper.ToDomain(request);
                editorDomain.Id = id;

                await _editorAppService.UpdateEditorAsync(editorDomain);
                _logger.Info($"Editor successfully updated | Id: {id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while updating editor | Id: {id}", ex);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteEditor(long id)
        {
            _logger.Info($"Attempting to delete editor | Id: {id}");

            try
            {
                await _editorAppService.DeleteEditorAsync(id);
                _logger.Info($"Editor successfully deleted | Id: {id}");
                return NoContent();
            }
            catch (EditorRepositoryEFException ex)
            {
                _logger.Warn($"Business rule violation while deleting editor | Id: {id} | {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while deleting editor | Id: {id}", ex);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
