using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class CustomerDemographicEntityConfiguration : IEntityTypeConfiguration<CustomerDemographic>
    {
        public void Configure(EntityTypeBuilder<CustomerDemographic> builder)
        {
            builder.HasKey(x => new { x.Id });
            builder.Property(e => e.CustomerDesc)
                               .IsRequired()
                               .HasMaxLength(50);
        }


    }
}