using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class ProjectPlatformEntityConfiguration : IEntityTypeConfiguration<ProjectPlatform>
    {
        public void Configure(EntityTypeBuilder<ProjectPlatform> builder)
        {
            builder.HasKey(x => new { x.Id });

            builder.HasIndex(e => new { e.ProjectId, e.GamePlatformId }, "uk_ProjectPlatform")
                    .IsUnique();

            builder.HasOne(d => d.GamePlatform)
                .WithMany(p => p.ProjectPlatforms)
                .HasForeignKey(d => d.GamePlatformId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ProjectPlatforms_GamePlatforms");

            builder.HasOne(d => d.Project)
                .WithMany(p => p.ProjectPlatforms)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ProjectPlatforms_CustomerProjects");

            builder.Property(x => x.ProjectId).IsRequired();
            builder.Property(x => x.GamePlatformId).IsRequired();
        }


    }
}