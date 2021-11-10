using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class VoteEntityConfiguration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            builder.HasKey(x => new { x.Id });
            builder.Property(e => e.VoteName)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(x => x.VoteValue).IsRequired();
        }
    }
}