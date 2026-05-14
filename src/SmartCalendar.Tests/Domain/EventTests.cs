using FluentAssertions;
using SmartCalendar.Domain.Constants;
using SmartCalendar.Domain.Entities;
using SmartCalendar.Domain.Exceptions;
using SmartCalendar.Tests.TestHelpers;

namespace SmartCalendar.Tests.Domain;

public sealed class EventTests
{
    [Fact]
    public void Constructor_WithValidArgs_CreatesEvent()
    {
        var start = DateTime.UtcNow.AddHours(1);
        var end = start.AddHours(2);

        var @event = new Event("Cinema", start, end);

        @event.Title.Should().Be("Cinema");
        @event.StartTime.Should().Be(start);
        @event.EndTime.Should().Be(end);
        @event.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_WithEmptyTitle_ThrowsDomainException()
    {
        var act = () => new Event(string.Empty, DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        act.Should().Throw<DomainException>().WithMessage("*Title*");
    }

    [Fact]
    public void Constructor_WithEndBeforeStart_ThrowsDomainException()
    {
        var start = DateTime.UtcNow.AddHours(2);
        var end = DateTime.UtcNow.AddHours(1);

        var act = () => new Event("Test", start, end);
        act.Should().Throw<DomainException>().WithMessage("*End must be after start*");
    }

    [Fact]
    public void AddReminder_AddsReminderToCollection()
    {
        var @event = EventFactory.CreateValid();

        @event.AddReminder(ReminderDefaults.FirstWarningMinutes);

        @event.Reminders.Should().HaveCount(1);
        @event.Reminders.First().OffsetMinutes.Should().Be(ReminderDefaults.FirstWarningMinutes);
    }

    [Fact]
    public void IsActiveAt_WhenInsideRange_ReturnsTrue()
    {
        var start = DateTime.UtcNow;
        var end = start.AddHours(2);
        var @event = new Event("Test", start, end);

        @event.IsActiveAt(start.AddHours(1)).Should().BeTrue();
    }

    [Fact]
    public void IsActiveAt_WhenOutsideRange_ReturnsFalse()
    {
        var start = DateTime.UtcNow.AddHours(1);
        var @event = new Event("Test", start, start.AddHours(1));

        @event.IsActiveAt(start.AddHours(3)).Should().BeFalse();
    }
}
