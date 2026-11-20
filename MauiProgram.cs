using Microsoft.Extensions.Logging;
using UsuariosApp.Services;
using UsuariosApp.ViewModels;
using UsuariosApp.Views;

namespace UsuariosApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Servicios (una sola instancia para toda la app)
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();

        // ViewModels
        builder.Services.AddTransient<UsuariosViewModel>();
        builder.Services.AddTransient<UsuarioDetalleViewModel>();
        builder.Services.AddTransient<NuevoUsuarioViewModel>();

        // Vistas
        builder.Services.AddTransient<UsuariosPage>();
        builder.Services.AddTransient<UsuarioDetallePage>();
        builder.Services.AddTransient<NuevoUsuarioPage>();

        return builder.Build();
    }
}
