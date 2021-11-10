using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class InvoiceEntityConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(x => new { x.Id });
            builder.Property(e => e.BillNo)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.CreatedAt)
                .HasColumnType("date")
                .HasDefaultValueSql("CURRENT_DATE");

            builder.Property(e => e.LastPaymentTime).HasColumnType("date");

            builder.HasOne(d => d.Discount)
                .WithMany(p => p.Invoices)
                .HasForeignKey(d => d.DiscountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Invoices_Discounts");
        }
    }
}