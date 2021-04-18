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

        public override int SaveChanges()
        {
            SetDates();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetDates();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            SetDates();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void SetDates()
        {
            var utcNow = DateTime.UtcNow;
            ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                                e.State == EntityState.Added
                                || e.State == EntityState.Modified))
                .ToList()
                .ForEach(entityEntry =>
                {
                    switch (entityEntry.State)
                    {
                        case EntityState.Modified:
                            ((BaseEntity)entityEntry.Entity).UpdatedDate = utcNow;
                            break;
                        case EntityState.Added:
                            ((BaseEntity)entityEntry.Entity).CreatedDate = utcNow;
                            break;
                    }
                });
        }
    }
}
