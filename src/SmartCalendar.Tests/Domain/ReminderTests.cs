using FluentAssertions;
using SmartCalendar.Domain.Entities;

namespace SmartCalendar.Tests.Domain;

public sealed class ReminderTests
{
    [Fact]
    public void MarkSent_SetsIsSentToTrue()
    {
        var reminder = new Reminder(Guid.NewGuid(), 10);

        reminder.MarkSent();

        reminder.IsSent.Should().BeTrue();
    }

    [Fact]
    public void Constructor_SetsIsSentFalse()
    {
        var reminder = new Reminder(Guid.NewGuid(), 5);

        reminder.IsSent.Should().BeFalse();
    }
}
