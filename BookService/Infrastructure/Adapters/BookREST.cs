using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

        [HttpGet]
        [AllowAnonymous] 
        public async Task<ActionResult<List<Book>>> GetBooks()
        {
            return Ok(await _bookService.GetAllBooksAsync());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult<Book>> GetBookById(long id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            return book == null ? NotFound() : Ok(book);
        }

        [HttpPost]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult<long>> AddBook([FromBody] Book book)
        {
            return Ok(await _bookService.CreateBookAsync(book));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult<long>> UpdateBook(long id, [FromBody] Book book)
        {
            return Ok(await _bookService.UpdateBookAsync(id, book));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult<long>> DeleteBook(long id)
        {
            return Ok(await _bookService.DeleteBookAsync(id));
        }

        [HttpGet("findByString")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Book>>> FindBooksByString([FromQuery] string param)
        {
            return Ok(await _bookService.GetBooksByTextAsync(param));
        }

        [HttpPost("findByBook")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult<List<Book>>> FindBooksByBook([FromBody] Book book)
        {
            return Ok(await _bookService.GetBooksByObjectAsync(book));
        }
    }
}
