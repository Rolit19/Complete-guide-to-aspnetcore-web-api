using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using my_books.Data.Services;
using my_books.Data.ViewModel;
using my_books.Data.ViewModel.Authentication;

namespace my_books.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Author)]
    public class AuthorsController : ControllerBase
    {
        public AuthorService _authorService;
        public AuthorsController(AuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpPost("add-author")]
        public IActionResult AddAuthor([FromBody] AuthorVM author)
        {
            _authorService.AddAuthor(author);
            return Ok();
        }

        [HttpGet("get-author-with-books/{id}")]
        public IActionResult GetAuthorWithBooks(int id)
        {
            var authorWithBooks = _authorService.GetAuthorWithBooks(id);
            if (authorWithBooks == null)
                return NotFound();
            return Ok(authorWithBooks);
        }
    }
}
