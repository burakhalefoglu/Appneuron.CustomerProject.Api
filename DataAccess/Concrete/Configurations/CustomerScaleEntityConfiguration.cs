using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class CustomerScaleEntityConfiguration : IEntityTypeConfiguration<CustomerScale>
    {
        public void Configure(EntityTypeBuilder<CustomerScale> builder)
        {
            builder.HasKey(x => new { x.Id });
            builder.Property(e => e.Description).HasMaxLength(100);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
        }


    }
}