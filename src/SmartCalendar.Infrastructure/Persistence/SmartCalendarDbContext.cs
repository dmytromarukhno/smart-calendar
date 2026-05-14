using Microsoft.EntityFrameworkCore;
using SmartCalendar.Domain.Entities;
using SmartCalendar.Domain.Interfaces;

namespace SmartCalendar.Infrastructure.Persistence;

public sealed class SmartCalendarDbContext : DbContext, IUnitOfWork
{
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Reminder> Reminders => Set<Reminder>();
    public DbSet<Scene> Scenes => Set<Scene>();
    public DbSet<Command> Commands => Set<Command>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<User> Users => Set<User>();

    public SmartCalendarDbContext(DbContextOptions<SmartCalendarDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartCalendarDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        base.SaveChangesAsync(ct);
}
