using Microsoft.EntityFrameworkCore;
using Serilog;
using SmartCalendar.Api.Endpoints;
using SmartCalendar.Application;
using SmartCalendar.Infrastructure;
using SmartCalendar.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console());

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// HttpClient for Blazor components → same app's REST API
builder.Services.AddScoped(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var baseUrl = cfg["ApiBaseUrl"] ?? "http://localhost:5123";
    return new HttpClient { BaseAddress = new Uri(baseUrl) };
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations and seed data
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SmartCalendarDbContext>();
    await db.Database.MigrateAsync();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await SeedData.InitializeAsync(db, logger);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();

app.MapEventEndpoints();
app.MapSceneEndpoints();
app.MapDeviceEndpoints();

app.MapRazorComponents<SmartCalendar.Api.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();

public partial class Program { }
