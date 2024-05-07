using Lexicom.Examples.InventoryManagement.Client.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Lexicom.Examples.InventoryManagement.Client.Application.Database;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductField> ProductFields { get; set; }
}
