using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wyvern.ConfigModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

bool isProduction = Config.GetKey<bool>("production");

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
