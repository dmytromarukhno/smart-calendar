using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCalendar.Domain.Entities;

namespace SmartCalendar.Infrastructure.Persistence.Configurations;

public sealed class SceneConfiguration : IEntityTypeConfiguration<Scene>
{
    public void Configure(EntityTypeBuilder<Scene> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);

        builder.HasMany(s => s.Commands)
            .WithOne()
            .HasForeignKey(c => c.SceneId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
