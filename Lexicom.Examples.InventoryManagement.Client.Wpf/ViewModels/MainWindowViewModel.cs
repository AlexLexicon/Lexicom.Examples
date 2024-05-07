using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lexicom.Examples.InventoryManagement.Client.Application.Models;
using Lexicom.Examples.InventoryManagement.Client.Application.Notifications;
using Lexicom.Examples.InventoryManagement.Client.Application.Services;
using Lexicom.Examples.InventoryManagement.Client.Wpf.Notifications;
using Lexicom.Examples.InventoryManagement.Client.Wpf.RuleSets;
using Lexicom.Examples.InventoryManagement.Client.Wpf.Validations;
using Lexicom.Mvvm;
using Lexicom.Validation;
using Lexicom.Wpf.DependencyInjection;
using MediatR;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Lexicom.Examples.InventoryManagement.Client.Wpf.ViewModels;
public partial class MainWindowViewModel : ObservableObject, IStartup, IShowableViewModel, INotificationHandler<NewProductNotification>, INotificationHandler<ProductFieldUpdatedNotification>
{
    private readonly IMediator _mediator;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IInventoryService _inventoryService;
    private readonly IProductService _productService;
    private readonly AddProductRecordValidation _createProductValidation;

    public MainWindowViewModel(
        ProductEditorViewModel productEditorViewModel,
        IRuleSetValidator<ProductRecordNameRuleSet, string?> addProductRecordNameValidator,
        IRuleSetValidator<ProductRecordMaximumStockRuleSet, string?> addProductRecordMaximumStockValidator,
        IRuleSetValidator<AddProductRecordCurrentStockRuleSet, string?> addProductRecordCurrentStockValidator,
        IMediator mediator,
        IViewModelFactory viewModelFactory,
        IInventoryService inventoryService,
        IProductService productService,
        AddProductRecordValidation createProductValidation)
    {
        _mediator = mediator;
        _viewModelFactory = viewModelFactory;
        _inventoryService = inventoryService;
        _productService = productService;
        _createProductValidation = createProductValidation;

        ProductEditorViewModel = productEditorViewModel;
        ProductRecordViewModels = [];
        AddProductRecordNameValidator = addProductRecordNameValidator;
        AddProductRecordMaximumStockValidator = addProductRecordMaximumStockValidator;
        AddProductRecordCurrentStockValidator = addProductRecordCurrentStockValidator;
    }

    public ICommand? ShowCommand { get; set; }

    [ObservableProperty]
    private ProductEditorViewModel _productEditorViewModel;

    [ObservableProperty]
    private ObservableCollection<ProductRecordViewModel> _productRecordViewModels;

    [ObservableProperty]
    private string? _addProductRecordName;

    [ObservableProperty]
    private IRuleSetValidator<ProductRecordNameRuleSet, string?> _addProductRecordNameValidator;

    [ObservableProperty]
    private string? _addProductRecordMaximumStock;

    [ObservableProperty]
    private IRuleSetValidator<ProductRecordMaximumStockRuleSet, string?> _addProductRecordMaximumStockValidator;

    [ObservableProperty]
    private string? _addProductRecordCurrentStock;

    [ObservableProperty]
    private IRuleSetValidator<AddProductRecordCurrentStockRuleSet, string?> _addProductRecordCurrentStockValidator;

    [ObservableProperty]
    private bool _isAddProductFormValid;

    public Task StartupAsync()
    {
        ShowCommand?.Execute(null);

        return Task.CompletedTask;
    }

    public async Task Handle(NewProductNotification notification, CancellationToken cancellationToken)
    {
        await LoadedAsync(cancellationToken);
    }

    public async Task Handle(ProductFieldUpdatedNotification notification, CancellationToken cancellationToken)
    {
        await LoadedAsync(cancellationToken);

        await _mediator.Publish(new ProductRecordSelectedNotification(notification.ProductId), cancellationToken);
    }

    [RelayCommand]
    private async Task LoadedAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Product> products = await _productService.GetProductsAsync(cancellationToken);

        ProductRecordViewModels.Clear();
        foreach (Product product in products)
        {
            var productRecodViewModel = _viewModelFactory.Create<ProductRecordViewModel, Guid>(product.Id);

            ProductRecordViewModels.Add(productRecodViewModel);
        }
    }

    [RelayCommand]
    private void ValidateAddProductForm()
    {
        if (int.TryParse(AddProductRecordMaximumStock, out int maximumStock))
        {
            _createProductValidation.MaximumStock = maximumStock;

            AddProductRecordCurrentStockValidator.Validate(AddProductRecordCurrentStock);
        }

        IsAddProductFormValid = AddProductRecordNameValidator.IsValid && AddProductRecordMaximumStockValidator.IsValid && AddProductRecordCurrentStockValidator.IsValid;
    }

    [RelayCommand]
    private async Task SubmitAddProductFormAsync(CancellationToken cancellationToken)
    {
        if (AddProductRecordName is not null && AddProductRecordCurrentStock is not null && AddProductRecordMaximumStock is not null)
        {
            await _inventoryService.CreateProductAsync(AddProductRecordName, AddProductRecordCurrentStock, AddProductRecordMaximumStock, cancellationToken);

            AddProductRecordName = null;
            AddProductRecordCurrentStock = null;
            AddProductRecordMaximumStock = null;
        }
    }
}
