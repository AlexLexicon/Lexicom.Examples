using Microsoft.Extensions.DependencyInjection;

namespace Lexicom.Examples.InventoryManagement.Client.Wpf.Extensions;
public static class MediatRServiceConfigurationExtensions
{
    public static void AddViewModels(this MediatRServiceConfiguration mediatRServiceConfiguration)
    {
        mediatRServiceConfiguration.RegisterServicesFromAssemblyContaining<AssemblyScanMarker>();
    }
}
