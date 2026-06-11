using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using my_books.Data.Services;
using my_books.Data.ViewModel;
using my_books.Exceptions;

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
            try
            {
                var newPublisher = _publisherService.AddPublisher(publisher);
                return Created(nameof(AddPublisher), newPublisher);
            }
            catch(PublisherNameException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-publisher-books-authors/{id}")]
        public IActionResult GetPublisherData(int id) 
        {
            var publisherData = _publisherService.GetPublisherData(id);
            if (publisherData == null)
                return NotFound();
            return Ok(publisherData);
        }

        [HttpGet("get-publisher-by-id/{id}")]
        public IActionResult GetPublisherById(int id)
        {
            var publisher = _publisherService.GetPublisherById(id);
            if (publisher == null)
                return NotFound();
            return Ok(publisher);
        }

        [HttpDelete("delete-publisher-by-id/{id}")]
        public IActionResult DeletePublisher(int id)
        {
            try
            {
                _publisherService.DeletePublisherById(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-all-publishers")]
        public IActionResult GetAllPublishers()
        {
            try
            {
                var publishers = _publisherService.GetAllPublishers();
                return Ok(publishers);
            }
            catch (Exception ex)
            {
                return BadRequest("Sorry, an error occurred while fetching all publishers.");
            }
        }
    }
}
