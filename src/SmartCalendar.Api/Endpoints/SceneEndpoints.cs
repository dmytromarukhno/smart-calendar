using SmartCalendar.Application.Dtos;
using SmartCalendar.Application.Services;

namespace SmartCalendar.Api.Endpoints;

public static class SceneEndpoints
{
    public static WebApplication MapSceneEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/scenes").WithOpenApi();

        group.MapGet("/", GetScenesAsync).WithName("GetScenes");
        group.MapPost("/", CreateSceneAsync).WithName("CreateScene");

        return app;
    }

    private static async Task<IResult> GetScenesAsync(
        SceneQueryService svc, CancellationToken ct) =>
        Results.Ok(await svc.GetAllAsync(ct));

    private static async Task<IResult> CreateSceneAsync(
        CreateSceneDto dto, SceneCommandService svc, CancellationToken ct)
    {
        var id = await svc.CreateAsync(dto, ct);
        return Results.Created($"/api/scenes/{id}", new { id });
    }
}
