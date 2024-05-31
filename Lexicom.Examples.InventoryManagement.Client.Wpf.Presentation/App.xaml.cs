using Lexicom.DependencyInjection.Primitives.Extensions;
using Lexicom.DependencyInjection.Primitives.For.Wpf.Extensions;
using Lexicom.Examples.InventoryManagement.Client.Application.Database;
using Lexicom.Examples.InventoryManagement.Client.Application.Extensions;
using Lexicom.Examples.InventoryManagement.Client.Wpf.Extensions;
using Lexicom.Examples.InventoryManagement.Client.Wpf.ViewModels;
using Lexicom.Mvvm.Amenities.Extensions;
using Lexicom.Mvvm.Extensions;
using Lexicom.Mvvm.For.Wpf.Extensions;
using Lexicom.Supports.Wpf.Extensions;
using Lexicom.Validation.Amenities.Extensions;
using Lexicom.Validation.For.Wpf.Extensions;
using Lexicom.Wpf.Amenities.Extensions;
using Lexicom.Wpf.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lexicom.Examples.InventoryManagement.Client.Wpf.Presentation;
public partial class App : System.Windows.Application
{
    public App()
    {
        var builder = WpfApplication.CreateBuilder(this);

        builder.Lexicom(l =>
        {
            l.AddAmenities();
            l.AddMvvm(options =>
            {
                options.AddMediatR(options =>
                {
                    options.AddViewModels();
                });

                options.AddViewModel<MainWindowViewModel>(options =>
                {
                    options.ForWindow<MainWindowView>();
                });
                options.AddViewModel<ProductEditorViewModel>();
                options.AddViewModel<ProductFieldViewModel>(ServiceLifetime.Transient);
                options.AddViewModel<ProductRecordViewModel>(ServiceLifetime.Transient);
            });

            l.AddPrimitives(p =>
            {
                p.AddGuidProvider();
            });

            l.AddValidation(options =>
            {
                options.AddAmenities();
                options.AddViewModels();
            });
        });

        builder.Services.AddApplication();
        builder.Services.AddViewModels();

        builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
        {
            string? cs = builder.Configuration.GetConnectionString("applicationdb");

            options.UseSqlite(cs);
        });

        var app = builder.Build();

        app.Startup<MainWindowViewModel>();
    }
}
