using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class CustomerEntityConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(e => e.UserId)
                    .HasName("Customers_pkey");

            builder.Property(e => e.UserId).ValueGeneratedNever();

            builder.HasOne(d => d.CustomerScaleNavigation)
                .WithMany(p => p.Customers)
                .HasForeignKey(d => d.CustomerScaleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Customers_CustomerScaleId_fkey");

            builder.HasOne(d => d.Demographic)
                .WithMany(p => p.Customers)
                .HasForeignKey(d => d.DemographicId)
                .HasConstraintName("Customers_DemographicId_fkey");

            builder.HasOne(d => d.Industry)
                .WithMany(p => p.Customers)
                .HasForeignKey(d => d.IndustryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Customers_IndustryId_fkey");
        }
    }
}