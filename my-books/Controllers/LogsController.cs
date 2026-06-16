using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using my_books.Data.Services;

namespace my_books.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private LogService _logService;
        public LogsController(LogService logService)
        {
            _logService = logService;
        }

        [HttpGet("get-all-logs-from-db")]
        public IActionResult GetLogs()
        {
            try
            {
                var logs = _logService.GetAllLogsFromDb();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
