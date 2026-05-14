using SmartCalendar.Application.Dtos;
using SmartCalendar.Domain.Interfaces;

namespace SmartCalendar.Application.Services;

public sealed class DeviceQueryService
{
    private readonly IDeviceRepository _devices;

    public DeviceQueryService(IDeviceRepository devices) => _devices = devices;

    public async Task<IEnumerable<DeviceDto>> GetAllAsync(CancellationToken ct = default)
    {
        var devices = await _devices.GetAllAsync(ct);
        return devices.Select(d => new DeviceDto(d.Id, d.Name, d.Location, d.IsOnline));
    }
}
