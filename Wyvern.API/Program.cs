using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Wyvern.ConfigModel;
using Wyvern.Database.Data;
using Wyvern.Database.Repositories;
using Npgsql;

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

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

if (!isProduction)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("The application is running in a non-production environment. This is normal in a developer environment and can be safely ignored.");
    app.UseDeveloperExceptionPage();
}

app.Run();
