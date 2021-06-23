using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class CustomerDiscountEntityConfiguration : IEntityTypeConfiguration<CustomerDiscount>
    {
        public void Configure(EntityTypeBuilder<CustomerDiscount> builder)
        {
            builder.HasKey(x => new { x.Id });
            builder.HasIndex(e => new { e.UserId, e.DiscountId }, "uk_CustomerDiscount")
                    .IsUnique();

            builder.HasOne(d => d.Discount)
                .WithMany(p => p.CustomerDiscounts)
                .HasForeignKey(d => d.DiscountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_CustomerDiscounts_Discounts");

            builder.HasOne(d => d.Customer)
                .WithMany(p => p.CustomerDiscounts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_CustomerDiscounts_Customers");
        }
    }
}