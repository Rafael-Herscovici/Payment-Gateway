using Common.Generics;
using CurrencyExchangeDB.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeDB
{
    /// <summary>
    /// The currency exchange database context
    /// </summary>
    public class CurrencyExchangeDbContext : DbContextSaveChangesOverride<CurrencyExchangeDbContext>
    {
        public CurrencyExchangeDbContext() { }
        public CurrencyExchangeDbContext(DbContextOptions<CurrencyExchangeDbContext> options) : base(options) { }

        public DbSet<CurrencyEntity> Currencies { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrencyEntity>(builder =>
            {
                builder.HasKey(request => request.Currency);
                builder.Property(currency => currency.Currency)
                    .ValueGeneratedNever()
                    .HasColumnType("char(3)");

                builder.Property(currency => currency.Rate)
                    .HasColumnType("decimal(19,4)");
            });
        }
    }
}
