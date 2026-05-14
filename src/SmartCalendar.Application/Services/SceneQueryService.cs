using SmartCalendar.Application.Dtos;
using SmartCalendar.Domain.Interfaces;

namespace SmartCalendar.Application.Services;

public sealed class SceneQueryService
{
    private readonly ISceneRepository _scenes;

    public SceneQueryService(ISceneRepository scenes) => _scenes = scenes;

    public async Task<IEnumerable<SceneDto>> GetAllAsync(CancellationToken ct = default)
    {
        var scenes = await _scenes.GetAllAsync(ct);
        return scenes.Select(s => new SceneDto(s.Id, s.Name, s.Commands.Count));
    }
}
