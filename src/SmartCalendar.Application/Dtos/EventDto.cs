namespace SmartCalendar.Application.Dtos;

public sealed record EventDto(
    Guid Id,
    string Title,
    DateTime StartTime,
    DateTime EndTime,
    bool IsRecurring,
    string RecurrencePattern,
    bool HasScene = false,
    IReadOnlyList<int>? ReminderOffsets = null,
    Guid? AttachedSceneId = null,
    string? AttachedSceneName = null);
