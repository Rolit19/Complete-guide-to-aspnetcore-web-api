using Microsoft.AspNetCore.Mvc;
using my_books.Data.ViewModel;

namespace my_books.ActionResults
{
    public class CustomActionResult : IActionResult
    {
        private readonly CustomActionResultVM _result;

        public CustomActionResult(CustomActionResultVM result)
        {
            _result = result;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(_result.Exception ?? _result.Publisher as object)
            {
                StatusCode = _result.Exception == null ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }
}
