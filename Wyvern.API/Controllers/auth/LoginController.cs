using Microsoft.AspNetCore.Mvc;

namespace Wyvern.API.Controllers
{
    [ApiController]
    [Route("auth/login")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post()
        {
            return Ok("Ok");
        }
    }
}
