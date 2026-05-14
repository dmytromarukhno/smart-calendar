namespace SmartCalendar.Application.Dtos;

public sealed record CreateEventDto(
    string Title,
    DateTime Start,
    DateTime End,
    bool IsRecurring = false,
    string RecurrencePattern = "");
