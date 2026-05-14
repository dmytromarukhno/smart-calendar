using Microsoft.Extensions.DependencyInjection;
using SmartCalendar.Application.Services;

namespace SmartCalendar.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<EventCommandService>();
        services.AddScoped<EventQueryService>();
        services.AddScoped<SceneCommandService>();
        services.AddScoped<SceneQueryService>();
        services.AddScoped<DeviceQueryService>();
        services.AddScoped<SceneExecutor>();
        services.AddScoped<ReminderDispatcher>();
        return services;
    }
}
