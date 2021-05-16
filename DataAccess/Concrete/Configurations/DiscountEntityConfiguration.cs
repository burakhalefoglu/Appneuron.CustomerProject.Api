using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class DiscountEntityConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.HasKey(x => new { x.Id });
            builder.Property(e => e.DiscountName)
                           .IsRequired()
                           .HasMaxLength(50);
            builder.Property(x => x.Percent).IsRequired();
        }


    }
}