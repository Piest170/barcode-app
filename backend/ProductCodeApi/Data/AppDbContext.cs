using Microsoft.EntityFrameworkCore;
using ProductCodeApi.Models;

namespace ProductCodeApi.Data;

public class AppDbContext : DbContext
{
    public DbSet<ProductCode> ProductCodes { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductCode>().HasIndex(p => p.Code).IsUnique();

        // base.OnModelCreating(modelBuilder);
    }
}