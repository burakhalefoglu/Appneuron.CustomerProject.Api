using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class CustomerProjectEntityConfiguration : IEntityTypeConfiguration<CustomerProject>
    {
        public void Configure(EntityTypeBuilder<CustomerProject> builder)
        {
            builder.HasKey(x => new { x.Id });
            builder.Property(e => e.CreatedAt)
                    .HasColumnType("date")
                    .HasDefaultValueSql("CURRENT_DATE");

            builder.Property(e => e.ProjectBody).HasMaxLength(100);

            builder.Property(e => e.ProjectKey)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.ProjectName)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.Statuse)
                .IsRequired()
                .HasDefaultValueSql("true");

            builder.HasOne(d => d.Customer)
                .WithMany(p => p.CustomerProjects)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_CustomerProjects_Customers");

            builder.HasOne(d => d.Vote)
                .WithMany(p => p.CustomerProjects)
                .HasForeignKey(d => d.VoteId)
                .HasConstraintName("fk_CustomerProjects_Votes");
        }


    }
}