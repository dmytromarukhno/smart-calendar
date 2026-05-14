using SmartCalendar.Domain.Entities;

namespace SmartCalendar.Domain.Interfaces;

public interface IDeviceRepository
{
    Task<Device?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Device>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Device device, CancellationToken ct = default);
}
