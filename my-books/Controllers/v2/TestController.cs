using Microsoft.AspNetCore.Mvc;

namespace my_books.Controllers.v2
{
    [ApiVersion("2.0")]
    [ApiVersion("2.1")]
    [ApiVersion("2.5")]
    [Route("api/[controller]")]
    //[Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("get-test-data"), MapToApiVersion("2.0")]
        public ActionResult Get()
        {
            return Ok("This is test controller data v2.0");
        }

        [HttpGet("get-test-data"), MapToApiVersion("2.1")]
        public ActionResult Getv21()
        {
            return Ok("This is test controller data v2.1");
        }

        [HttpGet("get-test-data"), MapToApiVersion("2.5")]
        public ActionResult Getv25()
        {
            return Ok("This is test controller data v2.5");
        }
    }
}
