using Lexicom.Validation;
using Lexicom.Validation.Extensions;

namespace Lexicom.Examples.InventoryManagement.Client.Wpf.Extensions;
public static class ValidationServiceBuilderExtensions
{
    public static void AddViewModels(this IValidationServiceBuilder builder)
    {
        builder.AddRuleSets<AssemblyScanMarker>();//todo ServiceLifetime.Transient
    }
}
