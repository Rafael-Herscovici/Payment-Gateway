using CommonAPI.HostedServices;
using CurrencyExchange;
using CurrencyExchange.HostedServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using System;
using System.IO;

namespace CommonAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDefaultServices<TStartup, TDbContext>(
            this IServiceCollection services,
            IConfiguration configuration)
            where TStartup : class
            where TDbContext : DbContext
        {
            services

                .AddDbContext<TDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetValue<string>(typeof(TDbContext).Name),
                        x => x.MigrationsAssembly(typeof(TDbContext).Namespace)))
                .AddDbContext<CurrencyExchangeDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetValue<string>(typeof(CurrencyExchangeDbContext).Name),
                        x => x.MigrationsAssembly(typeof(CurrencyExchangeDbContext).Namespace)))

                .AddHostedService<DbMigrationHostedService<TDbContext>>()
                .AddHostedService<CurrencyExchangeHostedService>()

                .AddSwaggerGen(c =>
                {
                    var openApiInfo = configuration.GetSection(nameof(OpenApiInfo)).Get<OpenApiInfo>();
                    c.SwaggerDoc(openApiInfo.Version, openApiInfo);
                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{typeof(TStartup).Namespace}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath, true);
                })
                .AddSwaggerGenNewtonsoftSupport()
                .AddAutoMapper(typeof(TStartup))
                .AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.Converters.Add(new StringEnumConverter()));

            return services;
        }
    }
}
