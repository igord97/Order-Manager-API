using Microsoft.EntityFrameworkCore;
using LearningProject1.Core.Models;

namespace LearningProject1.DataLayer.Data;

public class AppDbContext : DbContext
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

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.Name)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(u => u.Email)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(u => u.CreatedAt)
                .HasColumnType("datetime2(3)")
                .IsRequired();

            entity.Property(u => u.UpdatedAt)
                .HasColumnType("datetime2(3)");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(o => o.Product)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(o => o.Quantity)
                .IsRequired();

            entity.Property(o => o.Price)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(o => o.Total)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(o => o.CreatedAt)
                .IsRequired();

            entity.Property(u => u.CreatedAt)
                .HasColumnType("datetime2(3)")
                .IsRequired();

            entity.Property(u => u.UpdatedAt)
                .HasColumnType("datetime2(3)");
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is User or Order &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            if (entry.Entity is User user)
            {
                if (entry.State == EntityState.Added)
                    user.CreatedAt = now;

                if (entry.State == EntityState.Modified)
                    user.UpdatedAt = now;
            }

            if (entry.Entity is Order order)
            {
                if (entry.State == EntityState.Added)
                    order.CreatedAt = now;

                if (entry.State == EntityState.Modified)
                    order.UpdatedAt = now;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}