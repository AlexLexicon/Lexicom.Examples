using Lexicom.Examples.InventoryManagement.Client.Application.Exceptions;
using Lexicom.Examples.InventoryManagement.Client.Application.Models;
using Lexicom.Examples.InventoryManagement.Client.Application.Notifications;
using MediatR;

namespace Lexicom.Examples.InventoryManagement.Client.Application.Services;
public interface IInventoryService
{
    /// <exception cref="ProductNameWasNotValidException"></exception>
    Task CreateProductAsync(string name, string currentStock, string maximumStock, CancellationToken cancellationToken);
    Task<string?> GetProductNameAsync(Guid productId, CancellationToken cancellationToken);
    Task<string?> GetProductCurrentStockAsync(Guid productId, CancellationToken cancellationToken);
    Task<string?> GetProductMaximumStockAsync(Guid productId, CancellationToken cancellationToken);
}
public class InventoryService : IInventoryService
{
    public const string FIELD_KEY_NAME = "name";
    public const string FIELD_KEY_CURRENTSTOCK = "current-stock";
    public const string FIELD_KEY_MAXIMUMSTOCK = "maximum-stock";

    private readonly IMediator _mediator;
    private readonly IProductService _productService;
    private readonly IProductFieldService _productFieldService;

    public InventoryService(
        IMediator mediator,
        IProductService productService,
        IProductFieldService productFieldService)
    {
        _mediator = mediator;
        _productService = productService;
        _productFieldService = productFieldService;
    }

    public async Task CreateProductAsync(string name, string currentStock, string maximumStock, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ProductNameWasNotValidException(name);
        }

        Product product = await _productService.CreateProductAsync(cancellationToken);

        var CreateNameProductFieldTask = _productFieldService.CreateProductFieldAsync(product.Id, FIELD_KEY_NAME, name, cancellationToken);
        var CreateCurrentStockProductFieldTask = _productFieldService.CreateProductFieldAsync(product.Id, FIELD_KEY_CURRENTSTOCK, currentStock, cancellationToken);
        var CreateMaximumStockProductFieldTask = _productFieldService.CreateProductFieldAsync(product.Id, FIELD_KEY_MAXIMUMSTOCK, maximumStock, cancellationToken);

        await CreateNameProductFieldTask;
        await CreateCurrentStockProductFieldTask;
        await CreateMaximumStockProductFieldTask;

        await _mediator.Publish(new NewProductNotification(product.Id), cancellationToken);
    }

    public async Task<string?> GetProductNameAsync(Guid productId, CancellationToken cancellationToken)
    {
        ProductField field = await _productFieldService.GetProductFieldAsync(productId, FIELD_KEY_NAME, cancellationToken);

        return field.Value;
    }

    public async Task<string?> GetProductCurrentStockAsync(Guid productId, CancellationToken cancellationToken)
    {
        ProductField field = await _productFieldService.GetProductFieldAsync(productId, FIELD_KEY_CURRENTSTOCK, cancellationToken);

        return field.Value;
    }

    public async Task<string?> GetProductMaximumStockAsync(Guid productId, CancellationToken cancellationToken)
    {
        ProductField field = await _productFieldService.GetProductFieldAsync(productId, FIELD_KEY_MAXIMUMSTOCK, cancellationToken);

        return field.Value;
    }
}
