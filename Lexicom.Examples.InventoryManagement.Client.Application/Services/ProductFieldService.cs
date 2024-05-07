using Lexicom.Examples.InventoryManagement.Client.Application.Database;
using Lexicom.Examples.InventoryManagement.Client.Application.Exceptions;
using Lexicom.Examples.InventoryManagement.Client.Application.Models;
using Lexicom.Examples.InventoryManagement.Client.Application.Notifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lexicom.Examples.InventoryManagement.Client.Application.Services;
public interface IProductFieldService
{
    Task<bool> DoesProductFieldExistAsync(Guid productId, string key, CancellationToken cancellationToken);
    /// <exception cref="ProductFieldDoesNotExistException"></exception>
    Task<ProductField> GetProductFieldAsync(Guid productId, string key, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductField>> GetProductFieldsAsync(Guid productId, CancellationToken cancellationToken);
    /// <exception cref="ProductFieldKeyWasNotValidException"></exception>
    /// <exception cref="ProductDoesNotExistException"></exception>
    /// <exception cref="ProductFieldAlreadyExistsException"></exception>
    Task<ProductField> CreateProductFieldAsync(Guid productId, string key, string value, CancellationToken cancellationToken);
    Task UpdateProductFieldAsync(Guid productId, string key, string value, CancellationToken cancellationToken);
}
public class ProductFieldService : IProductFieldService
{
    private readonly IMediator _mediator;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IProductService _productService;

    public ProductFieldService(
        IMediator mediator,
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        IProductService productService)
    {
        _mediator = mediator;
        _dbContextFactory = dbContextFactory;
        _productService = productService;
    }

    public async Task<bool> DoesProductFieldExistAsync(Guid productId, string key, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.ProductFields.AnyAsync(pf => pf.ProductId == productId && pf.Key == key, cancellationToken);
    }

    public async Task<ProductField> GetProductFieldAsync(Guid productId, string key, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        ProductField? productField = await db.ProductFields.FirstOrDefaultAsync(pf => pf.ProductId == productId && pf.Key == key, cancellationToken);
        if (productField is null)
        {
            throw new ProductFieldDoesNotExistException(productId, key);
        }

        return productField;
    }

    public async Task<IReadOnlyList<ProductField>> GetProductFieldsAsync(Guid productId, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.ProductFields
            .Where(pf => pf.ProductId == productId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductField> CreateProductFieldAsync(Guid productId, string key, string value, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ProductFieldKeyWasNotValidException(key);
        }

        key = key.ToLowerInvariant();

        bool productExists = await _productService.DoesProductExistAsync(productId, cancellationToken);
        if (!productExists)
        {
            throw new ProductDoesNotExistException(productId);
        }

        bool productFieldExists = await DoesProductFieldExistAsync(productId, key, cancellationToken);
        if (productFieldExists)
        {
            throw new ProductFieldAlreadyExistsException(productId, key);
        }

        var productField = new ProductField
        {
            Key = key,
            Value = value,
            ProductId = productId,
        };

        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        await db.ProductFields.AddAsync(productField, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return productField;
    }

    public async Task UpdateProductFieldAsync(Guid productId, string key, string newValue, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var productField = await db.ProductFields.FirstOrDefaultAsync(pf => pf.ProductId == productId && pf.Key == key, cancellationToken);

        if (productField is null)
        {
            throw new ProductFieldDoesNotExistException(productId, key);
        }

        productField.Value = newValue;

        await db.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new ProductFieldUpdatedNotification(productId, key), cancellationToken);
    }
}
