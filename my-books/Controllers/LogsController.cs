using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using my_books.Data.Services;
using my_books.Data.ViewModel.Authentication;

namespace my_books.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Admin)]
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
