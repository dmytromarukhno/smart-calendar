using SmartCalendar.Domain.Exceptions;

namespace SmartCalendar.Domain.Entities;

public sealed class Device
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Location { get; private set; } = null!;
    public bool IsOnline { get; private set; }

    // Required by EF Core
    private Device() { }

    public Device(string name, string location)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Device name is required");
        if (string.IsNullOrWhiteSpace(location))
            throw new DomainException("Device location is required");

        Id = Guid.NewGuid();
        Name = name;
        Location = location;
        IsOnline = true;
    }

    public void SetOnline(bool isOnline) => IsOnline = isOnline;
}
