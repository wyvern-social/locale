using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Wyvern.ConfigModel;
using Wyvern.Utils.Validators;
using Wyvern.Database.Data;
using Microsoft.EntityFrameworkCore;
using Wyvern.Database.Repositories;
using Wyvern.Utils.Generators;

namespace Wyvern.API.Controllers
{
    [ApiController]
    [Route("auth/register")]
    public class RegisterController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWaitlistRepository _waitlistRepository;
        private readonly LocaleService _localeService;

        public RegisterController(
            AppDbContext db,
            IWaitlistRepository waitlistRepository,
            LocaleService localeService)
        {
            _db = db;
            _waitlistRepository = waitlistRepository;
            _localeService = localeService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterRequest model)
        {
            model.Locale ??= "en_us";

            var usernameResult = await UsernameValidator.CheckAsync(model.Username);
            if (!usernameResult.Success)
            {
                var msg = _localeService.GetString(usernameResult.MessageKey!, model.Locale);
                return BadRequest(new { success = false, statusCode = 400, data = new { message = msg } });
            }

            var emailResult = await EmailValidator.CheckAsync(model.Email);
            if (!emailResult.Success)
            {
                var msg = _localeService.GetString(emailResult.MessageKey!, model.Locale);
                if (emailResult.Data != null && emailResult.Data.TryGetValue("domain", out var domain))
                    msg = msg.Replace("{domain}", domain?.ToString() ?? "");
                return BadRequest(new { success = false, statusCode = 400, data = new { message = msg } });
            }

            var passResult = await PasswordValidator.CheckAsync(model.Password);
            if (!passResult.Success)
            {
                var msg = _localeService.GetString(passResult.MessageKey!, model.Locale);
                return BadRequest(new { success = false, statusCode = 400, data = new { message = msg } });
            }

            var ageResult = AgeValidator.Check(model.Birthday);
            if (!ageResult.Success)
            {
                var msg = _localeService.GetString(ageResult.MessageKey!, model.Locale);
                int age = DateTime.UtcNow.Year - model.Birthday.Year;
                if (DateTime.UtcNow < model.Birthday.AddYears(age)) age--;
                msg = msg.Replace("{age}", age.ToString());
                return BadRequest(new { success = false, statusCode = 400, data = new { message = msg } });
            }

            bool isInviteOnly = Config.GetKey<bool>("api", "registration", "invite_only");
            if (isInviteOnly)
            {
                if (string.IsNullOrWhiteSpace(model.InviteCode))
                    return StatusCode(403, new { success = false, statusCode = 403, data = new { message = _localeService.GetString("API.Auth.Register.InviteOnly", model.Locale) } });

                var validInvite = "AB12-CD34"; // Example
                if (model.InviteCode != validInvite)
                    return StatusCode(403, new { success = false, statusCode = 403, data = new { message = _localeService.GetString("API.Auth.Register.InvalidInvite", model.Locale) } });
            }

            var waitlistEntry = await _waitlistRepository.GetByUsernameAsync(model.Username);
            if (waitlistEntry != null && !string.Equals(waitlistEntry.Email, model.Email, StringComparison.OrdinalIgnoreCase))
                return StatusCode(403, new { success = false, statusCode = 403, data = new { message = _localeService.GetString("API.Auth.Register.UsernameReserved", model.Locale) } });

            string user_id = IdGen.GenerateId();
            string verify_token = TokenGen.GenerateToken(30);

            return StatusCode(201, new
            {
                success = true,
                statusCode = 201,
                data = new { message = _localeService.GetString("API.Auth.Register.VerifyEmailRequired", model.Locale) }
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
