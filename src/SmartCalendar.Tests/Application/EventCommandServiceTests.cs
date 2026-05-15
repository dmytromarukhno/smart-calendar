using FluentAssertions;
using Moq;
using SmartCalendar.Application.Dtos;
using SmartCalendar.Application.Services;
using SmartCalendar.Domain.Entities;
using SmartCalendar.Domain.Interfaces;
using SmartCalendar.Tests.TestHelpers;

namespace SmartCalendar.Tests.Application;

public sealed class EventCommandServiceTests
{
    private readonly Mock<IEventRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly EventCommandService _sut;

    public EventCommandServiceTests()
    {
        _sut = new EventCommandService(_repoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_ReturnsNewGuid()
    {
        var dto = new CreateEventDto("Meeting",
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2));

        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var id = await _sut.CreateAsync(dto);

        id.Should().NotBeEmpty();
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_CallsReplaceRemindersWithNewOffsets()
    {
        var eventId = Guid.NewGuid();
        var start = DateTime.UtcNow.AddHours(1);
        var existingEvent = EventFactory.CreateValid(start: start, end: start.AddHours(1));
        var offsets = new List<int> { 10, 30 };

        _repoMock.Setup(r => r.GetByIdAsync(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEvent);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var dto = new UpdateEventDto(eventId, "Updated", start, start.AddHours(1),
            false, "", offsets);
        await _sut.UpdateAsync(dto);

        _repoMock.Verify(r => r.ReplaceRemindersAsync(eventId, offsets, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AttachSceneAsync_CallsReplaceSchedule()
    {
        var eventId = Guid.NewGuid();
        var sceneId = Guid.NewGuid();
        var start = DateTime.UtcNow.AddHours(1);

        _repoMock.Setup(r => r.GetByIdAsync(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(EventFactory.CreateValid(start: start, end: start.AddHours(1)));
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _sut.AttachSceneAsync(eventId, sceneId);

        _repoMock.Verify(r => r.ReplaceScheduleAsync(
            eventId, sceneId, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenEventNotFound_ThrowsEventNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Event?)null);

        var act = () => _sut.DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<Exception>();
    }
}
