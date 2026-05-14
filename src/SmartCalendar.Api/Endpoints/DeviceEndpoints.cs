using SmartCalendar.Application.Services;

namespace SmartCalendar.Api.Endpoints;

public static class DeviceEndpoints
{
    public static WebApplication MapDeviceEndpoints(this WebApplication app)
    {
        app.MapGet("/api/devices", GetDevicesAsync)
            .WithName("GetDevices")
            .WithOpenApi();

        return app;
    }

    private static async Task<IResult> GetDevicesAsync(
        DeviceQueryService svc, CancellationToken ct) =>
        Results.Ok(await svc.GetAllAsync(ct));
}
