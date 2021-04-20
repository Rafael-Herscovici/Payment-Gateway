using CurrencyExchange;
using Microsoft.EntityFrameworkCore;
using PaymentGatewayDB;

namespace PaymentGateway.UnitTests
{
    public abstract class UnitTestBase
    {
        public PaymentGatewayDbContext GetGatewayDbContext() =>
            new PaymentGatewayDbContext(GetInMemoryDbContextOptions<PaymentGatewayDbContext>());
        public CurrencyExchangeDbContext GetExchangeDbContext() =>
            new CurrencyExchangeDbContext(GetInMemoryDbContextOptions<CurrencyExchangeDbContext>());
        public DbContextOptions<TDbContext> GetInMemoryDbContextOptions<TDbContext>()
            where TDbContext : DbContext =>
            new DbContextOptionsBuilder<TDbContext>()
                .UseInMemoryDatabase(databaseName: nameof(PaymentGatewayDB))
                .Options;
    }
}
