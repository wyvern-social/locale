using Microsoft.AspNetCore.Mvc;

namespace Wyvern.API.Controllers
{
    [ApiController]
    [Route("auth/delete_account")]
    public class DeleteAccount : ControllerBase
    {
        [HttpPost]
        public IActionResult Post()
        {
            return Ok("Ok");
        }
    }
}
