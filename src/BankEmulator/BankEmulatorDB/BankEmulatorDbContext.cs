using System;
using System.Collections.Generic;
using BankEmulatorDB.Entities;
using Common.Generics;
using Microsoft.EntityFrameworkCore;

namespace BankEmulatorDB
{
    public class BankEmulatorDbContext : DbContextSaveChangesOverride<BankEmulatorDbContext>
    {
        public BankEmulatorDbContext() { }

        public BankEmulatorDbContext(DbContextOptions<BankEmulatorDbContext> options) : base(options) { }

        public DbSet<AccountEntity> Accounts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountEntity>(builder =>
            {
                builder.HasKey(request => request.CardNumber);
                builder.HasIndex(request => new
                {
                    request.CardNumber,
                    request.CardExpiryDate,
                    request.CardSecurityCode
                });

                builder.Property(request => request.Balance)
                    .HasColumnType("decimal(19,4)")
                    .HasDefaultValue(0);

                builder.Property(request => request.CreatedDate)
                    .HasColumnType("datetime2");
                builder.Property(request => request.UpdatedDate)
                    .HasColumnType("datetime2");
            });

            // Initial seed
            modelBuilder.Entity<AccountEntity>().HasData(new List<AccountEntity>
            {
                new AccountEntity
                {
                    CardNumber = "367839570755981",
                    CardExpiryDate = new DateTime(2029,12,29),
                    CardSecurityCode = 123,
                    Balance = 1000
                }
            });
        }
    }
}
