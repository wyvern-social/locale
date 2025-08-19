using Microsoft.AspNetCore.Mvc;

namespace Wyvern.API.Controllers
{
    [ApiController]
    [Route("auth/register")]
    public class RegisterController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post()
        {
            return Ok("Ok");
        }
    }
}
