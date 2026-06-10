using CommerceOps.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CommerceOps.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<SyncLog> SyncLogs => Set<SyncLog>();
    public DbSet<IntegrationHealth> IntegrationHealths => Set<IntegrationHealth>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ExternalOrderId).HasMaxLength(100).IsRequired();
            e.Property(x => x.Channel).HasMaxLength(50).IsRequired();
            e.Property(x => x.CustomerName).HasMaxLength(200).IsRequired();
            e.Property(x => x.TotalAmount).HasPrecision(18, 2);
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.ExternalOrderId);
            e.HasIndex(x => x.Channel);
            e.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ExternalProductId).HasMaxLength(100).IsRequired();
            e.Property(x => x.Name).HasMaxLength(500).IsRequired();
            e.Property(x => x.Price).HasPrecision(18, 2);
            e.Property(x => x.Channel).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.ExternalProductId);
            e.HasIndex(x => x.Channel);
        });

        modelBuilder.Entity<SyncLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Channel).HasMaxLength(50).IsRequired();
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.Property(x => x.Message).HasMaxLength(2000);
            e.HasIndex(x => x.Channel);
            e.HasIndex(x => x.CreatedAt);
        });

        modelBuilder.Entity<IntegrationHealth>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Channel).HasMaxLength(50).IsRequired();
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Channel).IsUnique();
        });
    }
}
