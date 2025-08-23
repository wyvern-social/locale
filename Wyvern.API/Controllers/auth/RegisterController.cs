using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Wyvern.ConfigModel;
using Wyvern.Utils.Validators;

namespace Wyvern.API.Controllers
{
    [ApiController]
    [Route("auth/register")]
    public class RegisterController : ControllerBase
    {
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterRequest model)
        {
            var usernameResult = await UsernameValidator.CheckAsync(model.Username);
            if (!usernameResult.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    statusCode = 400,
                    data = new { message = usernameResult.ErrorMessage }
                });
            }

            var emailResult = await EmailValidator.CheckAsync(model.Email);
            if (!emailResult.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    statusCode = 400,
                    data = new { message = emailResult.ErrorMessage }
                });
            }

            var passResult = await PasswordValidator.CheckAsync(model.Password);
            if (!passResult.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    statusCode = 400,
                    data = new { message = passResult.ErrorMessage }
                });
            }

            var ageResult = AgeValidator.Check(model.Birthday);
            if (!ageResult.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    statusCode = 400,
                    data = new { message = ageResult.ErrorMessage }
                });
            }

            bool isInviteOnly = Config.GetKey<bool>("api", "registration", "invite_only");
            if (isInviteOnly)
            {
                if (string.IsNullOrWhiteSpace(model.InviteCode))
                {
                    Console.WriteLine("Invite code required.");
                    return StatusCode(403, new
                    {
                        success = false,
                        statusCode = 403,
                        data = new { message = "Registration is currently invite-only. Please provide a valid invite code to continue." }
                    });
                }

                var validInvite = "AB12-CD34"; // Example
                if (model.InviteCode != validInvite)
                {
                    Console.WriteLine("Invalid invite code.");
                    return StatusCode(403, new
                    {
                        success = false,
                        statusCode = 403,
                        data = new { message = "The provided invite code is invalid." }
                    });
                }
            }

            return Ok(new
            {
                success = true,
                statusCode = 200,
                data = new { message = "success" }
            });
        }
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime Birthday { get; set; }
        public string? Locale { get; set; }

        [JsonPropertyName("invite_code")]
        public string? InviteCode { get; set; }
    }
}
