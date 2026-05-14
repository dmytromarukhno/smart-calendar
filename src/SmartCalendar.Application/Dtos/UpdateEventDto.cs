namespace SmartCalendar.Application.Dtos;

public sealed record UpdateEventDto(
    Guid Id,
    string Title,
    DateTime Start,
    DateTime End,
    bool IsRecurring,
    string RecurrencePattern,
    int ReminderOffsetMin);
