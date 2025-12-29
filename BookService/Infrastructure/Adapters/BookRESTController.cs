using Library.BookService.Core.Ports;
using Library.BookService.Infrastructure.DTO.REST;
using Library.BookService.Infrastructure.DTO.REST.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.BookService.Infrastructure.Adapters
{
    [Route("library")]
    [ApiController]
    public class BookRESTController : ControllerBase
    {
        private readonly BookServicePort _bookService;

        public BookRESTController(BookServicePort bookService)
        {
            _bookService = bookService;
        }

        // GET /library
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<BookResponse>>> GetBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            var response = BookDTOMapper.ToResponseList(books);
            return Ok(response);
        }

        // GET /library/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<BookResponse>> GetBookById(long id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();
            var response = BookDTOMapper.ToResponse(book);
            return Ok(response);
        }

        // POST /library
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<long>> AddBook([FromBody] BookRequest request)
        {
            var bookDomain = BookDTOMapper.ToDomain(request);
            var newId = await _bookService.CreateBookAsync(bookDomain);
            return Ok(newId);
        }

        // PUT /library/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<long>> UpdateBook(long id, [FromBody] BookRequest request)
        {
            var bookDomain = BookDTOMapper.ToDomain(request);
            var updatedId = await _bookService.UpdateBookAsync(id, bookDomain);
            return Ok(updatedId);
        }

        // DELETE /library/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<long>> DeleteBook(long id)
        {
            return Ok(await _bookService.DeleteBookAsync(id));
        }

        // GET /library/findByString?param=...
        [HttpGet("findByString")]
        [AllowAnonymous]
        public async Task<ActionResult<List<BookResponse>>> FindBooksByString([FromQuery] string param)
        {
            var books = await _bookService.GetBooksByTextAsync(param);
            var response = BookDTOMapper.ToResponseList(books);
            return Ok(response);
        }

        // POST /library/findByBook
        [HttpPost("findByBook")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<BookResponse>>> FindBooksByBook([FromBody] BookRequest request)
        {
            var bookDomain = BookDTOMapper.ToDomain(request);
            var books = await _bookService.GetBooksByObjectAsync(bookDomain);
            var response = BookDTOMapper.ToResponseList(books);
            return Ok(response);
        }
    }
}
