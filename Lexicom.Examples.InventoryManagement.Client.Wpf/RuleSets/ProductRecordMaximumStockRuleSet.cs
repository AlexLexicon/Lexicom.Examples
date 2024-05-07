using FluentValidation;
using Lexicom.Validation;
using Lexicom.Validation.Amenities.Extensions;

namespace Lexicom.Examples.InventoryManagement.Client.Wpf.RuleSets;
public class ProductRecordMaximumStockRuleSet : AbstractRuleSet<string?>
{
    public override void Use<T>(IRuleBuilderOptions<T, string?> ruleBuilder)
    {
        ruleBuilder
            .NotNull()
            .NotSimplyEmpty()
            .Digits()
            .GreaterThanOrEqualTo(0);
    }
}
