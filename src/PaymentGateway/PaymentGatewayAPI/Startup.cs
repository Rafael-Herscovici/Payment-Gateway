using CommonAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGatewayAPI.Models;
using PaymentGatewayAPI.Services;
using PaymentGatewayDB;

namespace PaymentGatewayAPI
{
#pragma warning disable CS1591
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultServices<Startup, PaymentGatewayDbContext>(Configuration);
            services.Configure<PaymentGatewayOptions>(Configuration.GetSection(nameof(PaymentGatewayOptions)));
            services.AddScoped<DbAccess>();
            services.AddSingleton<Encryption>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.SetupDefaultApp<Startup>(env);
        }
    }
#pragma warning restore CS1591
}
