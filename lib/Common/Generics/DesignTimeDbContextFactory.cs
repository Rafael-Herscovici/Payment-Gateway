using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Common.Generics
{
    [ExcludeFromCodeCoverage]
    public class DesignTimeDbContextFactory<TDbContext> : IDesignTimeDbContextFactory<TDbContext>
        where TDbContext : DbContext, new()
    {
        public TDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<TDbContext>()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
            optionsBuilder.UseSqlServer(configuration[typeof(TDbContext).Name]);

            return Activator.CreateInstance(typeof(TDbContext), optionsBuilder.Options) as TDbContext;
        }
    }
}
