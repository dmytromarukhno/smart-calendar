using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmartCalendar.Application.Interfaces;
using SmartCalendar.Application.Services;
using SmartCalendar.Domain.Entities;
using SmartCalendar.Domain.Exceptions;
using SmartCalendar.Domain.Interfaces;
using SmartCalendar.Domain.ValueObjects;

namespace SmartCalendar.Tests.Application;

public sealed class SceneExecutorTests
{
    private readonly Mock<IMqttPublisher> _mqttMock = new();
    private readonly Mock<ISceneRepository> _sceneRepoMock = new();
    private readonly SceneExecutor _sut;

    public SceneExecutorTests()
    {
        _sut = new SceneExecutor(
            _mqttMock.Object,
            _sceneRepoMock.Object,
            NullLogger<SceneExecutor>.Instance);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidScene_PublishesCommands()
    {
        var sceneId = Guid.NewGuid();
        var device = new Device("Light", "livingroom");
        var scene = new Scene("Cinema");
        scene.AddCommand(new Command(sceneId, device, "dim", "20", 1));

        _sceneRepoMock.Setup(r => r.GetByIdAsync(sceneId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(scene);

        await _sut.ExecuteAsync(sceneId);

        _mqttMock.Verify(m => m.PublishAsync(
            It.IsAny<string>(),
            It.IsAny<MqttCommandPayload>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnknownScene_ThrowsSceneNotFoundException()
    {
        var sceneId = Guid.NewGuid();
        _sceneRepoMock.Setup(r => r.GetByIdAsync(sceneId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Scene?)null);

        var act = async () => await _sut.ExecuteAsync(sceneId);

        await act.Should().ThrowAsync<SceneNotFoundException>()
            .Where(e => e.SceneId == sceneId);
    }
}
