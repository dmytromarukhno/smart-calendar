using Microsoft.Extensions.Logging;
using SmartCalendar.Application.Interfaces;
using SmartCalendar.Domain.Entities;
using SmartCalendar.Domain.Exceptions;
using SmartCalendar.Domain.Interfaces;
using SmartCalendar.Domain.ValueObjects;

namespace SmartCalendar.Application.Services;

public sealed class SceneExecutor
{
    private readonly IMqttPublisher _mqtt;
    private readonly ISceneRepository _scenes;
    private readonly ILogger<SceneExecutor> _log;

    public SceneExecutor(IMqttPublisher mqtt, ISceneRepository scenes, ILogger<SceneExecutor> log)
    {
        _mqtt = mqtt;
        _scenes = scenes;
        _log = log;
    }

    public async Task ExecuteAsync(Guid sceneId, CancellationToken ct = default)
    {
        var scene = await GetSceneOrThrowAsync(sceneId, ct);
        foreach (var command in scene.Commands.OrderBy(c => c.Order))
            await PublishCommandAsync(command, ct);
    }

    private async Task<Scene> GetSceneOrThrowAsync(Guid id, CancellationToken ct)
        => await _scenes.GetByIdAsync(id, ct)
           ?? throw new SceneNotFoundException(id);

    private async Task PublishCommandAsync(Command cmd, CancellationToken ct)
    {
        var topic = BuildMqttTopic(cmd.Device);
        var payload = new MqttCommandPayload(cmd.Action, cmd.Value);
        await _mqtt.PublishAsync(topic, payload, ct);
        _log.LogInformation("Published {Topic} {@Payload}", topic, payload);
    }

    private static string BuildMqttTopic(Device d) =>
        $"smart/{d.Location}/{d.Name.ToLowerInvariant().Replace(" ", "-")}";
}
