using Microsoft.EntityFrameworkCore;
using OrderManager.DataLayer.BusinessObjects.Persistent.dbo;

namespace OrderManager.DataLayer.Context;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.ClrType == typeof(string) && p.GetMaxLength() == null)
            .ToList()
            .ForEach(p => p.SetMaxLength(255));

        modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?))
            .ToList()
            .ForEach(p => p.SetColumnType("datetime2(2)"));

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    protected virtual void OnBeforeSaving()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
    }

    protected virtual void OnPostSaveChanges()
    {
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaving();

        var result = base.SaveChanges(acceptAllChangesOnSuccess);

        OnPostSaveChanges();

        return result;
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();

        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        OnPostSaveChanges();

        return result;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return SaveChangesAsync(true, cancellationToken);
    }

    public new TEntity? Find<TEntity>(params object[] ids)
        where TEntity : class
    {
        return Set<TEntity>().Find(ids);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}