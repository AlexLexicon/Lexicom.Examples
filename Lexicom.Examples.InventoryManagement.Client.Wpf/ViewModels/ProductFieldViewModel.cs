using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lexicom.Examples.InventoryManagement.Client.Application.Models;
using Lexicom.Examples.InventoryManagement.Client.Application.Services;
using Lexicom.Examples.InventoryManagement.Client.Wpf.Notifications;
using Lexicom.Examples.InventoryManagement.Client.Wpf.Validations;
using Lexicom.Validation;
using MediatR;
using System.Collections.ObjectModel;

namespace Lexicom.Examples.InventoryManagement.Client.Wpf.ViewModels;
public partial class ProductFieldViewModel : ObservableObject, INotificationHandler<ProductFieldLoadedNotification>, INotificationHandler<ProductEditorSavedNotification>, INotificationHandler<ProductRecordMaximumStockEditedNotification>
{
    private readonly IMediator _mediator;
    private readonly IProductFieldService _productFieldService;
    private readonly EditProductRecordValidation _editProductRecordValidation;

    public ProductFieldViewModel(
        IMediator mediator,
        ProductField productField,
        IProductFieldService productFieldService,
        EditProductRecordValidation editProductRecordValidation)
    {
        _mediator = mediator;
        _productFieldService = productFieldService;
        _editProductRecordValidation = editProductRecordValidation;

        ProductField = productField;
        Name = ProductField.Key;
        Value = ProductField.Value;
        Errors = [];
    }

    private string? PreviousValue { get; set; }

    public ProductField ProductField { get; }

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private string? _value;

    private IRuleSetValidator<string?>? _validator;
    public IRuleSetValidator<string?>? Validator
    {
        get => _validator;
        set
        {
            _validator = value;
            OnPropertyChanged(nameof(Validator));
            _validator?.Validation.Invoke(Value);
        }
    }

    [ObservableProperty]
    private ObservableCollection<string> _errors;

    public async Task Handle(ProductFieldLoadedNotification notification, CancellationToken cancellationToken)
    {
        OnPropertyChanged(nameof(Value));

        await UpdateFormValidationAsync(cancellationToken);
    }

    public async Task Handle(ProductEditorSavedNotification notification, CancellationToken cancellationToken)
    {
        if (Value is not null)
        {
            await _productFieldService.UpdateProductFieldAsync(ProductField.ProductId, ProductField.Key, Value, cancellationToken);
        }
    }

    public async Task Handle(ProductRecordMaximumStockEditedNotification notification, CancellationToken cancellationToken)
    {
        if (Validator is not null && ProductField.Key == InventoryService.FIELD_KEY_CURRENTSTOCK)
        {
            await Validator.ValidateAsync(Value, cancellationToken);

            await ValidateAsync(cancellationToken);
        }
    }

    [RelayCommand]
    private async Task LoadedAsync(CancellationToken cancellationToken)
    {
        await UpdateFormValidationAsync(cancellationToken);

        await _mediator.Publish(new ProductFieldLoadedNotification(), cancellationToken);
    }

    [RelayCommand]
    private async Task ValidateAsync(CancellationToken cancellationToken)
    {
        await UpdateFormValidationAsync(cancellationToken);

        if (Validator is not null)
        {
            Errors.Clear();
            foreach (string error in Validator.ValidationErrors)
            {
                Errors.Add(error);
            }
        }

        await _mediator.Publish(new ProductFieldValidatedNotification(), cancellationToken);

        if (Value != PreviousValue)
        {
            if (PreviousValue is not null)
            {
                await _mediator.Publish(new ProductFieldChangedNotification(), cancellationToken);
            }

            PreviousValue = Value;
        }
    }

    private async Task UpdateFormValidationAsync(CancellationToken cancellationToken)
    {
        if (ProductField.Key == InventoryService.FIELD_KEY_MAXIMUMSTOCK && int.TryParse(Value, out int maximumStock))
        {
            _editProductRecordValidation.MaximumStock = maximumStock;

            await _mediator.Publish(new ProductRecordMaximumStockEditedNotification(), cancellationToken);
        }
    }
}
