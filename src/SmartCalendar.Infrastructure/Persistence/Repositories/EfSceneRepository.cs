using Microsoft.EntityFrameworkCore;
using SmartCalendar.Domain.Entities;
using SmartCalendar.Domain.Interfaces;

namespace SmartCalendar.Infrastructure.Persistence.Repositories;

public sealed class EfSceneRepository : ISceneRepository
{
    private readonly SmartCalendarDbContext _db;

    public EfSceneRepository(SmartCalendarDbContext db) => _db = db;

    public async Task<Scene?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Scenes
            .Include(s => s.Commands)
            .ThenInclude(c => c.Device)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IEnumerable<Scene>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Scenes
            .Include(s => s.Commands)
            .ToListAsync(ct);

    public async Task AddAsync(Scene scene, CancellationToken ct = default) =>
        await _db.Scenes.AddAsync(scene, ct);

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var scene = await GetByIdAsync(id, ct);
        if (scene is not null)
            _db.Scenes.Remove(scene);
    }
}
