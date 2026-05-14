using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCalendar.Application.Interfaces;
using SmartCalendar.Domain.Interfaces;
using SmartCalendar.Infrastructure.Mqtt;
using SmartCalendar.Infrastructure.Persistence;
using SmartCalendar.Infrastructure.Persistence.Repositories;
using SmartCalendar.Infrastructure.Scheduling;

namespace SmartCalendar.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=smart-calendar.db";

        services.AddDbContext<SmartCalendarDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SmartCalendarDbContext>());
        services.AddScoped<IEventRepository, EfEventRepository>();
        services.AddScoped<ISceneRepository, EfSceneRepository>();
        services.AddScoped<IDeviceRepository, EfDeviceRepository>();

        services.AddSingleton<IMqttPublisher, ConsoleMqttPublisher>();
        services.AddHostedService<TimerBasedSceneScheduler>();

        return services;
    }
}
