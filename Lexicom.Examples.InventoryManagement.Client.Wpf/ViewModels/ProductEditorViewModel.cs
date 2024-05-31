using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lexicom.Examples.InventoryManagement.Client.Application.Models;
using Lexicom.Examples.InventoryManagement.Client.Application.Services;
using Lexicom.Examples.InventoryManagement.Client.Wpf.Notifications;
using Lexicom.Examples.InventoryManagement.Client.Wpf.RuleSets;
using Lexicom.Mvvm;
using Lexicom.Validation;
using MediatR;
using System.Collections.ObjectModel;

namespace Lexicom.Examples.InventoryManagement.Client.Wpf.ViewModels;
public partial class ProductEditorViewModel : ObservableObject, INotificationHandler<ProductRecordSelectedNotification>, INotificationHandler<ProductFieldValidatedNotification>, INotificationHandler<ProductFieldChangedNotification>
{
    private readonly IMediator _mediator;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IProductFieldService _productFieldService;
    private readonly IProductService _productService;
    private readonly IRuleSetValidator<ProductRecordNameRuleSet, string?> _editProductRecordNameValidator;
    private readonly IRuleSetValidator<ProductRecordMaximumStockRuleSet, string?> _editProductRecordMaximumStockValidator;
    private readonly IRuleSetValidator<EditProductRecordCurrentStockRuleSet, string?> _editProductRecordCurrentStockValidator;

    public ProductEditorViewModel(
        IMediator mediator,
        IViewModelFactory viewModelFactory,
        IProductFieldService productFieldService,
        IProductService productService,
        IRuleSetValidator<ProductRecordNameRuleSet, string?> editProductRecordNameValidator,
        IRuleSetValidator<ProductRecordMaximumStockRuleSet, string?> editProductRecordMaximumStockValidator,
        IRuleSetValidator<EditProductRecordCurrentStockRuleSet, string?> editProductRecordCurrentStockValidator,
        IRuleSetValidator<AddProductFieldKeyRuleSet, string?> addProductFieldKeyValidator,
        IRuleSetValidator<AddProductFieldValueRuleSet, string?> addProductFieldValueValidator)
    {
        _mediator = mediator;
        _viewModelFactory = viewModelFactory;
        _productFieldService = productFieldService;
        _productService = productService;
        _editProductRecordNameValidator = editProductRecordNameValidator;
        _editProductRecordMaximumStockValidator = editProductRecordMaximumStockValidator;
        _editProductRecordCurrentStockValidator = editProductRecordCurrentStockValidator;

        ItemViewModels = [];
        AddProductFieldKeyValidator = addProductFieldKeyValidator;
        AddProductFieldValueValidator = addProductFieldValueValidator;
    }

    [ObservableProperty]
    private bool _isVisible;

    [ObservableProperty]
    private Guid _id;

    [ObservableProperty]
    private ObservableCollection<ProductFieldViewModel> _itemViewModels;

    [ObservableProperty]
    private bool _isValid;

    [ObservableProperty]
    private bool _canSave;

    [ObservableProperty]
    private bool _isCreateFieldFormValid;

    [ObservableProperty]
    private string? _createFieldKey;

    [ObservableProperty]
    private IRuleSetValidator<AddProductFieldKeyRuleSet, string?> _addProductFieldKeyValidator;

    [ObservableProperty]
    private string? _createFieldValue;

    [ObservableProperty]
    private IRuleSetValidator<AddProductFieldValueRuleSet, string?> _addProductFieldValueValidator;

    public async Task Handle(ProductRecordSelectedNotification notification, CancellationToken cancellationToken)
    {
        IsVisible = notification.ProductId is not null;

        if (notification.ProductId is not null)
        {
            Id = notification.ProductId.Value;

            IReadOnlyList<ProductField> products = await _productFieldService.GetProductFieldsAsync(Id, cancellationToken);

            ItemViewModels.Clear();
            foreach (ProductField productField in products)
            {
                var itemViewModel = _viewModelFactory.Create<ProductFieldViewModel, ProductField>(productField);

                if (productField.Key is InventoryService.FIELD_KEY_NAME)
                {
                    itemViewModel.Validator = _editProductRecordNameValidator;
                }
                else if (productField.Key is InventoryService.FIELD_KEY_MAXIMUMSTOCK)
                {
                    itemViewModel.Validator = _editProductRecordMaximumStockValidator;
                }
                else if (productField.Key is InventoryService.FIELD_KEY_CURRENTSTOCK)
                {
                    itemViewModel.Validator = _editProductRecordCurrentStockValidator;
                }

                ItemViewModels.Add(itemViewModel);
            }
        }

        await _mediator.Publish(new ProductFieldLoadedNotification(), cancellationToken);

        CanSave = false;
    }

    public Task Handle(ProductFieldValidatedNotification notification, CancellationToken cancellationToken)
    {
        IsValid = _editProductRecordNameValidator.IsValid && _editProductRecordMaximumStockValidator.IsValid && _editProductRecordCurrentStockValidator.IsValid;

        if (!IsValid)
        {
            CanSave = false;
        }

        return Task.CompletedTask;
    }

    public Task Handle(ProductFieldChangedNotification notification, CancellationToken cancellationToken)
    {
        CanSave = IsValid;

        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CloseAsync(CancellationToken cancellationToken)
    {
        await _mediator.Publish(new ProductRecordSelectedNotification(null), cancellationToken);
    }

    [RelayCommand]
    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _mediator.Publish(new ProductEditorSavedNotification(Id), cancellationToken);

        CanSave = false;
    }

    [RelayCommand]
    private async Task DeleteAsync(CancellationToken cancellationToken)
    {
        await _productService.DeleteProductAsync(Id, cancellationToken);

        await CloseAsync(cancellationToken);
    }

    [RelayCommand]
    private void ValidateCreateFieldForm()
    {
        IsCreateFieldFormValid = AddProductFieldKeyValidator.IsValid && AddProductFieldValueValidator.IsValid;
    }

    [RelayCommand]
    private async Task SubmitCreateFieldFormAsync(CancellationToken cancellationToken)
    {
        if (CreateFieldKey is not null && CreateFieldValue is not null)
        {
            await _productFieldService.CreateProductFieldAsync(Id, CreateFieldKey, CreateFieldValue, cancellationToken);

            await _mediator.Publish(new ProductRecordSelectedNotification(Id), cancellationToken);

            CreateFieldKey = null;
            CreateFieldValue = null;
        }
    }
}
