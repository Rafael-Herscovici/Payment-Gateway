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

        /// <summary>
        /// Existing payments
        /// </summary>
        public DbSet<PaymentRequest> PaymentRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentRequest>(builder =>
            {
                builder.HasKey(request => request.PaymentId);
                builder.HasIndex(request => request.MerchantId);

                builder.Property(request => request.PaymentId)
                    .ValueGeneratedOnAdd();
                builder.Property(request => request.MerchantId)
                    .ValueGeneratedNever();
                builder.Property(request => request.CardDetails)
                    .IsRequired();
                builder.Property(request => request.Amount)
                    .HasColumnType("decimal(18,4)");
                builder.Property(request => request.Currency)
                    .HasColumnType("char(3)")
                    .IsRequired();
            });
        }
    }
}
