namespace SmartCalendar.Domain.Exceptions;

public sealed class DeviceNotFoundException : DomainException
{
    public Guid DeviceId { get; }

    public DeviceNotFoundException(Guid id)
        : base($"Device {id} not found") => DeviceId = id;
}
