using Lexicom.Examples.InventoryManagement.Client.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lexicom.Examples.InventoryManagement.Client.Application.Extensions;
public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IInventoryService, InventoryService>();
        services.AddSingleton<IProductFieldService, ProductFieldService>();
        services.AddSingleton<IProductService, ProductService>();
    }
}
