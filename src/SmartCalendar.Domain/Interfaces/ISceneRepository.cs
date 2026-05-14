using SmartCalendar.Domain.Entities;

namespace SmartCalendar.Domain.Interfaces;

public interface ISceneRepository
{
    Task<Scene?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Scene>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Scene scene, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
