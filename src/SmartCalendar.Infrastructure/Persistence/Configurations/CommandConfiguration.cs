using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCalendar.Domain.Entities;

namespace SmartCalendar.Infrastructure.Persistence.Configurations;

public sealed class CommandConfiguration : IEntityTypeConfiguration<Command>
{
    public void Configure(EntityTypeBuilder<Command> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Action).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Value).HasMaxLength(500);

        builder.HasOne(c => c.Device)
            .WithMany()
            .HasForeignKey(c => c.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
