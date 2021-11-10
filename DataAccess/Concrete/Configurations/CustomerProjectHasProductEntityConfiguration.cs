using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class CustomerProjectHasProductEntityConfiguration : IEntityTypeConfiguration<CustomerProjectHasProduct>
    {
        public void Configure(EntityTypeBuilder<CustomerProjectHasProduct> builder)
        {
            builder.HasKey(x => new { x.Id });
            builder.HasIndex(e => new { e.ProductId, e.ProjectId }, "Uk_CustomerProjectsProduct")
                .IsUnique();

            builder.HasOne(d => d.Product)
                .WithMany(p => p.CustomerProjectHasProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_CustomerHasProducts_Products");

            builder.HasOne(d => d.Project)
                .WithMany(p => p.CustomerProjectHasProducts)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_CustomerHasProducts_CustomerProjects");
        }
    }
}