using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Wyvern.ConfigModel;
using Wyvern.Utils.Validators;
using Wyvern.Database.Data;
using Microsoft.EntityFrameworkCore;
using Wyvern.Database.Repositories;

namespace Wyvern.API.Controllers
{
    [ApiController]
    [Route("auth/register")]
    public class RegisterController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWaitlistRepository _WaitlistRepository;

        public RegisterController(AppDbContext db, IWaitlistRepository WaitlistRepository)
        {
            _db = db;
            _WaitlistRepository = WaitlistRepository;
        }

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
                    data = new
                    {
                        message_key = usernameResult.MessageKey,
                        message = usernameResult.Message
                    }
                });
            }

            var emailResult = await EmailValidator.CheckAsync(model.Email);
            if (!emailResult.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    statusCode = 400,
                    data = new
                    {
                        message_key = emailResult.MessageKey,
                        message = emailResult.Message
                    }
                });
            }

            var passResult = await PasswordValidator.CheckAsync(model.Password);
            if (!passResult.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    statusCode = 400,
                    data = new
                    {
                        message_key = passResult.MessageKey,
                        message = passResult.Message
                    }
                });
            }

            var ageResult = AgeValidator.Check(model.Birthday);
            if (!ageResult.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    statusCode = 400,
                    data = new
                    {
                        message_key = ageResult.MessageKey,
                        message = ageResult.Message
                    }
                });
            }

            bool isInviteOnly = Config.GetKey<bool>("api", "registration", "invite_only");
            if (isInviteOnly)
            {
                if (string.IsNullOrWhiteSpace(model.InviteCode))
                {
                    return StatusCode(403, new
                    {
                        success = false,
                        statusCode = 403,
                        data = new
                        {
                            message_key = "auth.register.invite_only",
                            message = "Registration is currently invite-only. Please provide a valid invite code to continue."
                        }
                    });
                }

                var validInvite = "AB12-CD34"; // Example
                if (model.InviteCode != validInvite)
                {
                    return StatusCode(403, new
                    {
                        success = false,
                        statusCode = 403,
                        data = new
                        {
                            message_key = "auth.register.invalid_invite",
                            message = "The provided invite code is invalid."
                        }
                    });
                }
            }

            var waitlistEntry = await _WaitlistRepository.GetByUsernameAsync(model.Username);

            if (waitlistEntry != null && !string.Equals(waitlistEntry.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(403, new
                {
                    success = false,
                    statusCode = 403,
                    data = new
                    {
                        message_key = "auth.register.username_reserved",
                        message = "This username is reserved."
                    }
                });
            }

            return StatusCode(201, new
            {
                success = true,
                statusCode = 201,
                data = new
                {
                    message_key = "auth.register.verify_email_required",
                    message = "Almost there! Check your inbox and verify your email to finish signing up.",
                    requires_verification = true
                }
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
