using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Wyvern.ConfigModel;
using Wyvern.Database.Data;
using Wyvern.Database.Repositories;
using Wyvern.Mailer;
using Npgsql;
using Wyvern.Utils;
using Wyvern.API;
using Wyvern.Utils.Generators;
using Wyvern.Utils.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

bool isProduction = Config.GetKey<bool>("production");

var npgsqlBuilder = new NpgsqlConnectionStringBuilder
{
    Host = Config.GetKey<string>("database", "postgres", "host"),
    Port = Config.GetKey<int>("database", "postgres", "port"),
    Username = Config.GetKey<string>("database", "postgres", "username"),
    Password = Config.GetKey<string>("database", "postgres", "password"),
    Database = Config.GetKey<string>("database", "postgres", "database"),
    SslMode = Enum.Parse<SslMode>(Config.GetKey<string>("database", "postgres", "ssl_mode"), ignoreCase: true),
    Timeout = Config.GetKey<int>("database", "postgres", "timeout"),
    Pooling = Config.GetKey<bool>("database", "postgres", "pooling")
};

var connectionString = npgsqlBuilder.ConnectionString;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IWaitlistRepository, WaitlistRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

var mailHost = Config.GetKey<string>("mailserver", "host");
var mailPort = Config.GetKey<int>("mailserver", "port");
var mailUser = Config.GetKey<string>("mailserver", "username");
var mailPassword = Config.GetKey<string>("mailserver", "password");
var useSsl = Config.GetKey<bool>("mailserver", "use_ssl");
var useStartTls = Config.GetKey<bool>("mailserver", "use_starttls");

builder.Services.AddSingleton(new EmailService(
    mailHost,
    mailPort,
    mailUser,
    mailPassword,
    useSsl,
    useStartTls
));

var resourcesPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Resources");
builder.Services.AddSingleton<LocaleService>(new LocaleService(resourcesPath));

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

if (!isProduction)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("The application is running in a non-production environment. This is normal in a developer environment and can be safely ignored.");
    app.UseDeveloperExceptionPage();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var existingAdmin = await db.Users.FirstOrDefaultAsync(u => u.Username == "admin");
    if (existingAdmin == null)
    {
        var adminUser = new User
        {
            Id = IdGen.GenerateId(),
            Username = "admin", // <--- Not actually possible if you register an account via the API, by the way. - Luni
            DisplayName = "Administrator",
            Email = "admin@wyvern.gg",
            Password = GenerateHash.HashGenerator("MyW0rdIsPassed!"),
            Birthday = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc),
            Locale = "en_us",
            Region = "US",
            Rank = 1,
            IsStaff = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Users.Add(adminUser);
        await db.SaveChangesAsync();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Admin user created: admin@wyvern.gg / MyW0rdIsPassed!");
        Console.ResetColor();
    }
    else
    {
        Console.WriteLine("Dev admin already exists.");
    }
}

// -------- send a test email --------
/*using (var scope = app.Services.CreateScope())
{
    var mailer = scope.ServiceProvider.GetRequiredService<EmailService>();

    await mailer.SendEmailAsync(
        EmailType.Welcome,
        "en_us",
        "Luni",
        "luni.s@wyvern.gg", 
        new { Name = "Luni" }
    );
}*/
//------------------------------------

app.Run();
