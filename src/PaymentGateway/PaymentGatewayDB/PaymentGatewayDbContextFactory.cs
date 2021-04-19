using Common.Generics;
using System.Diagnostics.CodeAnalysis;

namespace PaymentGatewayDB
{
    [ExcludeFromCodeCoverage]
    public class PaymentGatewayDbContextFactory : DesignTimeDbContextFactory<PaymentGatewayDbContext>
    {
    }
}
