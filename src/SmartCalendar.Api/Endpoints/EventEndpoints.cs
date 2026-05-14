using SmartCalendar.Application.Dtos;
using SmartCalendar.Application.Services;
using SmartCalendar.Domain.Exceptions;

namespace SmartCalendar.Api.Endpoints;

public static class EventEndpoints
{
    public static WebApplication MapEventEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/events").WithOpenApi();

        group.MapGet("/", GetEventsAsync).WithName("GetEvents");
        group.MapGet("/{id:guid}", GetEventByIdAsync).WithName("GetEventById");
        group.MapPost("/", CreateEventAsync).WithName("CreateEvent");
        group.MapPut("/{id:guid}", UpdateEventAsync).WithName("UpdateEvent");
        group.MapDelete("/{id:guid}", DeleteEventAsync).WithName("DeleteEvent");
        group.MapPost("/{eventId:guid}/scenes/{sceneId:guid}", AttachSceneAsync)
            .WithName("AttachSceneToEvent");

        return app;
    }

    private static async Task<IResult> GetEventsAsync(
        DateTime? from, DateTime? to,
        EventQueryService svc, CancellationToken ct)
    {
        if (from.HasValue && to.HasValue)
            return Results.Ok(await svc.GetForDateRangeAsync(from.Value, to.Value, ct));
        return Results.Ok(await svc.GetAllAsync(ct));
    }

    private static async Task<IResult> GetEventByIdAsync(
        Guid id, EventQueryService svc, CancellationToken ct)
    {
        try
        {
            return Results.Ok(await svc.GetByIdAsync(id, ct));
        }
        catch (EventNotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> CreateEventAsync(
        CreateEventDto dto, EventCommandService svc, CancellationToken ct)
    {
        var id = await svc.CreateAsync(dto, ct);
        return Results.Created($"/api/events/{id}", new { id });
    }

    private static async Task<IResult> UpdateEventAsync(
        Guid id, UpdateEventDto dto, EventCommandService svc, CancellationToken ct)
    {
        try
        {
            await svc.UpdateAsync(dto with { Id = id }, ct);
            return Results.Ok();
        }
        catch (EventNotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> DeleteEventAsync(
        Guid id, EventCommandService svc, CancellationToken ct)
    {
        try
        {
            await svc.DeleteAsync(id, ct);
            return Results.NoContent();
        }
        catch (EventNotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> AttachSceneAsync(
        Guid eventId, Guid sceneId, EventCommandService svc, CancellationToken ct)
    {
        try
        {
            await svc.AttachSceneAsync(eventId, sceneId, ct);
            return Results.Ok();
        }
        catch (EventNotFoundException)
        {
            return Results.NotFound();
        }
    }
}
