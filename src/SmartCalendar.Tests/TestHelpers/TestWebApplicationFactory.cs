using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmartCalendar.Infrastructure.Persistence;

namespace SmartCalendar.Tests.TestHelpers;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"sc-test-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // ConfigureAppConfiguration is too early — AddInfrastructure reads the connection string
        // at service-registration time (before ConfigureWebHost runs), so the config override
        // never reaches the already-registered DbContextOptions. Replace the options directly.
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<SmartCalendarDbContext>>();
            services.AddDbContext<SmartCalendarDbContext>(
                options => options.UseSqlite($"Data Source={_dbPath}"),
                ServiceLifetime.Scoped);
        });
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await base.DisposeAsync();
        foreach (var suffix in new[] { "", "-shm", "-wal" })
        {
            var path = _dbPath + suffix;
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
