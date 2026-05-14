using Microsoft.EntityFrameworkCore;
using SmartCalendar.Domain.Entities;
using SmartCalendar.Domain.Interfaces;

namespace SmartCalendar.Infrastructure.Persistence.Repositories;

public sealed class EfDeviceRepository : IDeviceRepository
{
    private readonly SmartCalendarDbContext _db;

    public EfDeviceRepository(SmartCalendarDbContext db) => _db = db;

    public async Task<Device?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Devices.FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<IEnumerable<Device>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Devices.ToListAsync(ct);

    public async Task AddAsync(Device device, CancellationToken ct = default) =>
        await _db.Devices.AddAsync(device, ct);
}
