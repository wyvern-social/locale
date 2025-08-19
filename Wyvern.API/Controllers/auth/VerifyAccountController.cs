using Microsoft.AspNetCore.Mvc;

namespace Wyvern.API.Controllers
{
    [ApiController]
    [Route("auth/verify")]
    public class VerifyAccountController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post()
        {
            return Ok("Ok");
        }
    }
}
