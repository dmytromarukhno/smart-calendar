namespace SmartCalendar.Domain.Exceptions;

public sealed class DeviceOfflineException : DomainException
{
    public Guid DeviceId { get; }

    public DeviceOfflineException(Guid deviceId)
        : base($"Device {deviceId} is offline") => DeviceId = deviceId;
}
