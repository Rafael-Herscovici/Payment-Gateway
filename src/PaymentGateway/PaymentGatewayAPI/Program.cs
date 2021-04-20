using System.Diagnostics.CodeAnalysis;

namespace PaymentGatewayAPI
{
#pragma warning disable CS1591
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            CommonAPI.Program<Startup>
                .SetupProgram(args);
        }
    }
#pragma warning restore CS1591
}
