using FluentValidation;
using Lexicom.Examples.InventoryManagement.Client.Wpf.Validations;
using Lexicom.Validation;
using Lexicom.Validation.Amenities.Extensions;

namespace Lexicom.Examples.InventoryManagement.Client.Wpf.RuleSets;
public class EditProductRecordCurrentStockRuleSet : AbstractRuleSet<string?>
{
    private readonly EditProductRecordValidation _editProductRecordValidation;

    public EditProductRecordCurrentStockRuleSet(EditProductRecordValidation editProductRecordValidation)
    {
        _editProductRecordValidation = editProductRecordValidation;
    }

    public override void Use<T>(IRuleBuilderOptions<T, string?> ruleBuilder)
    {
        ruleBuilder
            .NotNull()
            .NotSimplyEmpty()
            .Digits()
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(() => _editProductRecordValidation.MaximumStock);
    }
}
