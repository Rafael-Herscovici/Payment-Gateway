using System;
using System.IO;
using System.Reflection;
using CommonAPI.HostedServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;

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
            var dbContextType = typeof(TDbContext);
            services
                .AddDbContext<TDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString(dbContextType.Name),
                        x => x.MigrationsAssembly(dbContextType.Namespace)))
                .AddHostedService<DbMigrationHostedService<TDbContext>>()
                .AddSwaggerGen(c =>
                {
                    var openApiInfo = configuration.GetSection(nameof(OpenApiInfo)).Get<OpenApiInfo>();
                    c.SwaggerDoc(openApiInfo.Version, openApiInfo);
                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{typeof(TStartup).Namespace}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath, true);
                })
                .AddAutoMapper(typeof(TStartup))
                .AddSwaggerGenNewtonsoftSupport()
                .AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.Converters.Add(new StringEnumConverter()));

            return services;
        }
    }
}
