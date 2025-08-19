using Microsoft.AspNetCore.Mvc;

namespace Wyvern.API.Controllers
{
    [ApiController]
    [Route("auth/password_reset")]
    public class ResetPassController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post()
        {
            return Ok("Ok");
        }
    }
}
