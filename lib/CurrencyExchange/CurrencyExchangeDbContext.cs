using Common.Entities;
using Common.Generics;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange
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

                builder.Property(request => request.CreatedDate)
                    .HasColumnType("datetime2");
                builder.Property(request => request.UpdatedDate)
                    .HasColumnType("datetime2");
            });
        }
    }
}
