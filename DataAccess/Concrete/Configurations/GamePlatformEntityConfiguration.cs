using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class GamePlatformEntityConfiguration : IEntityTypeConfiguration<GamePlatform>
    {
        public void Configure(EntityTypeBuilder<GamePlatform> builder)
        {
            builder.HasKey(x => new { x.Id });
            builder.Property(e => e.PlatformDescription).HasMaxLength(100);

            builder.Property(e => e.PlatformName)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}