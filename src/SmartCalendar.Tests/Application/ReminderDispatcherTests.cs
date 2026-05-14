using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmartCalendar.Application.Services;
using SmartCalendar.Domain.Entities;
using SmartCalendar.Domain.Interfaces;
using SmartCalendar.Tests.TestHelpers;

namespace SmartCalendar.Tests.Application;

public sealed class ReminderDispatcherTests
{
    private readonly Mock<IEventRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly ReminderDispatcher _sut;

    public ReminderDispatcherTests()
    {
        _sut = new ReminderDispatcher(
            _repoMock.Object,
            _uowMock.Object,
            NullLogger<ReminderDispatcher>.Instance);
    }

    [Fact]
    public async Task DispatchPendingAsync_MarksOverdueRemindersAsSent()
    {
        var start = DateTime.UtcNow.AddMinutes(5);
        var @event = new Event("Test", start, start.AddHours(1));
        @event.AddReminder(10); // trigger at start - 10 min = 5 min ago

        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { @event });
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _sut.DispatchPendingAsync();

        @event.Reminders.First().IsSent.Should().BeTrue();
    }
}
