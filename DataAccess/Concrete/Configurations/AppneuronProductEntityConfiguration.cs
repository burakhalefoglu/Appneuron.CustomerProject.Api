using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class AppneuronProductEntityConfiguration : IEntityTypeConfiguration<AppneuronProduct>
    {
        public void Configure(EntityTypeBuilder<AppneuronProduct> builder)
        {
            builder.HasKey(x => new { x.Id });
            builder.Property(e => e.ProductName)
                              .IsRequired()
                              .HasMaxLength(50);
        }


    }
}