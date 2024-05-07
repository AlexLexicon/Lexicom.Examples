using FluentValidation;
using Lexicom.Examples.InventoryManagement.Client.Wpf.Validations;
using Lexicom.Validation;
using Lexicom.Validation.Amenities.Extensions;

namespace Lexicom.Examples.InventoryManagement.Client.Wpf.RuleSets;
public class AddProductRecordCurrentStockRuleSet : AbstractRuleSet<string?>
{
    private readonly AddProductRecordValidation _addProductRecordValidation;

    public AddProductRecordCurrentStockRuleSet(AddProductRecordValidation addProductRecordValidation)
    {
        _addProductRecordValidation = addProductRecordValidation;
    }

    public override void Use<T>(IRuleBuilderOptions<T, string?> ruleBuilder)
    {
        ruleBuilder
            .NotNull()
            .NotSimplyEmpty()
            .Digits()
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(() => _addProductRecordValidation.MaximumStock);
    }
}
