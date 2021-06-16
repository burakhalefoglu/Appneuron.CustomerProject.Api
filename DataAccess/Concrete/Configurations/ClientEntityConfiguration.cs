using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Concrete.Configurations
{
    public class ClientEntityConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.Property(e => e.ClientId)
                    .IsRequired()
                    .HasMaxLength(500);

            builder.Property(e => e.CreatedAt)
                .HasColumnType("date")
                .HasDefaultValueSql("CURRENT_DATE");

            builder.Property(e => e.IsPaidClient).HasColumnName("isPaidClient");

            builder.HasOne(d => d.CustomerProject)
               .WithMany(p => p.Clients)
               .HasForeignKey(d => d.ProjectId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_Client_CustomerProject_id");
        }
    }
}
