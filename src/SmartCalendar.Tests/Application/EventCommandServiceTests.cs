using FluentAssertions;
using Moq;
using SmartCalendar.Application.Dtos;
using SmartCalendar.Application.Services;
using SmartCalendar.Domain.Entities;
using SmartCalendar.Domain.Interfaces;

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
}
