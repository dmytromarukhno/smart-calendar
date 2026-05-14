using SmartCalendar.Domain.Exceptions;

namespace SmartCalendar.Domain.Entities;

public sealed class Command
{
    public Guid Id { get; private set; }
    public Guid SceneId { get; private set; }
    public Guid DeviceId { get; private set; }
    public Device Device { get; private set; } = null!;
    public string Action { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public int Order { get; private set; }

    // Required by EF Core
    private Command() { }

    public Command(Guid sceneId, Guid deviceId, string action, string value, int order)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new DomainException("Action is required");

        Id = Guid.NewGuid();
        SceneId = sceneId;
        DeviceId = deviceId;
        Action = action;
        Value = value;
        Order = order;
    }

    // Convenience constructor for in-memory/test scenarios where Device is already resolved
    public Command(Guid sceneId, Device device, string action, string value, int order)
        : this(sceneId, device.Id, action, value, order)
    {
        Device = device;
    }
}
