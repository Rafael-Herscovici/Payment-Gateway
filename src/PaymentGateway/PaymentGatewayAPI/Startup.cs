using System;
using System.Diagnostics.CodeAnalysis;
using CommonAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGatewayAPI.Models;
using PaymentGatewayAPI.Models.Mappings;
using PaymentGatewayAPI.Services;
using PaymentGatewayDB;
using Polly;
using Polly.Extensions.Http;

namespace PaymentGatewayAPI
{
#pragma warning disable CS1591
    [ExcludeFromCodeCoverage]
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
            services.AddSingleton<PaymentRequestEntityProfile.MaskCardDetailsResolver>();
            services.AddSingleton<PaymentRequestEntityProfile.EncryptCardDetailsResolver>();
            services.Configure<PaymentGatewayOptions>(Configuration.GetSection(nameof(PaymentGatewayOptions)));
            services.AddScoped<DbAccess>();
            services.AddSingleton<Encryption>();
            services.AddHttpClient<IBankAccess, BankAccess>(client => client.BaseAddress = new Uri(Configuration["BankEmulatorAPI"]))
                .AddPolicyHandler(requestMessage => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                    .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.SetupDefaultApp<Startup>(env);
        }
    }
#pragma warning restore CS1591
}
