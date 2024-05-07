using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lexicom.Examples.InventoryManagement.Client.Application.Services;
using Lexicom.Examples.InventoryManagement.Client.Wpf.Notifications;
using MediatR;

namespace Lexicom.Examples.InventoryManagement.Client.Wpf.ViewModels;
public partial class ProductRecordViewModel : ObservableObject, INotificationHandler<ProductRecordSelectedNotification>
{
    private readonly Guid _productId;
    private readonly IMediator _mediator;
    private readonly IInventoryService _inventoryService;

    public ProductRecordViewModel(
        Guid productId,
        IMediator mediator,
        IInventoryService inventoryService)
    {
        _productId = productId;
        _mediator = mediator;
        _inventoryService = inventoryService;

        Id = _productId;
    }

    [ObservableProperty]
    private Guid _id;

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private string? _currentStock;

    [ObservableProperty]
    private string? _maximumStock;

    [ObservableProperty]
    private bool _isSelected;

    public Task Handle(ProductRecordSelectedNotification notification, CancellationToken cancellationToken)
    {
        IsSelected = notification.ProductId == _productId;

        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task SelectAsync(CancellationToken cancellationToken)
    {
        await _mediator.Publish(new ProductRecordSelectedNotification(_productId), cancellationToken);
    }

    [RelayCommand]
    private async Task LoadedAsync(CancellationToken cancellationToken)
    {
        var getProductNameTask = _inventoryService.GetProductNameAsync(_productId, cancellationToken);
        var getProductCurrentStockTask = _inventoryService.GetProductCurrentStockAsync(_productId, cancellationToken);
        var getProductMaximumStockTask = _inventoryService.GetProductMaximumStockAsync(_productId, cancellationToken);

        Name = await getProductNameTask;
        CurrentStock = await getProductCurrentStockTask;
        MaximumStock = await getProductMaximumStockTask;
    }
}
