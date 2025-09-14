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
using Wyvern.Mailer;
using Wyvern.Utils.Cryptography;

namespace Wyvern.API.Controllers
{
    [ApiController]
    [Route("auth/register")]
    public class RegisterController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWaitlistRepository _waitlistRepository;
        private readonly LocaleService _localeService;
        private readonly ITokenRepository _tokenRepository;
        private readonly EmailService _mailer;

        public RegisterController(
            AppDbContext db,
            IWaitlistRepository waitlistRepository,
            LocaleService localeService,
            ITokenRepository tokenRepository,
            EmailService mailer)
        {
            _db = db;
            _waitlistRepository = waitlistRepository;
            _localeService = localeService;
            _tokenRepository = tokenRepository;
            _mailer = mailer;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterRequest model)
        {
            model.Locale ??= "en_us";
            string country = Request.Headers.TryGetValue("CF-IPCountry", out var cfCountry) && !string.IsNullOrWhiteSpace(cfCountry)
                ? cfCountry.ToString()
                : "US";

            var usernameResult = await UsernameValidator.CheckAsync(model.Username);
            if (!usernameResult.Success)
            {
                var msg = _localeService.GetString(usernameResult.MessageKey!, model.Locale);
                return BadRequest(new { success = false, statusCode = 400, data = new { message = msg } });
            }

            bool usernameTaken = await _db.Users.AnyAsync(u => u.Username == model.Username);
            if (usernameTaken)
            {
                return BadRequest(new
                {
                    success = false,
                    statusCode = 400,
                    data = new { message = _localeService.GetString("API.Auth.Register.UsernameTaken", model.Locale) }
                });
            }

            var emailResult = await EmailValidator.CheckAsync(model.Email);
            if (!emailResult.Success)
            {
                var msg = _localeService.GetString(emailResult.MessageKey!, model.Locale);
                if (emailResult.Data != null && emailResult.Data.TryGetValue("domain", out var domain))
                    msg = msg.Replace("{domain}", domain?.ToString() ?? "");
                return BadRequest(new { success = false, statusCode = 400, data = new { message = msg } });
            }

            bool emailTaken = await _db.Users.AnyAsync(u => u.Email == model.Email);
            if (emailTaken)
            {
                return BadRequest(new
                {
                    success = false,
                    statusCode = 400,
                    data = new { message = _localeService.GetString("API.Auth.Register.EmailTaken", model.Locale) }
                });
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

            string userId = IdGen.GenerateId();
            string hashedPassword = GenerateHash.HashGenerator(model.Password);

            var user = new User
            {
                Id = userId,
                Username = model.Username.ToLowerInvariant(),
                Email = model.Email,
                Password = hashedPassword,
                Birthday = model.Birthday,
                Locale = model.Locale!,
                Region = country,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            string verify_token = TokenGen.GenerateToken(30);
            string verify_link = $"https://app.wyvern.gg/verify-email?token={verify_token}";

            Console.WriteLine(verify_link);

            var token = new Token
            {
                UserId = userId,
                TokenValue = verify_token,
                Type = "email_verification",
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                Used = false
            };

            _db.Users.Add(user);
            _db.Tokens.Add(token);

            await _db.SaveChangesAsync();

            await _mailer.SendEmailAsync(
                EmailType.Welcome,
                model.Locale,
                model.Username,
                model.Email,
                new { Username = model.Username, Link = verify_link }
            );

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
