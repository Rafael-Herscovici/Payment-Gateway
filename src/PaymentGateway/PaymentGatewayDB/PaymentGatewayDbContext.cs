using Common.Generics;
using Microsoft.EntityFrameworkCore;
using PaymentGatewayDB.Entities;

namespace PaymentGatewayDB
{
    /// <summary>
    /// The payment gateway's database context
    /// </summary>
    public class PaymentGatewayDbContext : DbContextSaveChangesOverride<PaymentGatewayDbContext>
    {
        public PaymentGatewayDbContext() { }
        public PaymentGatewayDbContext(DbContextOptions<PaymentGatewayDbContext> options) : base(options) { }

        public DbSet<PaymentRequestEntity> PaymentRequests { get; set; } = null!;

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
                    .HasColumnType("decimal(19,4)");
                builder.Property(request => request.Currency)
                    .HasColumnType("char(3)")
                    .IsRequired();
                builder.Property(request => request.PaymentStatus)
                    .IsRequired();

                builder.Property(request => request.CreatedDate)
                    .HasColumnType("datetime2");
                builder.Property(request => request.UpdatedDate)
                    .HasColumnType("datetime2");
            });
        }
    }
}
