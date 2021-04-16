using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentGatewayDB.Entities;

namespace PaymentGatewayDB
{
    /// <summary>
    /// The payment gateway's database context
    /// </summary>
    public class PaymentGatewayDbContext : DbContext
    {
        public PaymentGatewayDbContext(DbContextOptions<PaymentGatewayDbContext> options) : base(options) { }

        public DbSet<PaymentRequestEntity> PaymentRequests { get; set; }

        /// <summary>
        /// Override save changes for automatic population of update date
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified)
                .ToList()
                .ForEach(e => e.Property("UpdatedDate").CurrentValue = DateTime.UtcNow);

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentRequestEntity>(builder =>
            {
                builder.HasKey(request => request.PaymentId);
                builder.HasIndex(request => request.MerchantId);

                builder.Property(request => request.PaymentId)
                    .ValueGeneratedOnAdd();
                builder.Property(request => request.MerchantId)
                    .ValueGeneratedNever();
                builder.Property(request => request.CardDetails)
                    .HasColumnType("varchar(max)")
                    .IsRequired();
                builder.Property(request => request.Amount)
                    .HasColumnType("decimal(18,4)");
                builder.Property(request => request.Currency)
                    .HasColumnType("char(3)")
                    .IsRequired();
                
                builder.Property(x => x.CreatedDate)
                    .HasColumnType("datetime2")
                    .HasDefaultValueSql("GETUTCDATE()")
                    .ValueGeneratedOnAdd();
                builder.Property(x => x.UpdatedDate)
                    .HasColumnType("datetime2")
                    .ValueGeneratedNever();
            });
        }
    }
}
