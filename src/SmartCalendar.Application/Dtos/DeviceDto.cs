namespace SmartCalendar.Application.Dtos;

public sealed record DeviceDto(Guid Id, string Name, string Location, bool IsOnline);
