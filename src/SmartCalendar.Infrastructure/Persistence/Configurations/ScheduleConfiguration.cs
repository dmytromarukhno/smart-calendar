using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCalendar.Domain.Entities;

namespace SmartCalendar.Infrastructure.Persistence.Configurations;

public sealed class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.Scene)
            .WithMany()
            .HasForeignKey(s => s.SceneId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
