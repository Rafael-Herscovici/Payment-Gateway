using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.Generics
{
    public class DbContextSaveChangesOverride<TDbContext> : DbContext
        where TDbContext : DbContext
    {
        public DbContextSaveChangesOverride() { }
        public DbContextSaveChangesOverride(DbContextOptions<TDbContext> options) : base(options) { }

        /// <summary>
        /// Override save changes for automatic population of update date
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified)
                .ToList()
                .ForEach(e => e.Property(nameof(BaseEntity.UpdatedDate)).CurrentValue = DateTime.UtcNow);

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
