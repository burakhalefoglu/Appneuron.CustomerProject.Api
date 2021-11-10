﻿using System;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Entities
{
    public class FakeDbContext : DbContext
    {
        public FakeDbContext()
        {
        }

        public FakeDbContext(DbContextOptions<FakeDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Client> Clients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql(
                    "Host=localhost;Port=5432;Database=Appneuron.CustomerProject;Username=burak;Password=Developer123");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "en_US.utf8");

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("date")
                    .HasDefaultValueSql("CURRENT_DATE");

                entity.Property(e => e.IsPaidClient).HasColumnName("isPaidClient");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        private void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }
    }
}