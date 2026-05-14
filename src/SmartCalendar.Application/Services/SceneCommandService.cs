using SmartCalendar.Application.Dtos;
using SmartCalendar.Domain.Entities;
using SmartCalendar.Domain.Interfaces;

namespace SmartCalendar.Application.Services;

public sealed class SceneCommandService
{
    private readonly ISceneRepository _scenes;
    private readonly IUnitOfWork _uow;

    public SceneCommandService(ISceneRepository scenes, IUnitOfWork uow)
    {
        _scenes = scenes;
        _uow = uow;
    }

    public async Task<Guid> CreateAsync(CreateSceneDto dto, CancellationToken ct = default)
    {
        var scene = new Scene(dto.Name);
        await _scenes.AddAsync(scene, ct);
        await _uow.SaveChangesAsync(ct);
        return scene.Id;
    }
}
