using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using my_books.Data.Services;
using my_books.Data.ViewModel;

namespace my_books.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        public PublisherService _publisherService;
        public PublishersController(PublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpPost("add-publisher")]
        public IActionResult AddPublisher([FromBody] PublisherVM publisher)
        {
            _publisherService.AddPublisher(publisher);
            return Ok();
        }

        [HttpGet("get-publisher-books-authors/{id}")]
        public IActionResult GetPublisherData(int id)
        {
            var publisherData = _publisherService.GetPublisherData(id);
            if (publisherData == null)
                return NotFound();
            return Ok(publisherData);
        }

        [HttpDelete("delete-publisher-by-id/{id}")]
        public IActionResult DeletePublisher(int id)
        {
            _publisherService.DeletePublisherById(id);
            return Ok();
        }
    }
}
