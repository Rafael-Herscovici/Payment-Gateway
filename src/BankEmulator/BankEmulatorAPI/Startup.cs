using BankEmulatorAPI.Models;
using BankEmulatorAPI.Services;
using BankEmulatorDB;
using CommonAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankEmulatorAPI
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
            services.AddDefaultServices<Startup, BankEmulatorDbContext>(Configuration);
            services.Configure<BankEmulatorOptions>(Configuration.GetSection(nameof(BankEmulatorOptions)));
            services.AddScoped<DbAccess>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.SetupDefaultApp<Startup>(env);
        }
    }
#pragma warning restore CS1591
}
