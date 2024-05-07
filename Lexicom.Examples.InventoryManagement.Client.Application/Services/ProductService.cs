using Lexicom.Examples.InventoryManagement.Client.Application.Database;
using Lexicom.Examples.InventoryManagement.Client.Application.Exceptions;
using Lexicom.Examples.InventoryManagement.Client.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Lexicom.Examples.InventoryManagement.Client.Application.Services;
public interface IProductService
{
    Task<bool> DoesProductExistAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken cancellationToken);
    /// <exception cref="ProductDoesNotExistException"></exception>
    Task<Product> GetProductAsync(Guid id, CancellationToken cancellationToken);
    Task<Product> CreateProductAsync(CancellationToken cancellationToken);
}
public class ProductService : IProductService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public ProductService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<bool> DoesProductExistAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.Products.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.Products.ToListAsync(cancellationToken);
    }

    public async Task<Product> GetProductAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        Product? product = await db.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product is null)
        {
            throw new ProductDoesNotExistException(id);
        }

        return product;
    }

    public async Task<Product> CreateProductAsync(CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
        };

        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        await db.AddAsync(product, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return product;
    }
}