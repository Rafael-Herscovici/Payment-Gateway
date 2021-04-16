using BankEmulatorDB.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankEmulatorDB
{
    public class BankEmulatorDbContext : DbContext
    {
        public BankEmulatorDbContext(DbContextOptions<BankEmulatorDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(builder =>
            {
                builder.HasKey(request => request.CardNumber);
                builder.HasIndex(request => new
                {
                    request.CardNumber,
                    request.CardExpiryDate,
                    request.CardSecurityCode
                });

                builder.Property(request => request.Balance)
                    .HasColumnType("decimal(19,4)");

                builder.Property(request => request.CreatedDate)
                    .HasColumnType("datetime2")
                    .HasDefaultValueSql("GETUTCDATE()")
                    .ValueGeneratedOnAdd();
                builder.Property(request => request.UpdatedDate)
                    .HasColumnType("datetime2")
                    .ValueGeneratedNever();
            });
        }
    }
}
