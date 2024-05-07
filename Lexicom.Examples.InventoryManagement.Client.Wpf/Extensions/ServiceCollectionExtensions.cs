using Lexicom.Examples.InventoryManagement.Client.Wpf.Validations;
using Microsoft.Extensions.DependencyInjection;

namespace Lexicom.Examples.InventoryManagement.Client.Wpf.Extensions;
public static class ServiceCollectionExtensions
{
    public static void AddViewModels(this IServiceCollection services)
    {
        services.AddSingleton<AddProductRecordValidation>();
        services.AddSingleton<EditProductRecordValidation>();
    }
}
